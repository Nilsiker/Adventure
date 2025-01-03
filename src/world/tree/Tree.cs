namespace Shellguard.Tree;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Player;
using Shellguard.Traits;

public interface ITree : IStaticBody2D, IDamageable { }

[Meta(typeof(IAutoNode))]
public partial class Tree : StaticBody2D, ITree
{
  #region Exports
  [Export]
  private float _occlusionTransparency = 0.6f;
  #endregion

  #region State
  private TreeLogic Logic { get; set; } = default!;
  private TreeLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
  [Node]
  private Area2D FadeArea { get; set; } = default!;

  [Node]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;

  [Node]
  private AudioStreamPlayer2D AudioChop { get; set; } = default!;

  [Node]
  private Sprite2D Canopy { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new();

    FadeArea.BodyEntered += OnFadeAreaBodyEntered;
    FadeArea.BodyExited += OnFadeAreaBodyExited;
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle(
        (in TreeLogic.Output.UpdateTransparency output) => OnOutputUpdateTransparency(output.Alpha)
      )
      .Handle((in TreeLogic.Output.Rustle output) => OnOutputRustle(output.Strength))
      .Handle((in TreeLogic.Output.Damaged _) => OnOutputDamaged());

    Logic.Set(
      new TreeLogic.Data
      {
        Age = 0.0f,
        Health = 1.0f,
        TimeToMature = 10.0f,
      }
    );

    Logic.Start();
  }
  #endregion



  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady()
  {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  private void OnFadeAreaBodyEntered(Node2D body)
  {
    if (body is IPlayer)
    {
      Logic.Input(new TreeLogic.Input.OccludingEntity(true));
    }
  }

  private void OnFadeAreaBodyExited(Node2D body)
  {
    if (body is IPlayer)
    {
      Logic.Input(new TreeLogic.Input.OccludingEntity(false));
    }
  }
  #endregion

  #region Output Callbacks
  private Tween? _canopyFadeTween; // NOTE is there a better place to put this?

  private void OnOutputUpdateTransparency(float alpha)
  {
    if (_canopyFadeTween != null && _canopyFadeTween.IsRunning())
    {
      _canopyFadeTween.Kill();
    }

    _canopyFadeTween = CreateTween();
    _canopyFadeTween.TweenProperty(
      Canopy,
      "modulate:a",
      alpha,
      1.0 - Math.Abs(Canopy.Modulate.A - alpha)
    );
  }

  private Tween? _rustleTween; // NOTE is there a better place to put this?

  private void OnOutputRustle(float strength) => AnimationPlayer.Play("rustle");

  private void OnOutputDamaged() => AudioChop.Play();
  #endregion

  public override void _UnhandledInput(InputEvent @event)
  {
    if (Input.IsKeyLabelPressed(Key.P))
    {
      Damage(1.0f);
    }
  }

  #region IDamageable
  public void Damage(float damage) => Logic.Input(new TreeLogic.Input.Damage(damage));
  #endregion
}

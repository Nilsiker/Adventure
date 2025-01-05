namespace Shellguard.Tree;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface ITreeModel : INode2D { }

[Meta(typeof(IAutoNode))]
public partial class TreeModel : Node2D, ITreeModel
{
  #region Exports
  #endregion

  #region Nodes
  [Node]
  private Area2D Canopy { get; set; } = default!;

  [Node]
  private AnimatedSprite2D StumpSprite { get; set; } = default!;

  [Node]
  private AnimatedSprite2D TrunkSprite { get; set; } = default!;

  [Node]
  private AnimatedSprite2D CanopySprite { get; set; } = default!;

  [Node]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;

  [Node]
  private CpuParticles2D LeafParticles { get; set; } = default!;

  #endregion

  #region Dependencies
  [Dependency]
  private ITreeLogic Logic => this.DependOn<ITreeLogic>();
  #endregion

  #region State
  private TreeLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Canopy.BodyEntered += OnCanopyBodyEntered;
    Canopy.BodyExited += OnCanopyBodyExited;
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();
    Binding
      .Handle(
        (in TreeLogic.Output.UpdateTransparency output) => OnOutputUpdateTransparency(output.Alpha)
      )
      .Handle((in TreeLogic.Output.StageUpdated output) => OnOutputStageUpdated(output.Stage));
  }
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady()
  {
    SetProcess(false);
    SetPhysicsProcess(false);
  }

  public void OnExitTree() => Binding.Dispose();
  #endregion

  #region Input Callbacks
  private void OnCanopyBodyExited(Node2D body) =>
    Logic.Input(new TreeLogic.Input.OccludingEntity(false));

  private void OnCanopyBodyEntered(Node2D body) =>
    Logic.Input(new TreeLogic.Input.OccludingEntity(true));
  #endregion

  #region Output Callbacks
  private void OnOutputStageUpdated(int stage)
  {
    StumpSprite.Frame = stage;
    TrunkSprite.Frame = stage;
    CanopySprite.Frame = stage;
  }

  private Tween? _canopyFadeTween; // NOTE is there a better place to put this?

  private void OnOutputUpdateTransparency(float alpha)
  {
    if (_canopyFadeTween != null && _canopyFadeTween.IsRunning())
    {
      _canopyFadeTween.Kill();
    }

    _canopyFadeTween = CreateTween();
    _canopyFadeTween.TweenProperty(
      CanopySprite,
      "modulate:a",
      alpha,
      1.0 - Math.Abs(CanopySprite.Modulate.A - alpha)
    );
  }

  private void OnOutputRustle(float strength)
  {
    AnimationPlayer.Play("rustle");
    LeafParticles.Emitting = true;
  }
  #endregion
}

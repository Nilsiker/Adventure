namespace Shellguard.Tree;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Shellguard.Player;

public interface IBush : IArea2D { }

[Meta(typeof(IAutoNode))]
public partial class Bush : Area2D, IBush
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
  private Sprite2D Greenery { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new();

    FadeArea.BodyEntered += OnFadeAreaBodyEntered;
    FadeArea.BodyExited += OnFadeAreaBodyExited;
  }

  private Tween _canopyFadeTween = default!;

  private void OnFadeAreaBodyExited(Node2D body)
  {
    if (body is IPlayer)
    {
      if (_canopyFadeTween != null && _canopyFadeTween.IsRunning())
      {
        _canopyFadeTween.Kill();
      }

      _canopyFadeTween = CreateTween();
      _canopyFadeTween.TweenProperty(Greenery, "modulate:a", 1.0f, 1.0 - Greenery.Modulate.A);
    }
  }

  private void OnFadeAreaBodyEntered(Node2D body)
  {
    if (body is IPlayer)
    {
      if (_canopyFadeTween != null && _canopyFadeTween.IsRunning())
      {
        _canopyFadeTween.Kill();
      }

      _canopyFadeTween = CreateTween();
      _canopyFadeTween.TweenProperty(
        Greenery,
        "modulate:a",
        _occlusionTransparency,
        Greenery.Modulate.A - _occlusionTransparency
      );
    }
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here

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
  #endregion

  #region Output Callbacks
  #endregion
}

public interface IBushLogic : ILogicBlock<BushLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class BushLogic : LogicBlock<BushLogic.State>, IBushLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output { }

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}

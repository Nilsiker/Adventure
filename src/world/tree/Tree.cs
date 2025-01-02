namespace Shellguard;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Shellguard.Player;

public interface ITree : IStaticBody2D
{
  void Chop(float strength);
}

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
  private Sprite2D Canopy { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new();

    FadeArea.BodyEntered += OnFadeAreaBodyEntered;
    FadeArea.BodyExited += OnFadeAreaBodyExited;
  }

  private Tween? _canopyFadeTween;

  private void OnFadeAreaBodyExited(Node2D body)
  {
    if (body is IPlayer)
    {
      if (_canopyFadeTween != null && _canopyFadeTween.IsRunning())
      {
        _canopyFadeTween.Kill();
      }

      _canopyFadeTween = CreateTween();
      _canopyFadeTween.TweenProperty(Canopy, "modulate:a", 1.0f, 1.0 - Canopy.Modulate.A);
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
        Canopy,
        "modulate:a",
        _occlusionTransparency,
        Canopy.Modulate.A - _occlusionTransparency
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

  #region IChoppable
  public void Chop(float strength) { }
  #endregion
}

public interface ITreeLogic : ILogicBlock<TreeLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class TreeLogic : LogicBlock<TreeLogic.State>, ITreeLogic
{
  public override Transition GetInitialState() => To<State>();

  public enum Stage
  {
    Seedling,
    Sapling,
    Young,
    Mature,
  }

  public struct Data
  {
    public float Health;
    public Stage Stage;
    public int Age;
  }

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

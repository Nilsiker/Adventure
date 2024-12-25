using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Shellguard.Game.Domain;

public interface IPauseMenu : IControl { }

[Meta(typeof(IAutoNode))]
public partial class PauseMenu : Control, IPauseMenu
{
  #region Exports
  #endregion

  #region State
  private PauseMenuLogic Logic { get; set; } = default!;
  private PauseMenuLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency]
  IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region Nodes
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();

    Logic.Set(GameRepo);
    // Bind functions to state outputs here
    Binding.Handle((in PauseMenuLogic.Output.VisibilityChanged output) => Visible = output.Visible);

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

public interface IPauseMenuLogic : ILogicBlock<PauseMenuLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class PauseMenuLogic : LogicBlock<PauseMenuLogic.State>, IPauseMenuLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output
  {
    public partial record struct VisibilityChanged(bool Visible);
  }

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync += OnGameIsPaused;
      });
      OnDetach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync -= OnGameIsPaused;
      });
    }

    private void OnGameIsPaused(bool paused) => Output(new Output.VisibilityChanged(paused));
  }
}

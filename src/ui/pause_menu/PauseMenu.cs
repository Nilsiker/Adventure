namespace Shellguard.UI;

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
  private IGameRepo GameRepo => this.DependOn<IGameRepo>();

  [Dependency]
  private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region Nodes
  [Node("%Resume")]
  private Button ResumeButton { get; set; } = default!;

  [Node("%Options")]
  private Button OptionsButton { get; set; } = default!;

  [Node("%QuitToMainMenu")]
  private Button QuitToMainMenuButton { get; set; } = default!;

  [Node("%QuitToDesktop")]
  private Button QuitToDesktopButton { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();

    Logic.Set(GameRepo);
    Logic.Set(AppRepo);

    // Bind functions to state outputs here
    Binding.Handle((in PauseMenuLogic.Output.VisibilityChanged output) => Visible = output.Visible);

    Logic.Start();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady()
  {
    GD.Print(ResumeButton);
    ResumeButton.Pressed += OnResumeButtonPressed;
    QuitToMainMenuButton.Pressed += OnQuitMainMenuButtonPressed;
    QuitToDesktopButton.Pressed += OnQuitToDesktopButtonPressed;
  }

  public void OnExitTree()
  {
    ResumeButton.Pressed -= OnResumeButtonPressed;
    QuitToMainMenuButton.Pressed -= OnQuitMainMenuButtonPressed;
    QuitToDesktopButton.Pressed -= OnQuitToDesktopButtonPressed;

    Logic.Stop();
    Binding.Dispose();
  }
  #endregion



  #region Input Callbacks
  private void OnResumeButtonPressed() => Logic.Input(new PauseMenuLogic.Input.Resume());

  private void OnQuitMainMenuButtonPressed() =>
    Logic.Input(new PauseMenuLogic.Input.QuitToMainMenu());

  private void OnQuitToDesktopButtonPressed() =>
    Logic.Input(new PauseMenuLogic.Input.QuitToDesktop());

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

  public static class Input
  {
    public record struct Resume;

    public record struct OpenOptions;

    public record struct QuitToMainMenu;

    public record struct QuitToDesktop;
  }

  public static class Output
  {
    public partial record struct VisibilityChanged(bool Visible);
  }

  public partial record State
    : StateLogic<State>,
      IGet<Input.QuitToMainMenu>,
      IGet<Input.Resume>,
      IGet<Input.QuitToDesktop>
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

    public Transition On(in Input.QuitToMainMenu input)
    {
      Get<IAppRepo>().RequestMainMenu();
      return ToSelf();
    }

    public Transition On(in Input.Resume input)
    {
      Get<IGameRepo>().Resume();
      return ToSelf();
    }

    public Transition On(in Input.QuitToDesktop input)
    {
      Get<IAppRepo>().QuitApp();
      return ToSelf();
    }

    private void OnGameIsPaused(bool paused) => Output(new Output.VisibilityChanged(paused));
  }
}

namespace Shellguard;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IAppLogic : ILogicBlock<AppLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.State>, IAppLogic
{
  public override Transition GetInitialState() => To<State.InMainMenu>();

  public static class Input
  {
    public record struct NewGame;

    public record struct LoadGame;

    public record struct BackToMainMenu;

    public record struct QuitGame;

    public record struct QuitApp;

    public record struct Blackout(EPostBlackoutAction Action);

    public record struct BlackoutFinished;

    public record struct GameReady;

    public record struct ScanForGameFile;
  }

  public static class Output
  {
    public record struct SetupGame;

    public record struct ShowGame;

    public record struct HideGame;

    public record struct RemoveGame;

    public record struct ShowMainMenu;

    public record struct HideMainMenu;

    public record struct CloseApplication;

    public record struct FadeIn;

    public record struct Blackout;
  }
}

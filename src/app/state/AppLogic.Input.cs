namespace Shellguard;

public partial class AppLogic
{
  public static class Input
  {
    public record struct NewGame;

    public record struct LoadGame;

    public record struct BackToMainMenu;

    public record struct QuitGame;

    public record struct QuitApp;

    public record struct FadeOutFinished;

    public record struct GameReady;

    public record struct ScanForGameFile;
  }
}

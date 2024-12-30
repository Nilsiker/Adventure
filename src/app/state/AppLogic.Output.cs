namespace Shellguard;

public partial class AppLogic
{
  public static class Output
  {
    public record struct SetupGame;

    public record struct ShowGame;

    public record struct LoadGame;

    public record struct HideGame;

    public record struct RemoveGame;

    public record struct ShowMainMenu;

    public record struct HideMainMenu;

    public record struct CloseApplication;

    public record struct FadeIn;

    public record struct FadeOut;
  }
}

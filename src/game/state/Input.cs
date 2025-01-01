namespace Shellguard.Game.State;

using Shellguard.Game.Domain;

public partial class GameLogic
{
  public static class Input
  {
    public readonly record struct Initialize(int NumEggsInWorld);

    public readonly record struct StartGame;

    public readonly record struct EndGame(EGameOverReason Reason);

    public readonly record struct PauseButtonPressed;

    public readonly record struct PauseMenuTransitioned;

    public readonly record struct RequestSave;

    public readonly record struct SaveCompleted;

    public readonly record struct RequestLoad;

    public readonly record struct LoadCompleted;

    public readonly record struct GoToStartMenu;
  }
}

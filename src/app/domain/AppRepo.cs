namespace Shellguard;

using System;

public enum EPostBlackoutAction
{
  StartNewGame,
  LoadExistingGame,
  GoToMainMenu,
  QuitApp,
}

public interface IAppRepo : IDisposable
{
  event Action? GameStarted;
  event Action? MainMenuRequested;
  event Action? AppQuit;

  public EPostBlackoutAction PostBlackoutAction { get; set; }

  void StartGame();
  void RequestMainMenu();
  void QuitApp();
}

public partial class AppRepo : IAppRepo
{
  public EPostBlackoutAction PostBlackoutAction { get; set; }

  public event Action? GameStarted;
  public event Action? MainMenuRequested;
  public event Action? AppQuit;

  public void StartGame() => GameStarted?.Invoke();

  public void RequestMainMenu() => MainMenuRequested?.Invoke();

  public void QuitApp() => AppQuit?.Invoke();

  public void Dispose(bool disposing)
  {
    GameStarted = null;
    MainMenuRequested = null;
    AppQuit = null;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}

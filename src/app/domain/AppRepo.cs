namespace Shellguard;

using System;
using Chickensoft.Collections;

public interface IAppRepo : IDisposable
{
  IAutoProp<bool> HasExistingGame { get; }
  event Action? GameStartRequested;
  event Action? GameStarted;
  event Action? GameLoadRequested;
  event Action? GameLoaded;
  event Action? MainMenuRequested;
  event Action? AppQuitRequested;

  void ScanForGameFile();
  void RequestMainMenu();
  void RequestGameStart();
  void OnGameStarted();
  void RequestQuitApp();
}

public partial class AppRepo : IAppRepo
{
  public IAutoProp<bool> HasExistingGame => _hasExistingGame;
  private readonly AutoProp<bool> _hasExistingGame;

  public event Action? MainMenuRequested;
  public event Action? AppQuitRequested;
  public event Action? GameStartRequested;
  public event Action? GameStarted;
  public event Action? GameLoadRequested;
  public event Action? GameLoaded;

  public AppRepo()
  {
    _hasExistingGame = new AutoProp<bool>(false);
  }

  public void ScanForGameFile() => _hasExistingGame.OnNext(false);

  public void RequestMainMenu() => MainMenuRequested?.Invoke();

  public void RequestGameStart() => GameStartRequested?.Invoke();

  public void OnGameStarted() => GameStarted?.Invoke();

  public void RequestQuitApp() => AppQuitRequested?.Invoke();

  public void Dispose(bool disposing)
  {
    _hasExistingGame.OnCompleted();
    _hasExistingGame.Dispose();

    MainMenuRequested = null;
    GameStartRequested = null;
    GameStarted = null;
    GameLoadRequested = null;
    GameLoaded = null;
    AppQuitRequested = null;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}

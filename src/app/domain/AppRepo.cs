namespace Shellguard;

using System;
using System.IO;
using Chickensoft.Collections;
using Godot;

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

  EPostBlackoutAction PostBlackoutAction { get; set; }
  public IAutoProp<bool> HasExistingGame { get; }
  void ScanForGameFile();
  void StartGame();
  void RequestMainMenu();
  void QuitApp();
}

public partial class AppRepo : IAppRepo
{
  public static string SAVE_FILE_PATH => Path.Join(OS.GetUserDataDir(), "game.json");

  public IAutoProp<bool> HasExistingGame => _hasExistingGame;
  private readonly AutoProp<bool> _hasExistingGame;

  public EPostBlackoutAction PostBlackoutAction { get; set; }

  public event Action? GameStarted;
  public event Action? MainMenuRequested;
  public event Action? AppQuit;

  public AppRepo()
  {
    _hasExistingGame = new AutoProp<bool>(false);
  }

  public void StartGame() => GameStarted?.Invoke();

  public void RequestMainMenu() => MainMenuRequested?.Invoke();

  public void QuitApp() => AppQuit?.Invoke();

  public void ScanForGameFile() => _hasExistingGame.OnNext(File.Exists(SAVE_FILE_PATH));

  public void Dispose(bool disposing)
  {
    _hasExistingGame.OnCompleted();
    _hasExistingGame.Dispose();

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

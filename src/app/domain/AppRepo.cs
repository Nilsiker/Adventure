namespace Shellguard;

using System;
using System.IO;
using Chickensoft.Collections;
using Godot;

public interface IAppRepo : IDisposable
{
  IAutoProp<bool> HasExistingGame { get; }
  event Action? StartGameRequested;
  event Action? LoadGameRequested;
  event Action? MainMenuRequested;
  event Action? AppQuitRequested;

  void ScanForGameFile();
  void RequestMainMenu();
  void RequestStartGame();
  void RequestLoadGame();
  void RequestQuitApp();
}

public partial class AppRepo : IAppRepo
{
  public static string SAVE_FILE_PATH => Path.Join(OS.GetUserDataDir(), "game.json");

  public IAutoProp<bool> HasExistingGame => _hasExistingGame;
  private readonly AutoProp<bool> _hasExistingGame;

  public event Action? MainMenuRequested;
  public event Action? AppQuitRequested;
  public event Action? StartGameRequested;
  public event Action? LoadGameRequested;

  public AppRepo()
  {
    _hasExistingGame = new AutoProp<bool>(false);
  }

  public void ScanForGameFile() => _hasExistingGame.OnNext(File.Exists(SAVE_FILE_PATH));

  public void RequestMainMenu() => MainMenuRequested?.Invoke();

  public void RequestStartGame() => StartGameRequested?.Invoke();

  public void RequestLoadGame() => LoadGameRequested?.Invoke();

  public void RequestQuitApp() => AppQuitRequested?.Invoke();

  public void Dispose(bool disposing)
  {
    _hasExistingGame.OnCompleted();
    _hasExistingGame.Dispose();

    MainMenuRequested = null;
    AppQuitRequested = null;
    StartGameRequested = null;
    LoadGameRequested = null;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}

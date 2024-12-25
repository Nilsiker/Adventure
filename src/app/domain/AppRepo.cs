namespace Shellguard;

using System;

public interface IAppRepo : IDisposable
{
  event Action? GameStarted;
  event Action? AppQuit;

  void StartGame();
  void QuitApp();
}

public partial class AppRepo : IAppRepo
{
  public event Action? GameStarted;
  public event Action? AppQuit;

  public void QuitApp() => AppQuit?.Invoke();

  public void StartGame() => GameStarted?.Invoke();

  public void Dispose(bool disposing) { }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}

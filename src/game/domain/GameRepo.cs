namespace Shellguard.Game.Domain;

using System;
using Chickensoft.Collections;
using Shellguard.Save;

public interface IGameRepo : IDisposable
{
  IAutoProp<int> EggsCollected { get; }
  IAutoProp<bool> IsPaused { get; }

  event Action? Saving;
  event Action? Saved;
  event Action? Loading;
  event Action? Loaded;
  void OnEggCollected();

  void Pause();
  void Resume();
  void RequestSave();
  void RequestLoad();
}

public class GameRepo(IGameFileService gameFileService) : IGameRepo
{
  public IAutoProp<int> EggsCollected => _eggsCollected;

  public IAutoProp<bool> IsPaused => _isPaused;

  private readonly AutoProp<int> _eggsCollected = new(0);
  private readonly AutoProp<bool> _isPaused = new(false);
  private readonly IGameFileService _gameFileService = gameFileService;

  public event Action? Saving;
  public event Action? Saved;
  public event Action? Loading;
  public event Action? Loaded;

  public void Pause() => _isPaused.OnNext(true);

  public void Resume() => _isPaused.OnNext(false);

  public void Dispose(bool disposing)
  {
    _eggsCollected.OnCompleted();
    _eggsCollected.Dispose();

    _isPaused.OnCompleted();
    _isPaused.Dispose();

    Saving = null;
    Saved = null;
    Loading = null;
    Loaded = null;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public void OnEggCollected() => _eggsCollected.OnNext(_eggsCollected.Value + 1);

  public void RequestSave()
  {
    Saving?.Invoke();
    _gameFileService.Save().ContinueWith((_) => Saved?.Invoke());
  }

  public void RequestLoad()
  {
    Loading?.Invoke();
    _gameFileService.Load().ContinueWith((_) => Loaded?.Invoke());
  }
}

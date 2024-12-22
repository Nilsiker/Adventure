namespace Shellguard.Game.Domain;

using System;
using Chickensoft.Collections;

public interface IGameRepo : IDisposable
{
  event Action<EGameOverReason>? Ended;

  IAutoProp<int> EggsCollected { get; }
  IAutoProp<bool> IsPaused { get; }

  void OnGameEnded(EGameOverReason gameOverReason);
  void OnEggCollected();

  void Pause();
  void Resume();
}

public class GameRepo : IGameRepo
{
  public IAutoProp<int> EggsCollected => _eggsCollected;

  public IAutoProp<bool> IsPaused => _isPaused;

  public event Action<EGameOverReason>? Ended;

  private readonly AutoProp<int> _eggsCollected;
  private readonly AutoProp<bool> _isPaused;

  public GameRepo()
  {
    _eggsCollected = new AutoProp<int>(0);
    _isPaused = new AutoProp<bool>(false);
  }

  public void OnGameEnded(EGameOverReason gameOverReason) => Ended?.Invoke(gameOverReason);

  public void Pause() => _isPaused.OnNext(true);

  public void Resume() => _isPaused.OnNext(false);

  public void Dispose(bool disposing)
  {
    _eggsCollected.OnCompleted();
    _eggsCollected.Dispose();

    _isPaused.OnCompleted();
    _isPaused.Dispose();
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public void OnEggCollected() => _eggsCollected.OnNext(_eggsCollected.Value + 1);
}

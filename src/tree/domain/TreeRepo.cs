namespace Shellguard.Tree;

using System;

public enum EStage
{
  Seedling,
  Sapling,
  Young,
  Mature,
  YoungStump,
  MatureStump,
}

public interface ITreeRepo : IDisposable
{
  event Action? TreeAdded;
  event Action? TreeRemoved;

  void OnTreeAdded();
  void OnTreeRemoved();
}

public class TreeRepo : ITreeRepo
{
  public event Action? TreeAdded;
  public event Action? TreeRemoved;

  public void OnTreeAdded() => TreeAdded?.Invoke();

  public void OnTreeRemoved() => TreeRemoved?.Invoke();

  public void Dispose()
  {
    TreeAdded = null;
    TreeRemoved = null;

    GC.SuppressFinalize(this);
  }
}

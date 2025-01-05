namespace Shellguard;

using System;
using System.Collections.Generic;
using System.Linq;
using Chickensoft.Collections;
using Godot;

public interface IWorldRepo : IDisposable
{
  event Action<Vector2I> TreePlanted;
  IAutoProp<HashSet<Vector2I>> OccupiedTiles { get; }

  void FreeTile(Vector2I tile);
  void OccupyTile(Vector2I tile);
  bool IsTileFree(Vector2I tile);
  void PlantTree(Vector2I tile);
}

public partial class WorldRepo() : IWorldRepo
{
  public event Action<Vector2I>? TreePlanted;

  public IAutoProp<HashSet<Vector2I>> OccupiedTiles => _occupiedTiles;

  private readonly AutoProp<HashSet<Vector2I>> _occupiedTiles = new([]);

  public void Dispose(bool disposing)
  {
    _occupiedTiles.OnCompleted();
    _occupiedTiles.Dispose();

    TreePlanted = null;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public void FreeTile(Vector2I tile) =>
    _occupiedTiles.OnNext([.. _occupiedTiles.Value.Without(tile)]);

  public bool IsTileFree(Vector2I tile) => !_occupiedTiles.Value.Contains(tile);

  public void OccupyTile(Vector2I tile) =>
    _occupiedTiles.OnNext([.. _occupiedTiles.Value.Append(tile)]);

  public void PlantTree(Vector2I tile) => TreePlanted?.Invoke(tile);
}

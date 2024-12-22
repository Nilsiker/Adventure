namespace Shellguard.Egg;

using Chickensoft.GodotNodeInterfaces;
using Godot;

public interface IEggCollector : IArea2D
{
  bool CanCollectEggs { get; }
}

public partial class EggCollector : Area2D, IEggCollector
{
  public bool CanCollectEggs => true;
}

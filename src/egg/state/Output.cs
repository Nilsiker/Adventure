namespace Shellguard.Egg.State;

using Godot;

public partial class EggLogic
{
  public static class Output
  {
    public partial record struct OffsetChanged(Vector2 Offset);

    public partial record struct SelfDestruct;
  }
}

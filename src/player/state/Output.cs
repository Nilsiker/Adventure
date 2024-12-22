namespace Shellguard.Player.State;

using Godot;

public partial class PlayerLogic
{
  public static class Output
  {
    public readonly record struct Movement(Vector2 Velocity);
  }
}

namespace Shellguard.Player.State;

using Godot;

public partial class PlayerLogic
{
  public static class Input
  {
    public readonly record struct Move(Vector2 Direction);
  }
}

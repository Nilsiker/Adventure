namespace Shellguard.Player.State;

using Godot;

public partial class PlayerLogic
{
  public enum FaceDirection
  {
    Left,
    Right,
  }

  public static class Output
  {
    public readonly record struct FaceDirectionChanged(FaceDirection FaceDirection);

    public readonly record struct Interact();

    public readonly record struct Attack(Vector2 Direction);

    public readonly record struct Movement(Vector2 Velocity);

    public readonly record struct AnimationChanged(string Animation);
  }
}

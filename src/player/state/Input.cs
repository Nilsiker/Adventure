namespace Shellguard.Player.State;

using Godot;

public partial class PlayerLogic
{
  public static class Input
  {
    public readonly record struct Enable;

    public readonly record struct Disable;

    public readonly record struct MoveInput(Vector2 Direction);

    public readonly record struct AttackInput();

    public readonly record struct InteractInput();

    public readonly record struct AnimationFinished(StringName Animation);
  }
}

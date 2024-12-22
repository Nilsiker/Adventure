namespace Shellguard.Player;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IPlayer : ICharacterBody2D { }

[Meta(typeof(IAutoNode))]
public partial class Player : CharacterBody2D, IPlayer
{
  public override void _Notification(int what) => this.Notify(what);

  public const float SPEED = 100.0f;

  public override void _PhysicsProcess(double delta)
  {
    var velocity = Velocity;

    // Get the input direction and handle the movement/deceleration.
    // As good practice, you should replace UI actions with custom gameplay actions.
    var direction = Input.GetVector("left", "right", "up", "down");
    velocity =
      direction != Vector2.Zero ? direction * SPEED : Velocity.MoveToward(Vector2.Zero, SPEED);

    Velocity = velocity;
    MoveAndSlide();
  }
}

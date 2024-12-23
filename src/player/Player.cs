namespace Shellguard.Player;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Player.State;
using Shellguard.Weapon;

public interface IPlayer : ICharacterBody2D { }

[Meta(typeof(IAutoNode))]
public partial class Player : CharacterBody2D, IPlayer
{
  #region State
  private PlayerLogic Logic { get; set; } = default!;
  private PlayerLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
  [Node("%Weapon")]
  private Weapon Weapon { get; set; } = default!;
  #endregion
  public override void _Notification(int what) => this.Notify(what);

  public const float SPEED = 100.0f;

  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();

    Binding.Handle((in PlayerLogic.Output.Movement output) => OnMovementOutput(output));

    Logic.Start();
  }

  public override void _PhysicsProcess(double delta)
  {
    var velocity = Velocity;

    // Get the input direction and handle the movement/deceleration.
    // As good practice, you should replace UI actions with custom gameplay actions.
    var direction = Input.GetVector("left", "right", "up", "down");
    velocity =
      direction != Vector2.Zero ? direction * SPEED : Velocity.MoveToward(Vector2.Zero, SPEED);

    Logic.Input(new PlayerLogic.Input.Move(velocity));

    // Aim weapon
    Weapon.Aim(GetGlobalMousePosition());
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event.IsActionPressed("attack"))
    {
      Weapon.Attack();
    }
  }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }

  private void OnMovementOutput(PlayerLogic.Output.Movement output)
  {
    Velocity = output.Velocity;
    GD.Print("hehe");
    MoveAndSlide();
  }
}

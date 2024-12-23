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

  [Node("StateLabel")]
  Label StateLabel { get; set; } = default!;

  [Node("PlayerModel/AnimationPlayer")]
  AnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion
  public override void _Notification(int what) => this.Notify(what);

  [Export]
  private float _speed = 100.0f;

  [Export]
  private PackedScene _bonk;

  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();
    Binding.When<PlayerLogic.State>(state => StateLabel.Text = state.GetType().Name);
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
      direction != Vector2.Zero ? direction * _speed : Velocity.MoveToward(Vector2.Zero, _speed);

    Logic.Input(new PlayerLogic.Input.Move(velocity));

    // Aim weapon
    Weapon.Aim(GetGlobalMousePosition());
    if (AnimationPlayer.CurrentAnimation == "hammer")
    {
      return;
    }

    if (direction.IsZeroApprox())
    {
      AnimationPlayer.Play($"idle_s");
    }
    else
    {
      AnimationPlayer.Play($"run_s");
      if (direction.X != 0)
      {
        GetNode<Sprite2D>("PlayerModel/Sprite2D").FlipH = direction.X > 0;
      }
    }

    Weapon.Aim(GetGlobalMousePosition());
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event.IsActionPressed("attack"))
    {
      Weapon.Attack();
      AnimationPlayer.Play("hammer");

      GetNode<Sprite2D>("PlayerModel/Sprite2D").FlipH =
        GetNode<Node2D>("Weapon/Sprite2D").GlobalPosition > GlobalPosition;
    }
  }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }

  private void OnMovementOutput(PlayerLogic.Output.Movement output)
  {
    if (AnimationPlayer.CurrentAnimation == "hammer")
    {
      return;
    }
    Velocity = output.Velocity;
    MoveAndSlide();
  }
}

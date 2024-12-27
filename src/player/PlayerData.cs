namespace Shellguard.Player;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;
using Shellguard.Player.State;

[Meta, Id("player_data")]
public partial record class PlayerData
{
  [Save("global_transform")]
  public required Transform2D GlobalTransform { get; init; }

  [Save("state_machine")]
  public required PlayerLogic StateMachine { get; init; }

  [Save("velocity")]
  public required Vector2 Velocity { get; init; }
}

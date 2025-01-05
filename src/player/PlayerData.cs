namespace Shellguard.Player;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;

[Meta, Id("player_data")]
public partial record PlayerData
{
  [Save("global_transform")]
  public required Transform2D GlobalTransform { get; set; }
}

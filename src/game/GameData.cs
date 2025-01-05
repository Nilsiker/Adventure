namespace Shellguard.Game;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Shellguard.Player;
using Shellguard.Save;

[Meta, Id("game_data")]
public partial record GameData
{
  [Save("player_data")]
  public required PlayerData PlayerData { get; init; }

  [Save("world_data")]
  public required WorldData WorldData { get; init; }
}

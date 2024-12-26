namespace Shellguard.Game;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Shellguard.Player;

[Meta, Id("game_data")]
public partial record GameData
{
  [Save("wood")]
  public required int Wood { get; init; }

  [Save("player_data")]
  public required PlayerData PlayerData { get; init; }
}

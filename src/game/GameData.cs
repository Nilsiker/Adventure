namespace Shellguard.Game;

using Chickensoft.Introspection;
using Chickensoft.Serialization;

[Meta, Id("game_data")]
public partial record GameData
{
  [Save("egg_collected")]
  public required int EggsCollected { get; init; }
}

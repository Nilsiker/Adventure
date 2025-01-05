namespace Shellguard.Game;

using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Shellguard.Player;
using Shellguard.Tree;

[Meta, Id("game_data")]
public partial record GameData
{
  [Save("player_data")]
  public required PlayerData PlayerData { get; init; }

  [Save("entity_table")]
  public required Dictionary<string, TreeData> TreeDictionary { get; init; }
}

namespace Shellguard.SaveData.App;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Shellguard.Game;

[Meta, Id("app_data")]
public partial record AppData
{
  [Save("game_data")]
  public required GameData GameData { get; init; }
}

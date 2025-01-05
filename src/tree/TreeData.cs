namespace Shellguard.Tree;

using Chickensoft.Introspection;
using Chickensoft.Serialization;

[Meta, Id("tree_data")]
public partial record TreeData
{
  [Save("health")]
  public required float Health { get; set; }

  [Save("age")]
  public required float Age { get; set; }

  [Save("time_to_mature")]
  public required float TimeToMature { get; set; }
}

namespace Shellguard.Save;

using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Shellguard.Tree;

[Meta, Id("world_data")]
public partial record WorldData
{
  [Save("trees")]
  public required List<TreeData> Trees { get; init; }
}

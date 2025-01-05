namespace Shellguard.Tree;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;

[Meta, Id("tree_data")]
public partial record TreeData
{
  [Save("global_transform")]
  public required Transform2D GlobalTransform { get; init; }

  [Save("state_machine")]
  public required ITreeLogic StateMachine { get; init; }
}

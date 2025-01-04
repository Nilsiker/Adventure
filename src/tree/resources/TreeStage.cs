namespace Shellguard.Tree;

using Godot;

public interface ITreeStage
{
  float Health { get; }
  float TimeToMature { get; }
}

[Tool, GlobalClass]
public partial class TreeStage : Resource, ITreeStage
{
  [Export]
  public float Health { get; set; }

  [Export]
  public float TimeToMature { get; set; }
}

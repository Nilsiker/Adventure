namespace Shellguard.Tree;

using Godot;

public interface ITreeSettings
{
  float YoungHealth { get; }
  float MatureHealth { get; }
  float SeedlingTimeToMature { get; }
  float SaplingTimeToMature { get; }
  float YoungTimeToMature { get; }
}

[Tool, GlobalClass]
public partial class TreeSettings : Resource, ITreeSettings
{
  [Export]
  public float YoungHealth { get; set; }

  [Export]
  public float MatureHealth { get; set; }

  [Export]
  public float SeedlingTimeToMature { get; set; }

  [Export]
  public float SaplingTimeToMature { get; set; }

  [Export]
  public float YoungTimeToMature { get; set; }
}

namespace Shellguard.Tree;

using System.Collections.Generic;
using Godot;
using Godot.Collections;

public interface ITreeSettings
{
  IEnumerable<ITreeStage> GetStages();
}

[Tool, GlobalClass]
public partial class TreeSettings : Resource, ITreeSettings
{
  [Export]
  public Array<TreeStage> Stages { get; set; } = default!;

  IEnumerable<ITreeStage> ITreeSettings.GetStages() => Stages;
}

namespace Shellguard;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface ITreeLogic : ILogicBlock<TreeLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class TreeLogic : LogicBlock<TreeLogic.State>, ITreeLogic
{
  public override Transition GetInitialState() => To<State>();

  public struct Data
  {
    public float Health;
    public int Age;
  }
}

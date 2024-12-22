namespace Shellguard.Egg.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IEggLogic : ILogicBlock<EggLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class EggLogic : LogicBlock<EggLogic.State>, IEggLogic
{
  public override Transition GetInitialState() => To<State.Idle>();

  public partial class Data
  {
    public double Elapsed { get; set; }
  }
}

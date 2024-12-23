namespace Shellguard.Stats;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IStatsLogic : ILogicBlock<StatsLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class StatsLogic : LogicBlock<StatsLogic.State>, IStatsLogic
{
  public override Transition GetInitialState() => To<State>();

  public partial record State : StateLogic<State> { }
}

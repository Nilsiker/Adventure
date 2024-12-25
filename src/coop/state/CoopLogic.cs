namespace Shellguard.Coop;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface ICoopLogic : ILogicBlock<CoopLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class CoopLogic : LogicBlock<CoopLogic.State>, ICoopLogic
{
  public override Transition GetInitialState() => To<State>();

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}

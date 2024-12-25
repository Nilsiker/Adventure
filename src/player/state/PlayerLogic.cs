namespace Shellguard.Player.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IPlayerLogic : ILogicBlock<PlayerLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class PlayerLogic : LogicBlock<PlayerLogic.State>, IPlayerLogic
{
  public partial class Data(float speed)
  {
    public float Speed { get; set; } = speed;
  }

  public override Transition GetInitialState() => To<State.Idle>();
}

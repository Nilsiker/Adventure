namespace Shellguard.Player.State;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public partial record Idle : State, IGet<Input.MoveInput>, IGet<Input.AttackInput>
    {
      public Idle()
        : base()
      {
        this.OnEnter(() => Output(new Output.AnimationChanged("idle_s")));
      }

      public Transition On(in Input.MoveInput input) =>
        input.Direction.IsZeroApprox() ? ToSelf() : To<Moving>();

      public Transition On(in Input.AttackInput input) => To<Attacking>();
    }
  }
}

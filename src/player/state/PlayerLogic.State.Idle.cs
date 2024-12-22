namespace Shellguard.Player.State;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public partial record Idle : State, IGet<Input.Move>
    {
      public Transition On(in Input.Move input) =>
        input.Direction.IsZeroApprox() ? ToSelf() : To<Moving>();
    }
  }
}

namespace Shellguard.Player.State;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public partial record Disabled : State, IGet<Input.Enable>
    {
      public Transition On(in Input.Enable _) => To<Idle>();
    }
  }
}

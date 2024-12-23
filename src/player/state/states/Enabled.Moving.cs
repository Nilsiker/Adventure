namespace Shellguard.Player.State;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public partial record Moving : State, IGet<Input.Move>
    {
      public Transition On(in Input.Move input)
      {
        if (input.Direction.IsZeroApprox())
        {
          return To<Idle>();
        }

        Output(new Output.Movement(input.Direction));
        return ToSelf();
      }
    }
  }
}

namespace Shellguard.Game.State;

public partial class GameLogic
{
  public abstract partial record State
  {
    public partial record Playing : State, IGet<Input.PauseButtonPressed>
    {
      public Playing() { }

      public Transition On(in Input.PauseButtonPressed input) => To<Paused>();
    }
  }
}

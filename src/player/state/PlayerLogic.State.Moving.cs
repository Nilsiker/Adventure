namespace Shellguard.Player.State;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public partial record Moving : State { }
  }
}

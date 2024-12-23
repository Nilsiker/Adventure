namespace Shellguard.Player.State;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public record Enabled : State { }
  }
}

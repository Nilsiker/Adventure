namespace Shellguard.Player.State;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}

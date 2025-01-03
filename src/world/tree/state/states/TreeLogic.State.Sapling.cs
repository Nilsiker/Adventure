namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Sapling : StateLogic<State>
    {
      public Sapling()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }
    }
  }
}

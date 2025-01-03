namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature : StateLogic<State>
    {
      public Mature()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }
    }
  }
}

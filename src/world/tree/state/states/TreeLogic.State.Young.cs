namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Young : StateLogic<State>
    {
      public Young()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }
    }
  }
}

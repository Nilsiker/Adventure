namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Seedling : StateLogic<State>
    {
      public Seedling()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }
    }
  }
}

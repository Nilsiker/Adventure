namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Young
    {
      public partial record Stump : StateLogic<State>
      {
        public Stump()
        {
          OnAttach(() => { });
          OnDetach(() => { });
        }
      }
    }
  }
}

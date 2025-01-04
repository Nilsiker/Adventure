namespace Shellguard.Tree;

using System.Linq;
using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature
    {
      public partial record Stump : Mature, IGet<Input.ChopDown>
      {
        public Stump()
          : base()
        {
          OnAttach(() => { });
          OnDetach(() => { });
        }

        public override Transition On(in Input.ChopDown input)
        {
          Output(new Output.Destroy());
          return ToSelf();
        }
      }
    }
  }
}

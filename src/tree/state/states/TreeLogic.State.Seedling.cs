namespace Shellguard.Tree;

using System.Linq;
using Chickensoft.LogicBlocks;
using Godot;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Seedling : State, IGet<Input.IncreaseMaturity>, IGet<Input.ChopDown>
    {
      protected override int Stage => 0;

      public Seedling()
        : base()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      public virtual Transition On(in Input.IncreaseMaturity input) => To<Sapling>();

      public Transition On(in Input.ChopDown input)
      {
        Output(new Output.Destroy());
        return ToSelf();
      }
    }
  }
}

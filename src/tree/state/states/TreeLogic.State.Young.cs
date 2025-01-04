namespace Shellguard.Tree;

using System.Linq;
using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Young
      : State,
        IGet<Input.IncreaseMaturity>,
        IGet<Input.ChopDown>,
        IGet<Input.OccludingEntity>
    {
      protected override int Stage => 2;

      public Young()
        : base()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      public Transition On(in Input.IncreaseMaturity input) => To<Mature>();

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();

      public Transition On(in Input.OccludingEntity input)
      {
        Output(new Output.UpdateTransparency(input.Occluding ? 0.5f : 1.0f));
        return ToSelf();
      }
    }
  }
}

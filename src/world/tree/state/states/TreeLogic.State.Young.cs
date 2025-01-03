namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Young : State, IGet<Input.IncreaseMaturity>, IGet<Input.ChopDown>
    {
      public Young()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      protected override EStage Stage => EStage.Young;

      public Transition On(in Input.IncreaseMaturity input) => To<Mature>();

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();
    }
  }
}

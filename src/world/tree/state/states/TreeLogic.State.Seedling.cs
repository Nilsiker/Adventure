namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Seedling : State, IGet<Input.IncreaseMaturity>, IGet<Input.ChopDown>
    {
      public Seedling()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      protected override EStage Stage => EStage.Seedling;

      public virtual Transition On(in Input.IncreaseMaturity input) => To<Sapling>();

      public Transition On(in Input.ChopDown input)
      {
        Output(new Output.Destroy());
        return ToSelf();
      }
    }
  }
}

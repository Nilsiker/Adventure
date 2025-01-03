namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Sapling : State, IGet<Input.ChopDown>, IGet<Input.IncreaseMaturity>
    {
      public Sapling()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      protected override EStage Stage => EStage.Sapling;

      public Transition On(in Input.ChopDown input)
      {
        Output(new Output.Destroy());
        return ToSelf();
      }

      public Transition On(in Input.IncreaseMaturity input) => To<Young>();
    }
  }
}

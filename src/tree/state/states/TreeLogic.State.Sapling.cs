namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Sapling : State, IGet<Input.ChopDown>, IGet<Input.IncreaseMaturity>
    {
      protected override int Stage => 1;

      public Sapling()
        : base()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      public Transition On(in Input.ChopDown input)
      {
        Output(new Output.Destroy());
        return ToSelf();
      }

      public Transition On(in Input.IncreaseMaturity input) => To<Young>();
    }
  }
}

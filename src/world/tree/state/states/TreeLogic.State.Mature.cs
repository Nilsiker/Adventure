namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature : State, IGet<Input.ChopDown>
    {
      public Mature()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      protected override EStage Stage => EStage.Mature;

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();
    }
  }
}

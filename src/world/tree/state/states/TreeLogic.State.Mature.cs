namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature : State, IGet<Input.ChopDown>
    {
      protected override EStage Stage => EStage.Mature;
      protected override float Health => Get<ITreeSettings>().MatureHealth;
      protected override float TimeToMature => 0;

      public Mature()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();
    }
  }
}

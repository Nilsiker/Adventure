namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Young : State, IGet<Input.IncreaseMaturity>, IGet<Input.ChopDown>
    {
      protected override EStage Stage => EStage.Young;
      protected override float Health => Get<ITreeSettings>().YoungHealth;
      protected override float TimeToMature => Get<ITreeSettings>().YoungTimeToMature;

      public Young()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      public Transition On(in Input.IncreaseMaturity input) => To<Mature>();

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();
    }
  }
}

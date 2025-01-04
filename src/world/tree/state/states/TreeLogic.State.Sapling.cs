namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

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

        this.OnEnter(() => Output(new Output.StageUpdated(EStage.Sapling)));
      }

      protected override EStage Stage => EStage.Sapling;

      protected override float Health => 1;

      protected override float TimeToMature => Get<ITreeSettings>().SaplingTimeToMature;

      public Transition On(in Input.ChopDown input)
      {
        Output(new Output.Destroy());
        return ToSelf();
      }

      public Transition On(in Input.IncreaseMaturity input) => To<Young>();
    }
  }
}

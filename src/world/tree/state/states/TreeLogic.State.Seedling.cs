namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Seedling : State, IGet<Input.IncreaseMaturity>, IGet<Input.ChopDown>
    {
      protected override EStage Stage => EStage.Seedling;
      protected override float Health => 1;
      protected override float TimeToMature => Get<ITreeSettings>().SeedlingTimeToMature;

      public Seedling()
      {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.StageUpdated(EStage.Seedling)));
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

namespace Shellguard.Tree;

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
      protected override EStage Stage => EStage.Young;
      protected override float Health => Get<ITreeSettings>().YoungHealth;
      protected override float TimeToMature => Get<ITreeSettings>().YoungTimeToMature;

      public Young()
      {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.StageUpdated(EStage.Young)));
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

namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature : State, IGet<Input.ChopDown>, IGet<Input.OccludingEntity>
    {
      protected override EStage Stage => EStage.Mature;
      protected override float Health => Get<ITreeSettings>().MatureHealth;
      protected override float TimeToMature => 0;

      public Mature()
      {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.StageUpdated(EStage.Mature)));
      }

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();

      public Transition On(in Input.OccludingEntity input)
      {
        Output(new Output.UpdateTransparency(input.Occluding ? 0.5f : 1.0f));
        return ToSelf();
      }
    }
  }
}

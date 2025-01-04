namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature
    {
      public partial record Stump : Mature, IGet<Input.ChopDown>
      {
        public Stump()
        {
          OnAttach(() => { });
          OnDetach(() => { });

          this.OnEnter(() => Output(new Output.StageUpdated(EStage.MatureStump)));
        }

        protected override EStage Stage => EStage.MatureStump;

        public override Transition On(in Input.ChopDown input)
        {
          Output(new Output.Destroy());
          return ToSelf();
        }
      }
    }
  }
}

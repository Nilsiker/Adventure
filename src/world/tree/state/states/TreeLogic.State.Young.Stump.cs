namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Young
    {
      public partial record Stump : Young, IGet<Input.ChopDown>
      {
        protected override EStage Stage => EStage.YoungStump;

        public Stump()
        {
          OnAttach(() => { });
          OnDetach(() => { });

          this.OnEnter(() => Output(new Output.StageUpdated(EStage.YoungStump)));
        }

        public override Transition On(in Input.ChopDown input)
        {
          Output(new Output.Destroy());
          return ToSelf();
        }
      }
    }
  }
}

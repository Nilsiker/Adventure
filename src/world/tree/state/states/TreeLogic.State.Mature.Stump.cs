namespace Shellguard.Tree;

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

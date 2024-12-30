namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record LeavingMenu : State, IGet<Input.FadeOutFinished>
    {
      public LeavingMenu()
      {
        this.OnEnter(() => Output(new Output.FadeOut()));
        this.OnExit(() =>
        {
          Output(new Output.HideMainMenu());
          Output(new Output.FadeIn());
        });
      }

      public Transition On(in Input.FadeOutFinished input) =>
        Get<Data>().ShouldLoadGame ? To<LoadingGame>() : To<StartingNewGame>();
    }
  }
}

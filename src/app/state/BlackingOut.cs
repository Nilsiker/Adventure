namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public record BlackingOut : State, IGet<Input.BlackoutFinished>
    {
      public BlackingOut()
      {
        OnAttach(() => Output(new Output.Blackout()));
        OnDetach(() => { });
      }

      public Transition On(in Input.BlackoutFinished input) =>
        Get<IAppRepo>().PostBlackoutAction switch
        {
          EPostBlackoutAction.StartNewGame => To<StartingGame>(),
          EPostBlackoutAction.LoadExistingGame => To<LoadingGame>(),
          EPostBlackoutAction.QuitApp => To<ClosingApplication>(),
          EPostBlackoutAction.GoToMainMenu => To<InMainMenu>(),
          _ => To<InMainMenu>(),
        };
    }
  }
}

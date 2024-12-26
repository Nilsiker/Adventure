namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record InMainMenu : State, IGet<Input.NewGame>
    {
      public InMainMenu()
      {
        OnAttach(() =>
        {
          Output(new Output.FadeIn());
          Output(new Output.ShowMainMenu());
          Output(new Output.RemoveGame());
        });
        OnDetach(() => Output(new Output.HideMainMenu()));
      }

      public Transition On(in Input.NewGame input)
      {
        Get<IAppRepo>().PostBlackoutAction = EPostBlackoutAction.StartNewGame;
        return To<BlackingOut>();
      }
    }
  }
}

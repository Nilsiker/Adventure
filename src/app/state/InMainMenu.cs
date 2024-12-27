namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record InMainMenu : State, IGet<Input.NewGame>, IGet<Input.QuitApp>
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

      Transition IGet<Input.NewGame>.On(in Input.NewGame input)
      {
        Get<IAppRepo>().PostBlackoutAction = EPostBlackoutAction.StartNewGame;
        return To<BlackingOut>();
      }

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input)
      {
        Get<IAppRepo>().PostBlackoutAction = EPostBlackoutAction.QuitApp;
        return To<BlackingOut>();
      }
    }
  }
}

namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record InGame : State, IGet<Input.BackToMainMenu>, IGet<Input.QuitApp>
    {
      public InGame()
      {
        OnAttach(() =>
        {
          Get<IAppRepo>().MainMenuRequested += OnMainMenuRequested;

          Output(new Output.SetupGame());
          Output(new Output.ShowGame());
          Output(new Output.HideMainMenu());
          Output(new Output.FadeIn());
        });
        OnDetach(() => Get<IAppRepo>().MainMenuRequested -= OnMainMenuRequested);
      }

      private void OnMainMenuRequested() => Input(new Input.BackToMainMenu());

      Transition IGet<Input.BackToMainMenu>.On(in Input.BackToMainMenu input)
      {
        Get<IAppRepo>().PostBlackoutAction = EPostBlackoutAction.GoToMainMenu;
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

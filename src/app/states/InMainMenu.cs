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
          Output(new Output.ShowMainMenu());
          Output(new Output.RemoveGame());
        });
        OnDetach(() => { });
      }

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();

      Transition IGet<Input.NewGame>.On(in Input.NewGame input) => To<StartingGame>();
    }
  }
}

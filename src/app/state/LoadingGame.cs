namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record LoadingGame : State, IGet<Input.GameReady>
    {
      public LoadingGame()
      {
        OnAttach(() =>
        {
          Output(new Output.SetupGame());
          Output(new Output.LoadGame());
          Input(new Input.GameReady());
        });
        OnDetach(() => { });
      }

      public Transition On(in Input.GameReady input) => To<InGame>();
    }
  }
}

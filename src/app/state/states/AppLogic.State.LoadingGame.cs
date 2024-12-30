namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record LoadingGame : State, IGet<Input.GameReady>
    {
      public LoadingGame()
      {
        OnAttach(() => Get<IAppRepo>().GameLoaded += OnAppGameLoaded);
        OnDetach(() => Get<IAppRepo>().GameLoaded -= OnAppGameLoaded);

        this.OnEnter(() =>
        {
          Output(new Output.SetupGame());
          Output(new Output.LoadGame());
        });
      }

      private void OnAppGameLoaded() => Input(new Input.GameReady());

      public Transition On(in Input.GameReady input) => To<InGame>();
    }
  }
}

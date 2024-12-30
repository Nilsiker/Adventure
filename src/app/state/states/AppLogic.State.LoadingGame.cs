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
        this.OnEnter(() =>
        {
          Output(new Output.SetupGame());
          Output(new Output.LoadGame());
        });
      }

      public Transition On(in Input.GameReady input) => To<InGame>();
    }
  }
}

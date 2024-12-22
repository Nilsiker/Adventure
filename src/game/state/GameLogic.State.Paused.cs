namespace Shellguard.Game.State;

using Chickensoft.LogicBlocks;
using Shellguard.Game.Domain;

public partial class GameLogic
{
  public abstract partial record State
  {
    public partial record Paused : State, IGet<Input.PauseButtonPressed>
    {
      public Paused()
      {
        this.OnEnter(() => Get<IGameRepo>().Pause());
        this.OnExit(() =>
        {
          Get<IGameRepo>().Resume();
          Output(new Output.ExitPauseMenu());
        });
      }

      public Transition On(in Input.PauseButtonPressed input) => To<Playing>();
    }
  }
}

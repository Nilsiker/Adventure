namespace Shellguard.Game.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Shellguard.Game.Domain;

public partial class GameLogic
{
  [Meta]
  public abstract partial record State : StateLogic<State>
  {
    protected State()
    {
      OnAttach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync += OnIsPaused;
      });

      OnDetach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync -= OnIsPaused;
      });
    }

    private void OnIsPaused(bool isPaused) => Output(new Output.SetPauseMode(isPaused));
  }
}

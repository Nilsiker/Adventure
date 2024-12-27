namespace Shellguard.Game.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Shellguard.Game.Domain;

public partial class GameLogic
{
  [Meta]
  public abstract partial record State
    : StateLogic<State>,
      IGet<Input.SaveRequested>,
      IGet<Input.SaveCompleted>,
      IGet<Input.LoadRequested>
  {
    protected State()
    {
      OnAttach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync += OnIsPaused;
        gameRepo.Saving += OnSaving;
      });

      OnDetach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync -= OnIsPaused;
        gameRepo.Saving -= OnSaving;
      });
    }

    private void OnSaving() => Output(new Output.StartSaving());

    public Transition On(in Input.SaveRequested input)
    {
      Get<IGameRepo>().RequestSave();
      return ToSelf();
    }

    public Transition On(in Input.LoadRequested input)
    {
      Get<IGameRepo>().RequestLoad();
      return ToSelf();
    }

    public Transition On(in Input.SaveCompleted input)
    {
      Get<IGameRepo>().OnSaved();
      return ToSelf();
    }

    private void OnIsPaused(bool isPaused) => Output(new Output.SetPauseMode(isPaused));
  }
}

namespace Shellguard.Game.State;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Shellguard.Game.Domain;

public partial class GameLogic
{
  [Meta]
  public abstract partial record State
    : StateLogic<State>,
      IGet<Input.RequestSave>,
      IGet<Input.RequestLoad>
  {
    protected State()
    {
      OnAttach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync += OnGameIsPaused;
        gameRepo.Saved += OnGameSaved;
      });

      OnDetach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsPaused.Sync -= OnGameIsPaused;
        gameRepo.Saved -= OnGameSaved;
      });
    }

    private void OnGameSaved() => throw new NotImplementedException();

    private void OnGameIsPaused(bool isPaused) => Output(new Output.SetPauseMode(isPaused));

    public Transition On(in Input.RequestSave input)
    {
      Get<IGameRepo>().RequestSave();
      return ToSelf();
    }

    public Transition On(in Input.RequestLoad input)
    {
      Get<IGameRepo>().RequestLoad();
      return ToSelf();
    }
  }
}

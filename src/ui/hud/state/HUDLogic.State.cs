namespace Shellguard.UI;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Shellguard.Game.Domain;

public partial class HUDLogic
{
  [Meta]
  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() =>
      {
        Get<IGameRepo>().EggsCollected.Sync += OnEggsCollectedSynced;
        Get<IGameRepo>().IsPaused.Sync += OnIsPaused;
      });
      OnDetach(() =>
      {
        Get<IGameRepo>().EggsCollected.Sync -= OnEggsCollectedSynced;
        Get<IGameRepo>().IsPaused.Sync -= OnIsPaused;
      });
    }

    private void OnIsPaused(bool paused) => Output(new Output.VisilibilityChanged(!paused));

    private void OnEggsCollectedSynced(int num) => Output(new Output.EggsCollectedChanged(num));
  }
}

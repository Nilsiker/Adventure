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
      OnAttach(() => Get<IGameRepo>().EggsCollected.Sync += OnEggsCollectedSynced);

      OnDetach(() => Get<IGameRepo>().EggsCollected.Sync -= OnEggsCollectedSynced);
    }

    private void OnEggsCollectedSynced(int num) => Output(new Output.EggsCollectedChanged(num));
  }
}

namespace Shellguard.UI;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta]
[LogicBlock(typeof(State))]
public partial class HUDLogic : LogicBlock<HUDLogic.State>
{
  public override Transition GetInitialState() => To<State>();

  public static class Output
  {
    public record struct VisilibilityChanged(bool Visible);

    public record struct EggsCollectedChanged(int Count);
  }
}

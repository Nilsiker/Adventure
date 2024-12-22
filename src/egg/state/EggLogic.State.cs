namespace Shellguard.Egg.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Shellguard.Game.Domain;

public partial class EggLogic
{
  [Meta]
  public abstract partial record State : StateLogic<State>
  {
    public partial record Idle : State, IGet<Input.Collect>
    {
      public Transition On(in Input.Collect input) => To<Collected>();
    }

    public partial record Collected : State
    {
      public Collected()
      {
        this.OnEnter(() =>
        {
          Get<IGameRepo>().OnEggCollected();
          Output(new Output.Collected());
        });
      }
    }
  }
}

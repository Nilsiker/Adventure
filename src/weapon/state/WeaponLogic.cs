namespace Shellguard.Weapon;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta]
[LogicBlock(typeof(State))]
public partial class WeaponLogic : LogicBlock<WeaponLogic.State>
{
  public override Transition GetInitialState() => To<State.Aiming>();

  public abstract partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }

    public partial record Aiming : State, IGet<Input.QueueAttack>
    {
      public Transition On(in Input.QueueAttack input) => To<Swinging>();
    }

    public partial record Swinging : State { }
  }

  public static partial class Input
  {
    public record struct QueueAttack;

    public record struct FinishAttack;
  }

  public partial record struct Output
  {
    public record struct CheckForDamageables(bool Monitoring);

    public record struct Swing(bool Monitor);
  }
}

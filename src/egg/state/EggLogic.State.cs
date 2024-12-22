namespace Shellguard.Egg.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Shellguard.Game.Domain;

public partial class EggLogic
{
  [Meta]
  public abstract partial record State : StateLogic<State>
  {
    public partial record Idle : State, IGet<Input.Collect>, IGet<Input.PhysicsProcess>
    {
      public Transition On(in Input.Collect input) => To<Collected>();

      public Transition On(in Input.PhysicsProcess input)
      {
        var data = Get<Data>();
        data.Elapsed += input.Delta;

        var offset = new Vector2(0.0f, -Mathf.Sin((float)data.Elapsed * 2) * 2);
        Output(new Output.OffsetChanged(offset));
        return ToSelf();
      }
    }

    public partial record Collected : State
    {
      public Collected()
      {
        this.OnEnter(() =>
        {
          Get<IGameRepo>().OnEggCollected();
          Output(new Output.SelfDestruct());
        });
      }
    }
  }
}

namespace Shellguard.Player.State;

using Chickensoft.LogicBlocks;
using Godot;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public partial record Moving : State, IGet<Input.MoveInput>, IGet<Input.AttackInput>
    {
      public Moving()
      {
        this.OnEnter(() => Output(new Output.AnimationChanged("run_s")));
      }

      public Transition On(in Input.MoveInput input)
      {
        if (input.Direction.IsZeroApprox())
        {
          return To<Idle>();
        }
        var data = Get<Data>();
        var velocity = input.Direction * data.Speed;

        Output(new Output.Movement(velocity));

        if (velocity.X != 0)
        {
          Output(
            new Output.FaceDirectionChanged(
              velocity.X < 0 ? FaceDirection.Left : FaceDirection.Right
            )
          );
        }

        return ToSelf();
      }

      public Transition On(in Input.AttackInput input) => To<Attacking>();
    }
  }
}

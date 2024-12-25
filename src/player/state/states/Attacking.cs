namespace Shellguard.Player.State;

using Chickensoft.LogicBlocks;
using Shellguard.Weapon;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    public record Attacking : State, IGet<Input.AnimationFinished>
    {
      public Attacking()
      {
        this.OnEnter(() =>
        {
          var weapon = Get<IWeapon>();
          if (weapon.Direction.X < 0)
          {
            Output(new Output.FaceDirectionChanged(FaceDirection.Left));
          }
          else if (weapon.Direction.X > 0)
          {
            Output(new Output.FaceDirectionChanged(FaceDirection.Right));
          }
          Output(new Output.AnimationChanged(weapon.Animation));
        });
      }

      public Transition On(in Input.AnimationFinished input) =>
        input.Animation == "hammer" ? To<Idle>() : ToSelf();
    }
  }
}

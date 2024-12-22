namespace Shellguard.Weapon;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IWeapon : IArea2D
{
  void Aim(Vector2 atPosition);
  void Attack();
}

[Meta(typeof(IAutoNode))]
public partial class Weapon : Area2D, IWeapon
{
  public override void _Notification(int what) => this.Notify(what);

  [Export]
  float AngleOffset;
  #region State
  private WeaponLogic Logic { get; set; } = default!;
  private WeaponLogic.IBinding Binding { get; set; } = default!;

  #endregion


  public void Setup() => Logic = new WeaponLogic();

  public void OnResolved()
  {
    Binding = Logic.Bind();
    Binding.Handle((in WeaponLogic.Output.Swing _) => GD.Print("Swing!"));
    Binding.Handle(
      (in WeaponLogic.Output.CheckForDamageables output) => Monitoring = output.Monitoring
    );

    Logic.Start();
  }

  public void Aim(Vector2 atPosition)
  {
    var direct_angle = GlobalPosition.DirectionTo(atPosition).Angle();
    GlobalRotation = direct_angle + Mathf.DegToRad(AngleOffset);
  } // FIXME go through logic?

  public void Attack() => Logic.Input(new WeaponLogic.Input.QueueAttack());
}

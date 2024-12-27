namespace Shellguard.Weapon;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IWeapon : IArea2D
{
  public string Animation { get; }
  public Vector2 Direction { get; }
  void Attack();
  void Aim(Vector2 atPosition);
}

[Meta(typeof(IAutoNode))]
public partial class Weapon : Area2D, IWeapon
{
  public override void _Notification(int what) => this.Notify(what);

  #region Exports
  [Export]
  private float _angleOffset;

  [Export]
  private string _animation = default!;
  #endregion

  #region State
  private WeaponLogic Logic { get; set; } = default!;
  private WeaponLogic.IBinding Binding { get; set; } = default!;
  #endregion

  public void Setup() => Logic = new WeaponLogic();

  public void OnResolved()
  {
    Binding = Logic.Bind();
    Binding.Handle(
      (in WeaponLogic.Output.CheckForDamageables output) => Monitoring = output.Monitoring
    );

    Logic.Start();
  }

  #region Interface
  public Vector2 Direction => Vector2.FromAngle(GlobalRotation);
  public string Animation => _animation;

  public void Attack() => Logic.Input(new WeaponLogic.Input.QueueAttack());

  public void Aim(Vector2 atPosition)
  {
    var direct_angle = GlobalPosition.DirectionTo(atPosition).Angle();
    GlobalRotation = direct_angle + Mathf.DegToRad(_angleOffset);
  }
  #endregion
}

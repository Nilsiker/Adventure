namespace Shellguard.Player;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Shellguard.Player.State;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class PlayerModel : Node2D
{
  public override void _Notification(int what) => this.Notify(what);

  [Dependency]
  public IPlayerLogic PlayerLogic => this.DependOn<IPlayerLogic>();

  public PlayerLogic.IBinding PlayerBinding { get; set; } = default!;

  #region Nodes
  [Node("%Sprite2D")]
  private ISprite2D Sprite { get; set; } = default!;

  [Node("%AnimationPlayer")]
  private IAnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion

  public void OnResolved()
  {
    PlayerBinding = PlayerLogic.Bind();

    PlayerBinding.Handle(
      (in PlayerLogic.Output.Movement output) => Sprite.FlipH = output.Velocity.X < 0
    );
  }
}

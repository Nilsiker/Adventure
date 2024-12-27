namespace Shellguard.Player;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;
using Shellguard.Game;
using Shellguard.Player.State;
using Shellguard.Weapon;

public interface IPlayer : ICharacterBody2D { }

[Meta(typeof(IAutoNode))]
public partial class Player : CharacterBody2D, IPlayer
{
  #region Exports
  [Export]
  private float _speed = 100.0f;

  [Export]
  private PackedScene _bonk = default!;
  #endregion

  #region State
  private PlayerLogic Logic { get; set; } = default!;
  private PlayerLogic.IBinding Binding { get; set; } = default!;
  private ISaveChunk<PlayerData> PlayerChunk { get; set; } = default!;

  #endregion

  #region Nodes
  [Node]
  private IWeapon Weapon { get; set; } = default!;

  [Node]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;

  [Node]
  private Sprite2D Sprite { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency]
  private ISaveChunk<GameData> GameChunk => this.DependOn<ISaveChunk<GameData>>();
  #endregion


  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new();

    PlayerChunk = new SaveChunk<PlayerData>(
      onSave: (chunk) =>
        new PlayerData()
        {
          GlobalTransform = GlobalTransform,
          StateMachine = Logic,
          Velocity = Velocity,
        },
      onLoad: (chunk, data) =>
      {
        GlobalTransform = data.GlobalTransform;
        Velocity = data.Velocity;
        Logic.RestoreFrom(data.StateMachine);
        Logic.Start();
      }
    );
  }

  public void OnResolved()
  {
    GameChunk.AddChunk(PlayerChunk);

    Logic.Set(Weapon);
    Logic.Set(AnimationPlayer);
    Logic.Set(new PlayerLogic.Data(_speed));

    Binding = Logic.Bind();
    Binding
      .Handle((in PlayerLogic.Output.Movement output) => OnMovementOutput(output.Velocity))
      .Handle((in PlayerLogic.Output.Attack output) => OnAttackOutput(output.Direction))
      .Handle(
        (in PlayerLogic.Output.FaceDirectionChanged output) =>
          OnFaceDirectionChangedOutput(output.FaceDirection)
      )
      .Handle(
        (in PlayerLogic.Output.AnimationChanged output) => OnAnimationChanged(output.Animation)
      );
    Logic.Start();
  }
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public override void _Ready()
  {
    AnimationPlayer.AnimationFinished += OnAnimationPlayerAnimationFinished;
    GetNode<Camera2D>("Camera2D").MakeCurrent();
  }

  public override void _PhysicsProcess(double delta)
  {
    Logic.Input(
      new PlayerLogic.Input.MoveInput(
        Input.GetVector(Inputs.Left, Inputs.Right, Inputs.Up, Inputs.Down)
      )
    );

    // Aim weapon (Go through logic?)
    Weapon.Aim(GetGlobalMousePosition());
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event.IsActionPressed(Inputs.Attack))
    {
      Logic.Input(new PlayerLogic.Input.AttackInput());
    }
  }

  public override void _ExitTree()
  {
    base._ExitTree();

    Logic.Stop();
    Binding.Dispose();

    AnimationPlayer.AnimationFinished -= OnAnimationPlayerAnimationFinished;
  }
  #endregion

  #region Input Callbacks
  private void OnAnimationPlayerAnimationFinished(StringName animName) =>
    Logic.Input(new PlayerLogic.Input.AnimationFinished(animName));
  #endregion

  #region Output Callbacks
  private void OnMovementOutput(Vector2 velocity)
  {
    Velocity = velocity;
    MoveAndSlide();
  }

  private void OnAttackOutput(Vector2 direction) => Weapon.Attack();

  private void OnAnimationChanged(string animation) => AnimationPlayer.Play(animation);

  private void OnFaceDirectionChangedOutput(PlayerLogic.FaceDirection faceDirection)
  {
    switch (faceDirection)
    {
      case PlayerLogic.FaceDirection.Left:
        Sprite.FlipH = false;
        break;
      case PlayerLogic.FaceDirection.Right:
        Sprite.FlipH = true;
        break;
      default:
        break;
    }
  }
  #endregion
}

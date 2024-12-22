namespace Shellguard.Game;

using Chickensoft.AutoInject;
using Chickensoft.GoDotLog;
using Chickensoft.Introspection;
using Shellguard.Game.Domain;
using Shellguard.Game.State;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IProvide<IGameRepo>
{
  public override void _Notification(int what) => this.Notify(what);

  private readonly GDLog _log = new(nameof(Game));

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic GameLogic { get; set; } = default!;
  public GameLogic.IBinding GameBinding { get; set; } = default!;
  #endregion

  #region Provisions
  public IGameRepo Value => GameRepo;
  #endregion


  public void Setup()
  {
    GameRepo = new GameRepo();
    GameLogic = new GameLogic();

    GameLogic.Set(GameRepo);
  }

  public void OnResolved()
  {
    GameBinding = GameLogic.Bind();

    GameBinding.Handle((in GameLogic.Output.SetPauseMode output) => SetGamePaused(output.IsPaused));

    this.Provide();
  }

  private void SetGamePaused(bool isPaused) => GetTree().Paused = isPaused;

  public override void _Input(InputEvent @event)
  {
    if (Input.IsActionJustPressed("esc"))
    {
      GameLogic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnExitTree()
  {
    GameLogic.Stop();
    GameBinding.Dispose();
    GameRepo.Dispose();
  }
}

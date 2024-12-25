namespace Shellguard.Game;

using Chickensoft.AutoInject;
using Chickensoft.GoDotLog;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Game.Domain;
using Shellguard.Game.State;

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IProvide<IGameRepo>
{
  public override void _Notification(int what) => this.Notify(what);

  private readonly GDLog _log = new(nameof(Game));

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic Logic { get; set; } = default!;
  public GameLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  public IGameRepo Value => GameRepo;
  #endregion


  public void Setup()
  {
    GameRepo = new GameRepo();
    Logic = new GameLogic();

    Logic.Set(GameRepo);
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    Binding.When<GameLogic.State>(state => GD.Print(state.ToString()));
    Binding.Handle((in GameLogic.Output.SetPauseMode output) => SetGamePaused(output.IsPaused));

    this.Provide();
  }

  private void SetGamePaused(bool isPaused) => GetTree().Paused = isPaused;

  public override void _Input(InputEvent @event)
  {
    if (Input.IsActionJustPressed(Inputs.Esc))
    {
      GD.Print("esc");
      Logic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
    GameRepo.Dispose();
  }
}

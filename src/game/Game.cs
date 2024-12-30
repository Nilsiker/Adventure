namespace Shellguard.Game;

using Chickensoft.AutoInject;
using Chickensoft.GoDotLog;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;
using Shellguard.Game.Domain;
using Shellguard.Game.State;
using Shellguard.Player;

public interface IGame : INode2D, IProvide<IGameRepo>, IProvide<ISaveChunk<GameData>>
{
  void StartNewGame();
  void RequestLoadGame();
  void RequestSaveGame();
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IGame
{
  private readonly GDLog _log = new(nameof(Game));

  #region Save
  public ISaveChunk<GameData> GameChunk { get; set; } = default!;

  #endregion

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic Logic { get; set; } = default!;
  public GameLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  ISaveChunk<GameData> IProvide<ISaveChunk<GameData>>.Value() => GameChunk;

  public IGameRepo Value() => GameRepo;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new GameLogic();
    GameChunk = new SaveChunk<GameData>(
      (chunk) =>
      {
        var gameData = new GameData() { PlayerData = chunk.GetChunkSaveData<PlayerData>() };
        return gameData;
      },
      onLoad: (chunk, data) => chunk.LoadChunkSaveData(data.PlayerData)
    );

    GameRepo = new GameRepo(GameChunk);
    Logic.Set(GameRepo);
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    Binding.Handle((in GameLogic.Output.SetPauseMode output) => SetGamePaused(output.IsPaused));

    this.Provide();
  }
  #endregion


  #region Input Callbacks
  public void StartNewGame() => Logic.Input(new GameLogic.Input.StartGame());

  public void RequestLoadGame() => Logic.Input(new GameLogic.Input.RequestLoad());

  public void RequestSaveGame() => Logic.Input(new GameLogic.Input.RequestSave());
  #endregion

  #region Output Callbacks
  private void SetGamePaused(bool isPaused) => GetTree().Paused = isPaused;
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public override void _Input(InputEvent @event)
  {
    if (Input.IsActionJustPressed(Inputs.Esc))
    {
      Logic.Input(new GameLogic.Input.PauseButtonPressed());
    }
    else if (Input.IsActionJustPressed(Inputs.Quicksave))
    {
      RequestSaveGame();
    }
    else if (Input.IsActionJustPressed(Inputs.Quickload))
    {
      RequestLoadGame();
    }
  }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
    GameRepo.Dispose();
  }
  #endregion
}

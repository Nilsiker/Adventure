namespace Shellguard.Game;

using System;
using System.IO.Abstractions;
using System.Text.Json;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GoDotLog;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
using Chickensoft.Serialization.Godot;
using Godot;
using Shellguard.Game.Domain;
using Shellguard.Game.State;
using Shellguard.Player;
using Shellguard.Save;

public interface IGame
  : INode2D,
    IProvide<IGameRepo>,
    IProvide<ISaveChunk<GameData>>,
    IProvide<EntityTable>
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
  [Signal]
  public delegate void SaveFileLoadedEventHandler();
  public JsonSerializerOptions JsonOptions { get; set; } = default!;
  public IFileSystem FileSystem { get; set; } = default!;
  public IEnvironmentProvider Environment { get; set; } = default!;
  public EntityTable EntityTable { get; set; } = new();

  public ISaveFile<GameData> SaveFile { get; set; } = default!;
  public ISaveChunk<GameData> GameChunk { get; set; } = default!;

  #endregion

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic Logic { get; set; } = default!;
  public GameLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  EntityTable IProvide<EntityTable>.Value() => EntityTable;

  ISaveChunk<GameData> IProvide<ISaveChunk<GameData>>.Value() => GameChunk;

  public IGameRepo Value() => GameRepo;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    GameRepo = new GameRepo();
    Logic = new GameLogic();
    Logic.Set(GameRepo);
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    Binding
      .Handle((in GameLogic.Output.SetPauseMode output) => SetGamePaused(output.IsPaused))
      .Handle((in GameLogic.Output.StartSaving output) => OnStartSaving());

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

  private void OnStartSaving() =>
    SaveFile.Save().ContinueWith(_ => Logic.Input(new GameLogic.Input.SaveCompleted()));
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

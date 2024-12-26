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

public interface IGame
  : INode2D,
    IProvide<IGameRepo>,
    IProvide<ISaveChunk<GameData>>,
    IProvide<EntityTable>
{
  void StartNewGame();
  void LoadExistingGame();
  void SaveGame();
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IGame
{
  private readonly GDLog _log = new(nameof(Game));

  #region Interface
  public void StartNewGame() => throw new NotImplementedException();

  public void LoadExistingGame() => throw new NotImplementedException();

  public void SaveGame() => throw new NotImplementedException();
  #endregion

  #region Save
  [Signal]
  public delegate void SaveFileLoadedEventHandler();
  public JsonSerializerOptions JsonOptions { get; set; } = default!;
  public const string SAVE_FILE_NAME = "game.json";
  public IFileSystem FileSystem { get; set; } = default!;
  public IEnvironmentProvider Environment { get; set; } = default!;
  public string SaveFilePath { get; set; } = default!;
  public EntityTable EntityTable { get; set; } = new();
  EntityTable IProvide<EntityTable>.Value => EntityTable;
  public ISaveFile<GameData> SaveFile { get; set; } = default!;
  public ISaveChunk<GameData> GameChunk { get; set; } = default!;

  ISaveChunk<GameData> IProvide<ISaveChunk<GameData>>.Value => GameChunk;
  #endregion

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic Logic { get; set; } = default!;
  public GameLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  public IGameRepo Value => GameRepo;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    FileSystem = new FileSystem();
    SaveFilePath = FileSystem.Path.Join(OS.GetUserDataDir(), SAVE_FILE_NAME);
    var resolver = new SerializableTypeResolver();
    // Tell our type type resolver about the Godot-specific converters.
    GodotSerialization.Setup();
    var upgradeDependencies = new Blackboard();
    // Create a standard JsonSerializerOptions with our introspective type
    // resolver and the logic blocks converter.
    JsonOptions = new JsonSerializerOptions
    {
      Converters = { new SerializableTypeConverter(upgradeDependencies) },
      TypeInfoResolver = resolver,
      WriteIndented = true,
    };

    GameRepo = new GameRepo();
    Logic = new GameLogic();
    Logic.Set(GameRepo);
  }

  public void OnResolved()
  {
    SaveFile = new SaveFile<GameData>(
      root: GameChunk,
      onSave: async (GameData data) =>
      {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await FileSystem.File.WriteAllTextAsync(SaveFilePath, json);
      },
      onLoad: async () =>
      {
        // Load the game data from disk.
        if (!FileSystem.File.Exists(SaveFilePath))
        {
          GD.Print("No save file to load :'(");
          return null;
        }

        var json = await FileSystem.File.ReadAllTextAsync(SaveFilePath);
        return JsonSerializer.Deserialize<GameData>(json, JsonOptions);
      }
    );

    Binding = Logic.Bind();

    Binding.When<GameLogic.State>(state => GD.Print(state.ToString()));
    Binding.Handle((in GameLogic.Output.SetPauseMode output) => SetGamePaused(output.IsPaused));

    this.Provide();
  }
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
      GD.Print("esc");
      Logic.Input(new GameLogic.Input.PauseButtonPressed());
    }
    else if (Input.IsActionJustPressed(Inputs.Quicksave))
    {
      GameRepo.RequestSave(); // TODO this is debug only, remove
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

namespace Shellguard;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;
using Shellguard.Game;
using Shellguard.Save;
using Shellguard.SaveData.App;

public interface IApp
  : INode,
    IProvide<IAppRepo>,
    IProvide<ISaveChunk<AppData>>,
    IProvide<IGameFileService>;

[Meta(typeof(IAutoNode))]
public partial class App : Node, IApp
{
  #region Save
  private IGameFileService GameFileService { get; set; } = default!;
  #endregion

  #region Exports
  [Export]
  private PackedScene _gameScene = default!;
  #endregion

  #region State
  private IAppRepo AppRepo { get; set; } = default!;
  private ISaveChunk<AppData> AppChunk { get; set; } = default!;
  private AppLogic Logic { get; set; } = default!;
  private AppLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

  public ISaveChunk<AppData> Value() => AppChunk;

  IGameFileService IProvide<IGameFileService>.Value() => GameFileService;
  #endregion

  #region Nodes
  [Node("GameContainer")]
  private Node2D GameContainer { get; set; } = default!;

  [Node("%MainMenu")]
  private Control MainMenu { get; set; } = default!;

  [Node("AnimationPlayer")]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;

  private IGame Game { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    AppChunk = new SaveChunk<AppData>(
      (chunk) =>
      {
        var appData = new AppData() { GameData = chunk.GetChunkSaveData<GameData>() };
        return appData;
      },
      onLoad: (chunk, data) => chunk.LoadChunkSaveData(data.GameData)
    );
    GameFileService = new GameFileService<AppData>(AppChunk);

    AppRepo = new AppRepo(GameFileService);

    Logic = new();
    Logic.Set(AppRepo);
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in AppLogic.Output.CloseApplication _) => OnOutputQuitApp())
      .Handle((in AppLogic.Output.SetupGame _) => OnOutputSetupGame())
      .Handle((in AppLogic.Output.LoadGame _) => OnOutputLoadGame())
      .Handle((in AppLogic.Output.HideGame _) => GameContainer.Visible = false)
      .Handle((in AppLogic.Output.RemoveGame _) => OnOutputRemoveGame())
      .Handle((in AppLogic.Output.ShowMainMenu _) => MainMenu.Visible = true)
      .Handle((in AppLogic.Output.HideMainMenu _) => MainMenu.Visible = false)
      .Handle((in AppLogic.Output.ShowGame _) => GameContainer.Visible = true)
      .Handle((in AppLogic.Output.FadeIn _) => AnimationPlayer.Play("fade_in"))
      .Handle((in AppLogic.Output.FadeOut _) => AnimationPlayer.Play("fade_out"));

    Logic.Start();
    this.Provide();
  }

  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady()
  {
    SetProcess(false);
    SetPhysicsProcess(false);

    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public void OnExitTree()
  {
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;

    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  private void OnAnimationFinished(StringName animName)
  {
    if (animName == "fade_out")
    {
      Logic.Input(new AppLogic.Input.FadeOutFinished());
    }
  }

  private void NewGame() => Logic.Input(new AppLogic.Input.NewGame());

  private void LoadGame() => Logic.Input(new AppLogic.Input.LoadGame());

  private void QuitApp() => Logic.Input(new AppLogic.Input.QuitApp());
  #endregion

  #region Output Callbacks
  public void OnOutputSetupGame()
  {
    Game = _gameScene.Instantiate<IGame>();
    GameContainer.AddChildEx(Game);
  }

  private void OnOutputLoadGame() => Game.RequestLoadGame();

  public void OnOutputRemoveGame()
  {
    if (GameContainer.GetChildCount() == 0)
    {
      return;
    }

    var game = GameContainer.GetChild(0);
    GameContainer.RemoveChildEx(game);
    game.QueueFree();
  }

  public void OnOutputQuitApp() => GetTree().Quit();
  #endregion
}

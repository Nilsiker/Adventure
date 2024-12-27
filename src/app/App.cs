namespace Shellguard;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Game;

public interface IApp : INode, IProvide<IAppRepo>;

[Meta(typeof(IAutoNode))]
public partial class App : Node, IApp
{
  #region Exports
  [Export]
  private PackedScene _gameScene = default!;
  #endregion

  #region State
  private IAppRepo AppRepo { get; set; } = default!;
  private AppLogic Logic { get; set; } = default!;
  private AppLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  public IAppRepo Value() => AppRepo;
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
    Logic = new();
    AppRepo = new AppRepo();

    Logic.Set(AppRepo);
  }

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in AppLogic.Output.CloseApplication _) => OnQuitApp())
      .Handle((in AppLogic.Output.SetupGame _) => OnSetupGame())
      .Handle((in AppLogic.Output.HideGame _) => GameContainer.Visible = false)
      .Handle((in AppLogic.Output.RemoveGame _) => OnRemoveGame())
      .Handle((in AppLogic.Output.ShowMainMenu _) => MainMenu.Visible = true)
      .Handle((in AppLogic.Output.HideMainMenu _) => MainMenu.Visible = false)
      .Handle((in AppLogic.Output.ShowGame _) => GameContainer.Visible = true)
      .Handle(
        (in AppLogic.Output.FadeIn _) =>
        {
          AnimationPlayer.Play("fade_in");
          GD.Print("fadein");
        }
      )
      .Handle((in AppLogic.Output.Blackout _) => AnimationPlayer.Play("fade_out"));

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
      Logic.Input(new AppLogic.Input.BlackoutFinished());
    }
  }

  private void NewGame() => Logic.Input(new AppLogic.Input.NewGame());

  private void QuitApp() => Logic.Input(new AppLogic.Input.QuitApp());
  #endregion

  #region Output Callbacks
  public void OnSetupGame()
  {
    var game = _gameScene.Instantiate();
    GameContainer.AddChildEx(game);
  }

  public void OnRemoveGame()
  {
    if (GameContainer.GetChildCount() == 0)
    {
      return;
    }

    var game = GameContainer.GetChild(0); // TODO this is called on every Main Menu state, and causes error on first app laod.
    GameContainer.RemoveChildEx(game);
    game.QueueFree();
  }

  public void OnQuitApp() => GetTree().Quit();
  #endregion
}

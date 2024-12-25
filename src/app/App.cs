namespace Shellguard;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

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
  public IAppRepo Value => AppRepo;
  #endregion

  #region Nodes
  [Node("%GameViewport")]
  private ISubViewport GameViewPort { get; set; } = default!;

  [Node("MainMenu")]
  private IControl MainMenu { get; set; } = default!;

  [Node("AnimationPlayer")]
  private IAnimationPlayer AnimationPlayer { get; set; } = default!;
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
      .Handle((in AppLogic.Output.HideGame _) => GameViewPort.GetParent<Control>().Visible = false)
      .Handle((in AppLogic.Output.RemoveGame _) => OnRemoveGame())
      .Handle((in AppLogic.Output.ShowMainMenu _) => MainMenu.Visible = true)
      .Handle((in AppLogic.Output.HideMainMenu _) => MainMenu.Visible = false)
      .Handle((in AppLogic.Output.ShowGame _) => GameViewPort.GetParent<Control>().Visible = true)
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

  private void QuitApp() => Logic.Input(new AppLogic.Input.QuitApp());
  #endregion

  #region Output Callbacks
  public void OnSetupGame()
  {
    var game = _gameScene.Instantiate();
    GameViewPort.AddChildEx(game);
  }

  public void OnRemoveGame()
  {
    var game = GameViewPort.GetChild(1);
    GameViewPort.RemoveChildEx(game);
    game.QueueFree();
  }

  public void OnQuitApp() => GetTree().Quit();
  #endregion
}

public interface IAppLogic : ILogicBlock<AppLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.State>, IAppLogic
{
  public override Transition GetInitialState() => To<State.InMainMenu>();

  public static class Input
  {
    public record struct NewGame;

    public record struct BackToMainMenu;

    public record struct QuitGame;

    public record struct QuitApp;

    public record struct FadeOutFinished;
  }

  public static class Output
  {
    public record struct SetupGame;

    public record struct ShowGame;

    public record struct HideGame;

    public record struct RemoveGame;

    public record struct ShowMainMenu;

    public record struct HideMainMenu;

    public record struct CloseApplication;

    public record struct FadeIn;

    public record struct FadeOut;
  }

  public abstract partial record State : StateLogic<State>, IGet<Input.QuitApp>
  {
    public State()
    {
      OnAttach(() => Get<IAppRepo>().AppQuit += OnAppQuit);
      OnDetach(() => Get<IAppRepo>().AppQuit -= OnAppQuit);
    }

    public Transition On(in Input.QuitApp input) => To<ClosingApplication>();

    private void OnAppQuit() => Input(new Input.QuitApp()); // NOTE: Is this kosher?

    public partial record InMainMenu : State, IGet<Input.NewGame>, IGet<Input.QuitApp>
    {
      public InMainMenu()
      {
        OnAttach(() => Output(new Output.ShowMainMenu()));
        OnDetach(() => { });
      }

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();

      Transition IGet<Input.NewGame>.On(in Input.NewGame input) => To<InGame>();
    }

    public partial record InGame : State, IGet<Input.BackToMainMenu>, IGet<Input.QuitApp>
    {
      public InGame()
      {
        OnAttach(() =>
        {
          Output(new Output.SetupGame());
          Output(new Output.ShowGame());
          Output(new Output.HideMainMenu());
        });
        OnDetach(() => { });
      }

      Transition IGet<Input.BackToMainMenu>.On(in Input.BackToMainMenu input) => To<InMainMenu>();

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();
    }

    public partial record ClosingApplication : State, IGet<Input.FadeOutFinished>
    {
      public ClosingApplication()
      {
        OnAttach(() => Output(new Output.FadeOut()));
        OnDetach(() => { });
      }

      public Transition On(in Input.FadeOutFinished input)
      {
        Output(new Output.CloseApplication());
        return ToSelf();
      }
    }
  }
}

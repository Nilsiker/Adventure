namespace Shellguard;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IApp : INode, IProvide<IAppRepo>
{
  void StartGame();
  void SaveGame();
  void LoadGame();
  void QuitApp();
}

[Meta(typeof(IAutoNode))]
public partial class App : Node, IApp
{
  #region Interface
  public void StartGame() => GetTree().ChangeSceneToPacked(_gameScene);

  public void SaveGame() => throw new System.NotImplementedException();

  public void LoadGame() => throw new System.NotImplementedException();

  public void QuitApp() => GetTree().Quit();
  #endregion

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
      .Handle((in AppLogic.Output.StartGame _) => StartGame())
      .Handle((in AppLogic.Output.QuitApp _) => QuitApp());

    Logic.Start();
    this.Provide();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady()
  {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}

public interface IAppLogic : ILogicBlock<AppLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.State>, IAppLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output
  {
    public record struct StartGame;

    public record struct QuitApp;
  }

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() =>
      {
        Get<IAppRepo>().AppQuit += OnAppQuit;
        Get<IAppRepo>().GameStarted += OnGameStarted;
      });
      OnDetach(() =>
      {
        Get<IAppRepo>().AppQuit -= OnAppQuit;
        Get<IAppRepo>().GameStarted -= OnGameStarted;
      });
    }

    private void OnGameStarted() => Output(new Output.StartGame());

    private void OnAppQuit() => Output(new Output.QuitApp());
  }
}

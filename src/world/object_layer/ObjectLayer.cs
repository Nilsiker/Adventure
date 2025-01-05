namespace Shellguard.World;

using System.Collections.Generic;
using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using ITree = Tree.ITree;

public interface IObjectLayer : ITileMapLayer
{
  IEnumerable<ITree> Trees { get; }
}

[Meta(typeof(IAutoNode))]
public partial class ObjectLayer : TileMapLayer, IObjectLayer
{
  public IEnumerable<ITree> Trees => GetChildren().OfType<ITree>();

  #region Exports
  [Export]
  private PackedScene? _treeScene;
  #endregion

  #region State
  private TreeLayerLogic Logic { get; set; } = default!;
  private TreeLayerLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here

    Logic.Start();
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

  public override void _UnhandledInput(InputEvent @event) => base._UnhandledInput(@event);
}

public interface ITreeLayerLogic : ILogicBlock<TreeLayerLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class TreeLayerLogic : LogicBlock<TreeLayerLogic.State>, ITreeLayerLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input
  {
    public record struct Initialize;

    public record struct AddTree(Vector2I Position);

    public record struct RemoveTree(Vector2I Position);
  }

  public static class Output
  {
    public record struct TreeAdded(Vector2I Position);

    public record struct TreeRemoved(Vector2I Position);
  }

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}

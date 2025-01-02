namespace ShellGuard;

using System;
using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Godot.Collections;

public enum Layer
{
  Dirt,
  Grass,
}

public interface ITerrain : INode { }

[Meta(typeof(IAutoNode))]
public partial class Terrain : Node, INode, ITerrain
{
  #region Exports
  [Export]
  private Array<Layer> _layers = [];
  #endregion

  #region State
  private TerrainLogic Logic { get; set; } = default!;
  private TerrainLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
  [Node]
  private Label? DebugLabel { get; set; }
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new();

    foreach (var child in GetChildren().OfType<TileMapLayer>())
    {
      child.Call("_update_tileset");
    }
  }

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

  public void OnProcess(double delta)
  {
    CheckTopLayer();
  }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region External Methods
  private void CheckTopLayer()
  {
    var children = GetChildren().OfType<TileMapLayer>().Reverse();
    foreach (var child in children)
    {
      var cell = child.LocalToMap(child.GetLocalMousePosition());
      var data = child.GetCellTileData(cell);
      if (data != null)
      {
        DebugLabel?.Set("text", child.Name);
        DebugLabel?.SetPosition(child.GetLocalMousePosition());
        return;
      }
    }
    DebugLabel.Visible = false;
  }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}

public interface ITerrainLogic : ILogicBlock<TerrainLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class TerrainLogic : LogicBlock<TerrainLogic.State>, ITerrainLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output { }

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}

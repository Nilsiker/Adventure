namespace Shellguard.World;

using System.Collections.Generic;
using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using ITree = Tree.ITree;

public interface ITreeLayer : ITileMapLayer
{
  IEnumerable<ITree> Trees { get; }
}

[Meta(typeof(IAutoNode))]
public partial class TreeLayer : TileMapLayer, ITreeLayer
{
  public IEnumerable<ITree> Trees => GetChildren().OfType<ITree>();

  #region Exports
  [Export]
  private PackedScene _treeScene = default!;
  #endregion

  #region State
  private ObjectLayerLogic Logic { get; set; } = default!;
  private ObjectLayerLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in ObjectLayerLogic.Output.SpawnTree output) => OnOutputSpawnTree(output.Position))
      .Handle(
        (in ObjectLayerLogic.Output.DisableTileRendering _) => OnOutputDisableTileRendering()
      );

    Logic.Input(new ObjectLayerLogic.Input.Initialize([.. GetUsedCells()]));
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
  private void OnOutputSpawnTree(Vector2I position)
  {
    var node = _treeScene.Instantiate<Node2D>();
    node.GlobalPosition = ToGlobal(MapToLocal(position));
    AddChild(node);
  }

  private object OnOutputDisableTileRendering() => Enabled = false;

  #endregion

  public override void _UnhandledInput(InputEvent @event)
  {
    if (
      @event is InputEventMouseButton button
      && button.IsPressed()
      && button.ButtonIndex == MouseButton.Left
    )
    {
      var cell = LocalToMap(GetLocalMousePosition());
      GD.Print(GetCellSourceId(cell));
      if (GetCellSourceId(cell) == -1)
      {
        SetCell(cell, 0, Vector2I.Zero);
        Logic.Input(new ObjectLayerLogic.Input.AddTree(cell));
      }
    }
  }
}

public interface IObjectLayerLogic : ILogicBlock<ObjectLayerLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class ObjectLayerLogic : LogicBlock<ObjectLayerLogic.State>, IObjectLayerLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input
  {
    public record struct Initialize(Vector2I[] Positions);

    public record struct AddTree(Vector2I Position);

    public record struct RemoveTree(Vector2I Position);
  }

  public static class Output
  {
    public record struct SpawnTree(Vector2I Position);

    public record struct DestroyTree(Vector2I Position);

    public record struct DisableTileRendering;
  }

  public partial record State : StateLogic<State>, IGet<Input.AddTree>, IGet<Input.Initialize>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }

    public Transition On(in Input.AddTree input)
    {
      Output(new Output.SpawnTree(input.Position));
      return ToSelf();
    }

    public Transition On(in Input.Initialize input)
    {
      foreach (var position in input.Positions)
      {
        Output(new Output.SpawnTree(position));
      }
      Output(new Output.DisableTileRendering());
      return ToSelf();
    }
  }
}

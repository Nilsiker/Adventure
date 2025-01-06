namespace Shellguard.World;

using System;
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
  private TreeLayerLogic Logic { get; set; } = default!;
  private TreeLayerLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency]
  private IWorldRepo WorldRepo => this.DependOn<IWorldRepo>();
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved()
  {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in TreeLayerLogic.Output.SpawnTree output) => OnOutputSpawnTree(output.Position))
      .Handle((in TreeLayerLogic.Output.DisableTileRendering _) => OnOutputDisableTileRendering());

    Logic.Set(WorldRepo);

    Logic.Input(new TreeLayerLogic.Input.Initialize([.. GetUsedCells()]));
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
    node.GlobalPosition = position;
    AddChild(node);
    GD.Print(node);
  }

  private object OnOutputDisableTileRendering() => Enabled = false;

  #endregion
}

public interface IObjectLayerLogic : ILogicBlock<TreeLayerLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class TreeLayerLogic : LogicBlock<TreeLayerLogic.State>, IObjectLayerLogic
{
  public override Transition GetInitialState() => To<State>();

  public static class Input
  {
    public record struct Initialize(Vector2I[] Positions);
  }

  public static class Output
  {
    public record struct SpawnTree(Vector2I Position);

    public record struct DestroyTree(Vector2I Position);

    public record struct DisableTileRendering;
  }

  public partial record State : StateLogic<State>, IGet<Input.Initialize>
  {
    public State()
    {
      OnAttach(() => Get<IWorldRepo>().TreePlanted += OnWorldTreePlanted);
      OnDetach(() => Get<IWorldRepo>().TreePlanted -= OnWorldTreePlanted);
    }

    private void OnWorldTreePlanted(Vector2I position) => Output(new Output.SpawnTree(position));

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

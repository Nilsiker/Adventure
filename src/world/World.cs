namespace Shellguard.World;

using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.SaveFileBuilder;
using Godot;
using Shellguard.Save;

public interface IWorld : INode2D, IProvide<IWorldRepo> { }

[Meta(typeof(IAutoNode))]
public partial class World : Node2D, IWorld
{
  #region Save
  private ISaveChunk<WorldData> WorldChunk { get; set; } = default!;
  #endregion

  #region Exports
  #endregion


  #region Nodes
  [Node]
  private Sprite2D Cursor { get; set; } = default!;
  #endregion

  #region State
  private WorldRepo WorldRepo { get; set; } = new();
  private WorldLogic Logic { get; set; } = default!;
  private WorldLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  public IWorldRepo Value() => WorldRepo;
  #endregion


  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved()
  {
    WorldChunk = new SaveChunk<WorldData>(
      onSave: (chunk) =>
        new()
        {
          Trees =
          [ /* TODO assign */
          ],
        },
      onLoad: (chunk, data) => { }
    );
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding.Handle(
      (in WorldLogic.Output.SelectedTilesUpdated output) =>
        OnOutputSelectedTilesUpdated(output.Tiles)
    );

    this.Provide();

    Logic.Set(WorldRepo as IWorldRepo);
    Logic.Start();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() { }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Output Callbacks
  private void OnOutputSelectedTilesUpdated(IEnumerable<Vector2I> tiles)
  {
    foreach (var tile in tiles)
    {
      Cursor.GlobalPosition = tile;
    }
  }
  #endregion
}

public interface IWorldLogic : ILogicBlock<WorldLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class WorldLogic : LogicBlock<WorldLogic.State>, IWorldLogic
{
  public static class Output
  {
    public record struct SelectedTilesUpdated(IEnumerable<Vector2I> Tiles);
  }

  public override Transition GetInitialState() => To<State>();

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() =>
      {
        var world = Get<IWorldRepo>();
        world.SelectedTiles.Sync += OnSelectedTilesSync;
      });
      OnDetach(() =>
      {
        var world = Get<IWorldRepo>();
        world.SelectedTiles.Sync -= OnSelectedTilesSync;
      });
    }

    private void OnSelectedTilesSync(HashSet<Vector2I> tiles) =>
      Output(new Output.SelectedTilesUpdated(tiles));
  }
}

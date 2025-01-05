namespace Shellguard.World;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.SaveFileBuilder;
using Godot;
using Shellguard.Save;

public interface IWorld : INode2D { }

[Meta(typeof(IAutoNode))]
public partial class World : Node2D, IWorld
{
  #region Save
  private ISaveChunk<WorldData> WorldChunk { get; set; } = default!;
  #endregion

  #region Exports
  #endregion


  #region Nodes
  #endregion

  #region State
  private WorldLogic Logic { get; set; } = default!;
  private WorldLogic.IBinding Binding { get; set; } = default!;
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
}

public interface IWorldLogic : ILogicBlock<WorldLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class WorldLogic : LogicBlock<WorldLogic.State>, IWorldLogic
{
  public override Transition GetInitialState() => To<State>();

  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}

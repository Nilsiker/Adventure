namespace Shellguard.Tree;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;
using Shellguard.Game;
using Shellguard.Traits;

public interface ITree : INode2D, IDamageable { }

[Meta(typeof(IAutoNode))]
public partial class Tree : Node2D, ITree, IProvide<ITreeLogic>
{
  #region Save
  private ISaveChunk<TreeData> TreeChunk { get; set; } = default!;
  #endregion

  #region Exports
  [Export]
  private TreeSettings Settings { get; set; } = default!;
  #endregion

  #region State
  private TreeLogic Logic { get; set; } = default!;
  private TreeLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
  [Node]
  private AudioStreamPlayer2D AudioChop { get; set; } = default!;

  #endregion

  #region Provisions
  public ITreeLogic Value() => Logic;
  #endregion

  #region Dependencies
  [Dependency]
  private ISaveChunk<GameData> GameChunk => this.DependOn<ISaveChunk<GameData>>();
  #endregion

  #region Dependency Lifecycle
  public void Setup()
  {
    Logic = new();

    Logic.Set(Settings as ITreeSettings);
  }

  public void OnResolved()
  {
    TreeChunk = new SaveChunk<TreeData>(
      onSave: (chunk) =>
        new TreeData()
        {
          Age = 0,
          Health = 0,
          TimeToMature = 0,
        },
      onLoad: (chunk, data) => { }
    );
    // GameChunk.AddChunk(TreeChunk);
    // TODO don't do this here, we need a TreeManager class that keeps track of trees and spawns the nodes.
    // ... EntityTable is really close

    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in TreeLogic.Output.Damaged _) => OnOutputDamaged())
      .Handle((in TreeLogic.Output.Destroy _) => OnOutputDestroyed());

    Binding.When<TreeLogic.State>(state => GD.Print(state.GetType().FullName));

    Logic.Set(
      new TreeData
      {
        Age = 0,
        Health = 1,
        TimeToMature = -1,
      }
    );

    this.Provide();

    Logic.Start();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() => SetProcess(true);

  public void OnProcess(double delta) => Age((float)delta);

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  private void Age(float time) => Logic.Input(new TreeLogic.Input.Age(time));
  #endregion

  #region Output Callbacks
  private void OnOutputDamaged() => AudioChop.Play();

  private void OnOutputDestroyed() => QueueFree();
  #endregion

  public override void _UnhandledInput(InputEvent @event)
  {
    if (Input.IsKeyLabelPressed(Key.P))
    {
      Damage(1.0f);
    }
  }

  #region IDamageable
  public void Damage(float damage) => Logic.Input(new TreeLogic.Input.Damage(damage));
  #endregion
}

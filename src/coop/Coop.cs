namespace Shellguard.Coop;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface ICoop : IStaticBody2D { }

[Meta(typeof(IAutoNode))]
public partial class Coop : StaticBody2D, ICoop
{
  #region Exports
  #endregion

  #region State
  private CoopLogic Logic { get; set; } = default!;
  private CoopLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
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
}

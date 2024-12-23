namespace Shellguard.Stats;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class Stats : Node
{
  public override void _Notification(int what) => this.Notify(what);

  #region State

  #endregion
}

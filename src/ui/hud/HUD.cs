namespace Shellguard.UI;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Game.Domain;

[Meta(typeof(IAutoNode))]
public partial class HUD : Control
{
  public override void _Notification(int what) => this.Notify(what);

  #region State
  public HUDLogic Logic { get; set; } = default!;
  public HUDLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  public void Setup() => Logic = new HUDLogic();

  public void OnResolved()
  {
    Logic.Set(GameRepo);

    Binding = Logic.Bind();
    Binding.Handle((in HUDLogic.Output.VisilibilityChanged output) => Visible = output.Visible);

    Logic.Start();
  }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
}

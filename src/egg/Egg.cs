namespace Shellguard.Egg;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Shellguard.Egg.State;
using Shellguard.Game.Domain;
using Godot;

public interface IEgg : IArea2D
{
  public IEggLogic Logic { get; }
  void OnCollectorDetectorAreaEntered(Node body);
}

[Meta(typeof(IAutoNode))]
public partial class Egg : Area2D, IEgg
{
  public override void _Notification(int what) => this.Notify(what);

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  public IEggLogic Logic { get; private set; } = default!;
  private EggLogic.IBinding Binding { get; set; } = default!;

  public void OnReady() => AreaEntered += OnCollectorDetectorAreaEntered;

  public void OnResolved()
  {
    Logic = new EggLogic();
    Logic.Set(GameRepo);

    Binding = Logic.Bind();

    Binding.Handle((in EggLogic.Output.Collected _) => QueueFree());
  }

  public void OnCollectorDetectorAreaEntered(Node body)
  {
    if (body is IEggCollector collector && collector.CanCollectEggs)
    {
      Logic.Input(new EggLogic.Input.Collect());
    }
  }

  public void OnExitTree()
  {
    Logic.Stop();
    Binding.Dispose();
  }
}

namespace Shellguard.Egg;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Egg.State;
using Shellguard.Game.Domain;

public interface IEgg : IArea2D
{
  IEggLogic Logic { get; }
  void OnCollectorDetectorAreaEntered(Node body);
}

[Meta(typeof(IAutoNode))]
public partial class Egg : Area2D, IEgg
{
  #region State
  public IEggLogic Logic { get; private set; } = default!;
  private EggLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region Nodes
  [Node]
  private Sprite2D Sprite { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new EggLogic();

  public void OnResolved()
  {
    Logic.Set(GameRepo);
    Logic.Set(new EggLogic.Data());

    Binding = Logic.Bind();
    Binding.Handle((in EggLogic.Output.OffsetChanged output) => Sprite.Offset = output.Offset);
    Binding.Handle((in EggLogic.Output.SelfDestruct _) => QueueFree());
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() => AreaEntered += OnCollectorDetectorAreaEntered;

  public override void _PhysicsProcess(double delta) =>
    Logic.Input(new EggLogic.Input.PhysicsProcess(delta));

  public void OnCollectorDetectorAreaEntered(Node body)
  {
    if (body is IEggCollector collector && collector.CanCollectEggs)
    {
      Logic.Input(new EggLogic.Input.Collect());
    }
  }

  public void OnExitTree()
  {
    AreaEntered -= OnCollectorDetectorAreaEntered;

    Logic.Stop();
    Binding.Dispose();
  }
  #endregion§
}

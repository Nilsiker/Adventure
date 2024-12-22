namespace Shellguard.Egg;

using System.Data.Common;
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
  public override void _Notification(int what) => this.Notify(what);

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region Nodes
  [Node("%Sprite")]
  Sprite2D Sprite { get; set; } = default!;
  #endregion

  public IEggLogic Logic { get; private set; } = default!;
  private EggLogic.IBinding Binding { get; set; } = default!;

  public void OnReady() => AreaEntered += OnCollectorDetectorAreaEntered;

  public void OnResolved()
  {
    Logic = new EggLogic();
    Logic.Set(GameRepo);
    Logic.Set(new EggLogic.Data());

    Binding = Logic.Bind();
    Binding.Handle((in EggLogic.Output.OffsetChanged output) => Sprite.Offset = output.Offset);
    Binding.Handle((in EggLogic.Output.SelfDestruct _) => QueueFree());
  }

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
    Logic.Stop();
    Binding.Dispose();
  }
}

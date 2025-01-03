namespace Shellguard.Tree;

public partial class TreeLogic
{
  public static class Output
  {
    public record struct StageUpdated(EStage Stage); // TODO replace with stage enum to decouple from Godot?

    public record struct UpdateTransparency(float Alpha);

    public record struct Rustle(float Strength);

    public record struct ProduceWood(int Amount); // TODO replace with Produce/Drop Item method, passing item data?

    public record struct Destroy();
  }
}

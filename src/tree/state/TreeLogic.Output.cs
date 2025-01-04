namespace Shellguard.Tree;

using Chickensoft.GodotNodeInterfaces;

public partial class TreeLogic
{
  public static class Output
  {
    public record struct StageUpdated(int Stage);

    public record struct CanopyUpdated(ITexture2D? Texture);

    public record struct UpdateTransparency(float Alpha);

    public record struct Rustle(float Strength);

    public record struct Damaged;

    public record struct ProduceWood(int Amount); // TODO replace with Produce/Drop Item method, passing item data?

    public record struct Destroy();
  }
}

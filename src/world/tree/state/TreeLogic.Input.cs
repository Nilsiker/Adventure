namespace Shellguard.Tree;

public partial class TreeLogic
{
  public static class Input
  {
    public record struct ChopDown;

    public record struct IncreaseMaturity;

    public record struct Age(float Time);

    public record struct Damage(float Amount);

    public record struct OccludingEntity(bool Occluding);
  }
}

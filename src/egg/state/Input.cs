namespace Shellguard.Egg.State;

public partial class EggLogic
{
  public static class Input
  {
    public partial record struct PhysicsProcess(double Delta);

    public partial record struct Collect;
  }
}

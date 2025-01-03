namespace Shellguard;

using Godot;

public partial class TreeLogic
{
  public static class Output
  {
    public record struct UpdateSprite(Texture2D Texture); // TODO replace with stage enum to decouple from Godot?

    public record struct Rustle(float Strength); // TODO replace with stage enum to decouple from Godot?

    public record struct ProduceWood(int Amount); // TODO replace with Produce/Drop Item method, passing item data?
  }
}

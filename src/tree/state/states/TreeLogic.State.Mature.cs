namespace Shellguard.Tree;

public partial class TreeLogic
{
  public partial record State
  {
    public partial record Mature : State, IGet<Input.ChopDown>, IGet<Input.OccludingEntity>
    {
      protected override int Stage => 3;

      public Mature()
        : base()
      {
        OnAttach(() => { });
        OnDetach(() => { });
      }

      public virtual Transition On(in Input.ChopDown input) => To<Stump>();

      public Transition On(in Input.OccludingEntity input)
      {
        Output(new Output.UpdateTransparency(input.Occluding ? 0.5f : 1.0f));
        return ToSelf();
      }
    }
  }
}

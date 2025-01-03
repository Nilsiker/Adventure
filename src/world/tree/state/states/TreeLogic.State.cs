namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public abstract partial record State
    : StateLogic<State>,
      IGet<Input.Age>,
      IGet<Input.Damage>,
      IGet<Input.OccludingEntity>
  {
    public State()
    {
      OnAttach(() => { });
      OnDetach(() => { });

      this.OnEnter(() => Output(new Output.StageUpdated(Stage)));
    }

    protected abstract EStage Stage { get; }

    public State(StateLogic<State> original)
      : base(original) { }

    public Transition On(in Input.Age input)
    {
      var data = Get<Data>();
      data.Age += input.Time;

      if (data.Age > data.TimeToMature)
      {
        Input(new Input.IncreaseMaturity());
      }

      return ToSelf();
    }

    public Transition On(in Input.Damage input)
    {
      var data = Get<Data>();
      data.Health -= input.Amount;

      Output(new Output.Rustle(input.Amount));

      if (data.Health <= 0)
      {
        Input(new Input.ChopDown());
      }

      return ToSelf();
    }

    public Transition On(in Input.OccludingEntity input)
    {
      Output(new Output.UpdateTransparency(input.Occluding ? 0.5f : 1.0f));
      return ToSelf();
    }
  }
}

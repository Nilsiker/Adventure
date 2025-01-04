namespace Shellguard.Tree;

using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public abstract partial record State : StateLogic<State>, IGet<Input.Age>, IGet<Input.Damage>
  {
    protected abstract EStage Stage { get; }
    protected abstract float Health { get; }
    protected abstract float TimeToMature { get; }

    public State()
    {
      OnAttach(() =>
      {
        var data = Get<Data>();
        data.Health = Health;
        data.TimeToMature = TimeToMature;
      });
      OnDetach(() => { });

      this.OnEnter(() => Output(new Output.StageUpdated(Stage)));
    }

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
      Output(new Output.Damaged());

      if (data.Health <= 0)
      {
        Input(new Input.ChopDown());
      }

      return ToSelf();
    }
  }
}

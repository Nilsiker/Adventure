namespace Shellguard.Tree;

using System.Linq;
using Chickensoft.LogicBlocks;

public partial class TreeLogic
{
  public abstract partial record State : StateLogic<State>, IGet<Input.Age>, IGet<Input.Damage>
  {
    protected abstract int Stage { get; }

    public State()
    {
      OnAttach(() =>
      {
        var settings = Get<ITreeSettings>();
        var data = Get<Data>();
        data.Age = 0;
        data.Health = settings.GetStages().ElementAt(Stage).Health;
        data.TimeToMature = settings.GetStages().ElementAt(Stage).TimeToMature;
        Output(new Output.StageUpdated(Stage));
      });
      OnDetach(() => { });
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

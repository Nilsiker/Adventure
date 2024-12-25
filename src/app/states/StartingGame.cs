namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record StartingGame : State, IGet<Input.FadeOutFinished>
    {
      public StartingGame()
      {
        OnAttach(() => Output(new Output.FadeOut()));
        OnDetach(() => { });
      }

      public Transition On(in Input.FadeOutFinished input) => To<InGame>();
    }
  }
}

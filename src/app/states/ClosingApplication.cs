namespace Shellguard;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record ClosingApplication : State, IGet<Input.FadeOutFinished>
    {
      public ClosingApplication()
      {
        OnAttach(() => Output(new Output.FadeOut()));
        OnDetach(() => { });
      }

      public Transition On(in Input.FadeOutFinished input)
      {
        Output(new Output.CloseApplication());
        return ToSelf();
      }
    }
  }
}

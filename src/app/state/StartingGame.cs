namespace Shellguard;

using System;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record StartingGame : State, IGet<Input.GameReady>
    {
      public StartingGame()
      {
        OnAttach(() =>
        {
          if (false) // TODO check for existing game? Maybe this shouldnt live here
          {
            throw new NotImplementedException();
          }
          else
          {
            Input(new Input.GameReady());
          }
        });
        OnDetach(() => { });
      }

      public Transition On(in Input.GameReady input) => To<InGame>();
    }
  }
}

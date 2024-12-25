namespace Shellguard.Game.State;

using Shellguard.Game.Domain;

public partial class GameLogic
{
  public abstract partial record State
  {
    public partial record Paused : State, IGet<Input.PauseButtonPressed>
    {
      public Paused()
      {
        OnAttach(() =>
        {
          Get<IGameRepo>().IsPaused.Changed += OnIsPausedChanged;
          Get<IGameRepo>().Pause();
        });
        OnDetach(() =>
        {
          Get<IGameRepo>().IsPaused.Changed -= OnIsPausedChanged;
          Get<IGameRepo>().Resume();
        });
      }

      private void OnIsPausedChanged(bool isPaused)
      {
        if (!isPaused)
        {
          Input(new Input.PauseButtonPressed());
        }
      }

      public Transition On(in Input.PauseButtonPressed input) => To<Playing>();
    }
  }
}

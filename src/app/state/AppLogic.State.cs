namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public abstract partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() => Get<IAppRepo>().AppQuit += OnAppQuit);
      OnDetach(() => Get<IAppRepo>().AppQuit -= OnAppQuit);
    }

    private void OnAppQuit() => Input(new Input.QuitApp()); // NOTE: Is this kosher?
  }
}

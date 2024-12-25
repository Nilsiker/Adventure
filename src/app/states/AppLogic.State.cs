namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public abstract partial record State : StateLogic<State>, IGet<Input.QuitApp>
  {
    public State()
    {
      OnAttach(() => Get<IAppRepo>().AppQuit += OnAppQuit);
      OnDetach(() => Get<IAppRepo>().AppQuit -= OnAppQuit);
    }

    public Transition On(in Input.QuitApp input) => To<ClosingApplication>();

    private void OnAppQuit() => Input(new Input.QuitApp()); // NOTE: Is this kosher?
  }
}

namespace Shellguard;

using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record InMainMenu
      : State,
        IGet<Input.NewGame>,
        IGet<Input.QuitApp>,
        IGet<Input.LoadGame>
    {
      public InMainMenu()
      {
        this.OnEnter(() =>
        {
          Get<IAppRepo>().ScanForGameFile();
          Get<Data>().ShouldLoadGame = false;

          Output(new Output.ShowMainMenu());
        });
      }

      Transition IGet<Input.LoadGame>.On(in Input.LoadGame input)
      {
        Get<Data>().ShouldLoadGame = true;
        return To<LeavingMenu>();
      }

      Transition IGet<Input.NewGame>.On(in Input.NewGame input) => To<LeavingMenu>();

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();
    }
  }
}

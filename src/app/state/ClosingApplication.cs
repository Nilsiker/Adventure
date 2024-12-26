namespace Shellguard;

using Godot;

public partial class AppLogic
{
  public abstract partial record State
  {
    public partial record ClosingApplication : State
    {
      public ClosingApplication()
      {
        OnAttach(() => Output(new Output.CloseApplication()));
        OnDetach(() => { });
      }
    }
  }
}

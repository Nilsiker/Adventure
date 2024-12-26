namespace Shellguard.UI;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IMainMenu : IControl { }

[Meta(typeof(IAutoNode))]
public partial class MainMenu : Control, IMainMenu
{
  #region Signals
  [Signal]
  public delegate void PlayPressedEventHandler();

  [Signal]
  public delegate void QuitPressedEventHandler();

  #endregion

  #region Nodes
  [Node("%Play")]
  private IButton PlayButton { get; set; } = default!;

  [Node("%Options")]
  private IButton OptionsButton { get; set; } = default!;

  [Node("%Credits")]
  private IButton CreditsButton { get; set; } = default!;

  [Node("%Quit")]
  private IButton QuitButton { get; set; } = default!;
  #endregion

  #region Dependencies
  [Dependency]
  private IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion

  #region Dependency Lifecycle
  public void OnResolved()
  {
    // NOTE UI is stateless, if turns more complex, go through Logic.
    PlayButton.Pressed += OnPlayButtonPressed;
    OptionsButton.Pressed += OnOptionsButtonPressed;
    CreditsButton.Pressed += OnCreditsButtonPressed;
    QuitButton.Pressed += OnQuitButtonPressed;
    GD.Print(AppRepo);
  }
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() { }

  public void OnExitTree()
  {
    PlayButton.Pressed -= OnPlayButtonPressed;
    OptionsButton.Pressed -= OnOptionsButtonPressed;
    CreditsButton.Pressed -= OnCreditsButtonPressed;
    QuitButton.Pressed -= OnQuitButtonPressed;
  }
  #endregion

  #region Signal Callbacks
  private void OnPlayButtonPressed() => EmitSignal(SignalName.PlayPressed);

  private void OnCreditsButtonPressed() => throw new NotImplementedException();

  private void OnOptionsButtonPressed() => throw new NotImplementedException();

  private void OnQuitButtonPressed() => EmitSignal(SignalName.QuitPressed);

  #endregion
}

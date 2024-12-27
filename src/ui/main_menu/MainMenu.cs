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
  [Node]
  private Button PlayButton { get; set; } = default!;

  [Node]
  private Button LoadButton { get; set; } = default!;

  [Node]
  private Button OptionsButton { get; set; } = default!;

  [Node]
  private Button CreditsButton { get; set; } = default!;

  [Node]
  private Button QuitButton { get; set; } = default!;
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
    LoadButton.Pressed += OnLoadButtonPressed;
    OptionsButton.Pressed += OnOptionsButtonPressed;
    CreditsButton.Pressed += OnCreditsButtonPressed;
    QuitButton.Pressed += OnQuitButtonPressed;

    // NOTE: taking a shortcut since main menu is fairly basic now
    AppRepo.HasExistingGame.Sync += OnAppRepoHasExistingGameSync;
  }

  private void OnAppRepoHasExistingGameSync(bool exists)
  {
    GD.Print("SAVE FILE", exists);
    LoadButton.Visible = exists;
  }
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() { }

  public void OnExitTree()
  {
    PlayButton.Pressed -= OnPlayButtonPressed;
    LoadButton.Pressed -= OnLoadButtonPressed;
    OptionsButton.Pressed -= OnOptionsButtonPressed;
    CreditsButton.Pressed -= OnCreditsButtonPressed;
    QuitButton.Pressed -= OnQuitButtonPressed;

    AppRepo.HasExistingGame.Sync -= OnAppRepoHasExistingGameSync;
  }
  #endregion

  #region Signal Callbacks
  private void OnPlayButtonPressed() => EmitSignal(SignalName.PlayPressed);

  private void OnLoadButtonPressed() => throw new NotImplementedException();

  private void OnCreditsButtonPressed() => throw new NotImplementedException();

  private void OnOptionsButtonPressed() => throw new NotImplementedException();

  private void OnQuitButtonPressed() => EmitSignal(SignalName.QuitPressed);

  #endregion
}

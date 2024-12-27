namespace Shellguard.UI;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Shellguard.Game.Domain;

[Meta(typeof(IAutoNode))]
public partial class SaveNotification : TextureRect, ITextureRect
{
  #region Exports
  [Export]
  private Texture2D _savingTexture = default!;

  [Export]
  private Texture2D _savedTexture = default!;
  #endregion

  #region Dependencies
  [Dependency]
  private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region Nodes
  [Node("AnimationPlayer")]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() { }

  public void OnResolved()
  {
    // Bind functions to state outputs here
    GameRepo.Saving += OnGameSaving;
    GameRepo.Saved += OnGameSaved;
  }
  #endregion


  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady()
  {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnExitTree()
  {
    GameRepo.Saving -= OnGameSaving;
    GameRepo.Saved -= OnGameSaved;
  }
  #endregion

  private void OnGameSaved()
  {
    Texture = _savedTexture;
    AnimationPlayer.Play("fade_out");
  }

  private void OnGameSaving()
  {
    Texture = _savingTexture;
    AnimationPlayer.Play("blink");
  }
}

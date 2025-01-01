namespace Shellguard.TileMap;

using Godot;

public partial class Ground : TileMapLayer
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready() { }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) { }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseMotion motion)
    {
      // var clicked_cell = tile_map_layer.local_to_map(tile_map_layer.get_local_mouse_position())
      // var data = tile_map_layer.get_cell_tile_data(clicked_cell)
      // if data:
      //     return data.get_custom_data("power")
      // else:
      //     return 0
      var cellPos = LocalToMap(GetLocalMousePosition());
      var data = GetCellTileData(cellPos);
      if (data == null)
      {
        return;
      }
    }
  }
}

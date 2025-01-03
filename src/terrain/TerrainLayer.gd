@tool extends TileMapDual
class_name TerrainLayer

var _shader: Shader = preload("res://src/terrain/terrain.gdshader")
var _texture: Texture2D
var _texture_resolution: Vector2i

@export var texture: Texture2D:
	get: return _texture;
	set(v):
		_texture = v
		(material as ShaderMaterial).set_shader_parameter("fill_texture", v)

@export var texture_resolution: Vector2i:
	get: return _texture_resolution;
	set(v):
		if v.x == 0: v.x = 1
		if v.y == 0: v.y = 1
		_texture_resolution = v
		(material as ShaderMaterial).set_shader_parameter("scale", _texture_resolution)



# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	super._ready()

	var shader_mat = ShaderMaterial.new()
	shader_mat.shader = _shader
	material = shader_mat
	shader_mat.set_shader_parameter("fill_texture", _texture)
	shader_mat.set_shader_parameter("scale", _texture_resolution)

	_update_tileset()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	super._process(delta)

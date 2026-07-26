class_name NodeVisual
extends Node3D

@onready var mesh_instance: MeshInstance3D = $MeshInstance3D
@onready var label_display: Label3D = $Label3D

var node_id: int = -1
var point_name: String = ""

var current_hrv: float = 0.0:
    set(value):
        current_hrv = value
        _update_visual_state()

var state_signature: String = "Unknown":
    set(value):
        state_signature = value
        _update_visual_state()


func initialize_node_identity(id: int, label_text: String) -> void:
    node_id = id
    point_name = label_text
    name = "NodeVisual_%d" % id

    if is_inside_tree() and label_display != null:
        label_display.text = str(node_id)


func _ready() -> void:
    if label_display != null and node_id != -1:
        label_display.text = str(node_id)

    _update_visual_state()


func _update_visual_state() -> void:
    if not is_inside_tree():
        return

    if label_display != null:
        label_display.modulate = Color.GREEN if state_signature == "Regulated" else Color.RED


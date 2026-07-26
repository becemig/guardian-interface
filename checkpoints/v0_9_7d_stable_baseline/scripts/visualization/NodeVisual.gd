class_name NodeVisual
extends Node3D

@export var theme_settings: Resource

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
    if theme_settings == null:
        theme_settings = load("res://resources/default_vis_settings.tres")

    if label_display != null and node_id != -1:
        label_display.text = str(node_id)

    _update_visual_state()


func _update_visual_state() -> void:
    if not is_inside_tree():
        return

    if theme_settings == null:
        return

    if label_display != null:
        if state_signature == "Regulated":
            label_display.modulate = theme_settings.color_regulated
        elif state_signature == "Stressed":
            label_display.modulate = theme_settings.color_stressed
        else:
            label_display.modulate = theme_settings.color_default


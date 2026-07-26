class_name NodeStateBinder
extends Node

var somatic_graph: Node = null
var visualization_manager: Node = null


func bind_state_layers(graph: Node, manager: Node) -> void:
    somatic_graph = graph
    visualization_manager = manager

    if visualization_manager != null and visualization_manager.has_signal("visual_frame_updated"):
        if not visualization_manager.visual_frame_updated.is_connected(_on_visual_frame_updated):
            visualization_manager.visual_frame_updated.connect(_on_visual_frame_updated)

    print("[NODE STATE BINDER] Explicit component property binding layer active.")


func _on_visual_frame_updated(summary: Dictionary) -> void:
    if somatic_graph == null:
        return

    if somatic_graph.active_node_registry.size() == 0:
        return

    var global_confidence: float = float(summary.get("confidence", 1.0))
    var current_hrv_value: float = float(summary.get("hrv", 0.0))
    var state_value: String = str(summary.get("guardian_state", "Unknown"))

    for node_id in somatic_graph.active_node_registry.keys():
        var node = somatic_graph.get_node_by_index(int(node_id))

        if node != null:
            node.scale = Vector3.ONE * global_confidence
            node.set("current_hrv", current_hrv_value)
            node.set("state_signature", state_value)


func verify_node_visual_state(node_id: int) -> Dictionary:
    if somatic_graph == null:
        return {}

    var node = somatic_graph.get_node_by_index(node_id)

    if node == null:
        return {}

    return {
        "scale": node.scale,
        "current_hrv": node.get("current_hrv"),
        "state_signature": node.get("state_signature")
    }


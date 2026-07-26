class_name SomaticGraph
extends Node3D

var active_node_registry: Dictionary = {}


func generate_anatomy_graph(meridian_json_path: String = "") -> void:
    var node_data_list: Array = []

    if meridian_json_path != "" and FileAccess.file_exists(meridian_json_path):
        var file := FileAccess.open(meridian_json_path, FileAccess.READ)
        if file:
            var json := JSON.new()
            if json.parse(file.get_as_text()) == OK and json.data is Array:
                node_data_list = json.data

    if node_data_list.size() != 338:
        node_data_list.clear()
        for i in range(338):
            node_data_list.append({
                "id": i,
                "name": "Acupoint_%d" % i,
                "position": Vector3(sin(i) * 5.0, i * 0.05, cos(i) * 5.0)
            })

    _build_scene_topology(node_data_list)


func _build_scene_topology(nodes_array: Array) -> void:
    for child in get_children():
        child.queue_free()

    active_node_registry.clear()

    for node_info in nodes_array:
        var node_id: int = int(node_info.get("id", -1))
        if node_id < 0:
            continue

        var point_node := Node3D.new()
        point_node.name = "Node3D_%d" % node_id

        var pos_data = node_info.get("position", Vector3.ZERO)

        if pos_data is Vector3:
            point_node.position = pos_data
        elif pos_data is Dictionary:
            point_node.position = Vector3(
                float(pos_data.get("x", 0.0)),
                float(pos_data.get("y", 0.0)),
                float(pos_data.get("z", 0.0))
            )
        elif pos_data is Array and pos_data.size() >= 3:
            point_node.position = Vector3(
                float(pos_data[0]),
                float(pos_data[1]),
                float(pos_data[2])
            )

        add_child(point_node)
        active_node_registry[node_id] = point_node

    print("[SOMATIC GRAPH] Static structural layout locked. Node count: ", active_node_registry.size())


func get_node_by_index(index: int) -> Node3D:
    return active_node_registry.get(index, null)

extends Label

func _ready():
    call_deferred("_setup_connections")

func _setup_connections():
    if AtlasSignalBus:
        AtlasSignalBus.telemetry_nodes_updated.connect(_on_nodes_updated)

func _on_nodes_updated(payload: Dictionary):
    var total = payload.get("matched_count", 0)
    var labels = payload.get("top_node_labels", [])
    text = "Total Matches: " + str(total) + "\nTop Priorities:\n- " + "\n- ".join(labels)

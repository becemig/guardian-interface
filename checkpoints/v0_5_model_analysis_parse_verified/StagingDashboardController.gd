extends HBoxContainer

var adapter: Node = null
var telemetry_model: Node = null
var cluster_card: Node = null
var influence_graph: Node = null

func _ready():
    print("StagingDashboardController v0.5 Active.")

    cluster_card = get_node_or_null("ClusterCard")
    influence_graph = get_node_or_null("InfluenceGraph")

    print("ClusterCard found: ", cluster_card != null)
    print("InfluenceGraph found: ", influence_graph != null)

    telemetry_model = preload("res://scripts/core/TelemetryModel.gd").new()
    add_child(telemetry_model)

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    add_child(adapter)

    adapter.dashboard_payload_updated.connect(telemetry_model.update_model)
    telemetry_model.model_updated.connect(_on_model_updated)
    telemetry_model.history_updated.connect(_on_history_updated)

func _on_model_updated(snapshot):
    var widget_payload = _convert_snapshot_to_widget_payload(snapshot)

    if cluster_card != null and cluster_card.has_method("_update_card"):
        cluster_card._update_card(widget_payload)

    if influence_graph != null and influence_graph.has_method("_on_data_received"):
        influence_graph._on_data_received(widget_payload)

func _on_history_updated(history_snapshot):
    print("History samples: ", history_snapshot.get("samples", 0))
    print("HRV history: ", history_snapshot.get("hrv", []))

func _convert_snapshot_to_widget_payload(snapshot):
    var state = str(snapshot.get("state", "Unknown"))

    return {
        "active_signal": state,
        "guardian_state": state,
        "cluster_category": "Telemetry Model v0.5",
        "matched_node_count": 4,
        "top_node_labels": [
            "HRV: " + str(snapshot.get("hrv", 0)),
            "Respiration: " + str(snapshot.get("respiration_rate", 0)),
            "Reaction Time: " + str(snapshot.get("reaction_time", 0)),
            "Sleep Quality: " + str(snapshot.get("sleep_quality", 0))
        ]
    }


extends HBoxContainer

var adapter: Node = null
var telemetry_model: Node = null
var cluster_card: Node = null
var influence_graph: Node = null

func _ready():
    print("StagingDashboardController active: Initializing sandboxed model architecture.")

    cluster_card = get_node_or_null("ClusterCard")
    influence_graph = get_node_or_null("InfluenceGraph")

    if cluster_card == null:
        print("Warning: ClusterCard not found.")
    else:
        print("ClusterCard found.")

    if influence_graph == null:
        print("Warning: InfluenceGraph not found.")
    else:
        print("InfluenceGraph found.")

    telemetry_model = preload("res://scripts/core/TelemetryModel.gd").new()
    telemetry_model.name = "TelemetryModel"
    add_child(telemetry_model)
    print("TelemetryModel instantiated as child.")

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    adapter.name = "DashboardBridgeAdapter"
    add_child(adapter)
    print("DashboardBridgeAdapter instantiated as child.")

    adapter.dashboard_payload_updated.connect(_on_adapter_payload)
    telemetry_model.model_updated.connect(_on_model_updated)

func _on_adapter_payload(payload):
    telemetry_model.update_model(payload)

func _on_model_updated(snapshot):
    print("TelemetryModel updated:")
    print(snapshot)

    var graph_payload = _convert_snapshot_to_widget_payload(snapshot)

    if cluster_card != null and cluster_card.has_method("_update_card"):
        cluster_card._update_card(graph_payload)

    if influence_graph != null and influence_graph.has_method("_on_data_received"):
        influence_graph._on_data_received(graph_payload)

func _convert_snapshot_to_widget_payload(snapshot):
    var state = str(snapshot.get("state", "Unknown"))

    return {
        "active_signal": state,
        "guardian_state": state,
        "cluster_category": "Telemetry Model",
        "matched_node_count": 4,
        "top_node_labels": [
            "HRV: " + str(snapshot.get("hrv", 0)),
            "Respiration: " + str(snapshot.get("respiration_rate", 0)),
            "Reaction Time: " + str(snapshot.get("reaction_time", 0)),
            "Sleep Quality: " + str(snapshot.get("sleep_quality", 0))
        ]
    }

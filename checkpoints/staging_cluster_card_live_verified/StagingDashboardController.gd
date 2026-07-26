extends HBoxContainer

var adapter: Node = null
var cluster_card: Node = null

func _ready():
    print("StagingDashboardController active.")

    cluster_card = get_node_or_null("ClusterCard")

    if cluster_card == null:
        print("Warning: ClusterCard not found.")
    else:
        print("ClusterCard found.")

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    add_child(adapter)

    adapter.dashboard_payload_updated.connect(_on_dashboard_payload)

func _on_dashboard_payload(payload):
    print("Routing payload to ClusterCard:")
    print(payload)

    if cluster_card == null:
        return

    var cluster_payload = {
        "active_signal": "HRV",
        "guardian_state": payload.get("state", "Unknown"),
        "cluster_category": "Guardian Bridge",
        "matched_node_count": 4,
        "top_node_labels": [
            "HRV: " + str(payload.get("hrv", 0)),
            "Respiration: " + str(payload.get("respiration_rate", 0)),
            "Reaction Time: " + str(payload.get("reaction_time", 0)),
            "Sleep Quality: " + str(payload.get("sleep_quality", 0))
        ]
    }

    cluster_card._update_card(cluster_payload)

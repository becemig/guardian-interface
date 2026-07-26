extends HBoxContainer

var adapter: Node = null
var cluster_card: Node = null
var influence_graph: Node = null

func _ready():
    print("StagingDashboardController active.")

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

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    add_child(adapter)

    adapter.dashboard_payload_updated.connect(_on_dashboard_payload)

func _on_dashboard_payload(payload):
    print("Routing payload to ClusterCard and InfluenceGraph:")
    print(payload)

    var graph_payload = _convert_to_graph_payload(payload)

    if cluster_card != null:
        if cluster_card.has_method("_update_card"):
            cluster_card._update_card(graph_payload)
        else:
            print("Warning: ClusterCard missing _update_card method.")

    if influence_graph != null:
        if influence_graph.has_method("_on_data_received"):
            influence_graph._on_data_received(graph_payload)
        elif influence_graph.has_method("update_graph"):
            influence_graph.update_graph(graph_payload)
        elif influence_graph.has_method("set_data"):
            influence_graph.set_data(graph_payload)
        else:
            print("Warning: InfluenceGraph has no compatible update method.")

func _convert_to_graph_payload(payload):
    var state = str(payload.get("state", "Unknown"))
    var hrv = payload.get("hrv", 0)
    var respiration = payload.get("respiration_rate", 0)
    var reaction = payload.get("reaction_time", 0)
    var sleep = payload.get("sleep_quality", 0)

    return {
        "active_signal": state,
        "guardian_state": state,
        "cluster_category": "Guardian Bridge",
        "matched_node_count": 4,
        "top_node_labels": [
            "HRV: " + str(hrv),
            "Respiration: " + str(respiration),
            "Reaction Time: " + str(reaction),
            "Sleep Quality: " + str(sleep)
        ]
    }

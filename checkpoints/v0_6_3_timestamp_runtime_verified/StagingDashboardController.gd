extends HBoxContainer

var adapter: Node = null
var telemetry_model: Node = null
var cluster_card: Node = null
var influence_graph: Node = null
var trend_graph: Node = null

func _ready():
    print("StagingDashboardController v0.6 - Calibration Diagnostics Engaged.")

    cluster_card = get_node_or_null("ClusterCard")
    influence_graph = get_node_or_null("InfluenceGraph")
    trend_graph = get_node_or_null("TrendGraph")

    print("ClusterCard found: ", cluster_card != null)
    print("InfluenceGraph found: ", influence_graph != null)
    print("TrendGraph found: ", trend_graph != null)

    telemetry_model = preload("res://scripts/core/TelemetryModel.gd").new()
    add_child(telemetry_model)

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    add_child(adapter)

    adapter.dashboard_payload_updated.connect(telemetry_model.update_model)
    telemetry_model.model_updated.connect(_on_model_updated)
    telemetry_model.history_updated.connect(_on_history_updated)
    telemetry_model.analysis_updated.connect(_on_analysis_updated)
    telemetry_model.baseline_updated.connect(_on_baseline_updated)
    telemetry_model.baseline_progress.connect(_on_baseline_progress)

func _on_model_updated(snapshot):
    var widget_payload = _convert_snapshot_to_widget_payload(snapshot)

    if cluster_card != null and cluster_card.has_method("_update_card"):
        cluster_card._update_card(widget_payload)

    if influence_graph != null and influence_graph.has_method("_on_data_received"):
        influence_graph._on_data_received(widget_payload)

func _on_history_updated(history_snapshot):
    print("History samples: ", history_snapshot.get("samples", 0))
    print("HRV history: ", history_snapshot.get("hrv", []))

    if trend_graph != null and trend_graph.has_method("update_trend"):
        trend_graph.update_trend(history_snapshot)

func _on_analysis_updated(analysis_snapshot):
    var hrv_analysis = analysis_snapshot.get("hrv", {})
    var deviation = analysis_snapshot.get("deviation_percent", {})
    var baseline = analysis_snapshot.get("baseline", {})

    print("--------------------------------")
    print("HRV avg: ", hrv_analysis.get("avg", 0.0))
    print("HRV min/max: ", hrv_analysis.get("min", 0.0), " / ", hrv_analysis.get("max", 0.0))
    print("HRV trend: ", hrv_analysis.get("trend", "No Data"))
    print("HRV samples: ", hrv_analysis.get("samples", 0))

    if baseline.get("is_calibrated", false):
        print("=== v0.6 Biometric Deviation Frame ===")
        print("HRV delta %: ", "%.2f" % deviation.get("hrv_delta_percent", 0.0), "%")
        print("Respiration delta %: ", "%.2f" % deviation.get("respiration_delta_percent", 0.0), "%")
        print("Reaction time delta %: ", "%.2f" % deviation.get("reaction_time_delta_percent", 0.0), "%")
        print("Sleep quality delta %: ", "%.2f" % deviation.get("sleep_quality_delta_percent", 0.0), "%")
        print("======================================")

func _on_baseline_progress(current_count, target_count):
    print("[Calibration Progress] ", current_count, "/", target_count)

func _on_baseline_updated(baseline_snapshot):
    print("[Controller Baseline Received]")
    print(baseline_snapshot)

func _convert_snapshot_to_widget_payload(snapshot):
    var state = str(snapshot.get("state", "Unknown"))

    return {
        "active_signal": state,
        "guardian_state": state,
        "cluster_category": "Telemetry Model v0.6",
        "matched_node_count": 4,
        "top_node_labels": [
            "HRV: " + str(snapshot.get("hrv", 0)),
            "Respiration: " + str(snapshot.get("respiration_rate", 0)),
            "Reaction Time: " + str(snapshot.get("reaction_time", 0)),
            "Sleep Quality: " + str(snapshot.get("sleep_quality", 0))
        ]
    }


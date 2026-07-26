class_name TelemetryDashboard
extends PanelContainer

var label_session_id: Label = null
var label_participant_id: Label = null
var label_sample_num: Label = null
var label_guardian_state: Label = null
var label_confidence: Label = null
var label_current_hrv: Label = null
var label_avg_hrv: Label = null
var label_current_resp: Label = null
var label_avg_resp: Label = null

func _cache_nodes() -> void:
    label_session_id = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_SessionID_Value")
    label_participant_id = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_ParticipantID_Value")
    label_sample_num = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_SampleNum_Value")
    label_guardian_state = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_GuardianState_Value")
    label_confidence = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_Confidence_Value")
    label_current_hrv = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_CurrentHRV_Value")
    label_avg_hrv = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_AvgHRV_Value")
    label_current_resp = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_CurrentResp_Value")
    label_avg_resp = get_node_or_null("MarginContainer/VBoxContainer/GridContainer/Label_AvgResp_Value")


func bind_dashboard_targets(telemetry_model: Node, replay_controller: Node = null) -> void:
    _cache_nodes()
    if telemetry_model != null and telemetry_model.has_signal("dashboard_data_updated"):
        telemetry_model.dashboard_data_updated.connect(_on_dashboard_data_updated)

func _on_dashboard_data_updated(summary: Dictionary) -> void:
    _cache_nodes()
    label_session_id.text = str(summary.get("session_id", "N/A"))
    label_participant_id.text = str(summary.get("participant_id", "N/A"))
    label_sample_num.text = str(summary.get("sample_index", 0))
    label_guardian_state.text = str(summary.get("guardian_state", "Unknown"))
    label_confidence.text = "%0.2f" % float(summary.get("confidence", 0.0))
    label_current_hrv.text = "%0.1f bpm" % float(summary.get("hrv", 0.0))
    label_avg_hrv.text = "%0.1f bpm" % float(summary.get("avg_hrv", 0.0))
    label_current_resp.text = "%0.1f/m" % float(summary.get("respiration_rate", 0.0))
    label_avg_resp.text = "%0.1f/m" % float(summary.get("avg_respiration", 0.0))

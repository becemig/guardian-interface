extends PanelContainer

@onready var title_label = $VBoxContainer/TitleLabel
@onready var state_label = $VBoxContainer/StateLabel
@onready var stats_label = $VBoxContainer/StatsLabel
@onready var nodes_label = $VBoxContainer/NodesLabel

var connected_to_telemetry = false

func _ready():
	title_label.text = "Waiting for telemetry..."
	state_label.text = "Guardian State: Unknown"
	stats_label.text = ""
	nodes_label.text = ""

	if not connected_to_telemetry:
		TelemetryController.telemetry_payload_updated.connect(_update_card)
		connected_to_telemetry = true

func _update_card(payload):
	var active_signal = str(payload.get("active_signal", "unknown"))
	var matched_count = int(payload.get("matched_count", 0))
	var top_nodes = payload.get("top_node_labels", [])
	var guardian_state = str(payload.get("guardian_state", _derive_guardian_state(payload)))
	var category = _get_cluster_category(top_nodes)

	title_label.text = "Active Signal: " + active_signal.to_upper()
	state_label.text = "Guardian State: " + guardian_state
	stats_label.text = "Cluster: " + category + "\nMatched Nodes: " + str(matched_count)

	if top_nodes.size() > 0:
		nodes_label.text = "Top Nodes:\n- " + "\n- ".join(top_nodes)
	else:
		nodes_label.text = "Top Nodes:\nNone"

func _get_cluster_category(nodes):
	var stress_score = 0
	var sleep_score = 0
	var autonomic_score = 0

	for n in nodes:
		var text = str(n).to_lower()

		if "stress" in text or "strain" in text:
			stress_score += 1

		if "sleep" in text or "wave" in text or "recovery" in text or "rem" in text:
			sleep_score += 1

		if "autonomic" in text or "vagal" in text or "vagus" in text or "regulation" in text or "breathing" in text:
			autonomic_score += 1

	if sleep_score >= stress_score and sleep_score >= autonomic_score:
		return "Sleep/Recovery"

	if autonomic_score >= stress_score:
		return "Autonomic"

	if stress_score > 0:
		return "Stress/Strain"

	return "General/Mixed"

func _derive_guardian_state(payload):
	var hrv = float(payload.get("hrv", -1.0))
	var respiration = float(payload.get("respiration_rate", -1.0))
	var reaction = float(payload.get("reaction_time", -1.0))
	var sleep = float(payload.get("sleep_quality", -1.0))

	if sleep >= 0.0 and sleep < 0.35:
		return "Fatigued"

	if hrv >= 0.70 and respiration > 0.0 and respiration <= 14.0:
		return "Regulated"

	if respiration >= 18.0:
		return "Stress Load"

	if reaction >= 0.60:
		return "Cognitive Load"

	if hrv >= 0.55:
		return "Adaptive"

	return "Monitoring"

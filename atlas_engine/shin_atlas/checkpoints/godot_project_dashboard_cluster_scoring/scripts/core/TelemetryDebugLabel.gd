extends Label

func _ready():
	text = "Waiting for telemetry..."
	# Ensure TelemetryController is ready before connecting
	if TelemetryController:
		TelemetryController.telemetry_payload_updated.connect(_on_payload)
	else:
		text = "Error: TelemetryController not found."

func _on_payload(payload: Dictionary):
	var active_signal = payload.get("active_signal", "unknown")
	var count = payload.get("matched_count", 0)
	var labels = payload.get("top_node_labels", [])

	text = "Signal: %s\nMatches: %s\nTop Nodes:\n%s" % [
		active_signal,
		str(count),
		"\n".join(labels)
	]

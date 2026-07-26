extends Node

signal concept_influence_updated(payload)
signal telemetry_nodes_updated(payload)

func update_influence(concept_name: String, intensity: float):
	var payload := {
		"name": concept_name,
		"intensity": intensity
	}
	emit_signal("concept_influence_updated", payload)

func update_telemetry_nodes(payload: Dictionary):
	emit_signal("telemetry_nodes_updated", payload)

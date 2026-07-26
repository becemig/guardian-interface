extends Node

signal concept_influence_updated(payload)
signal telemetry_nodes_updated(payload)
signal somatic_node_queried(node_id, uids)

func update_influence(concept_name: String, intensity: float):
	var payload := {
		"name": concept_name,
		"intensity": intensity
	}
	emit_signal("concept_influence_updated", payload)

func update_telemetry_nodes(payload: Dictionary):
	emit_signal("telemetry_nodes_updated", payload)

func emit_somatic_query(node_id: int, uids: Array):
	emit_signal("somatic_node_queried", node_id, uids)

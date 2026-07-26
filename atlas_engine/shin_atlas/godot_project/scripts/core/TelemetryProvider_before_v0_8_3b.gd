class_name TelemetryProvider
extends Node

func csv_self_test() -> bool:
	push_error("CRITICAL: csv_self_test() not implemented.")
	return false

func initialize_new_session(new_participant_id: String = "anonymous") -> void:
	push_error("CRITICAL: initialize_new_session() not implemented.")

func flush_buffer_to_disk() -> void:
	push_error("CRITICAL: flush_buffer_to_disk() not implemented.")

func is_active() -> bool:
	return false

func get_session_id() -> String:
	return ""

func get_participant_id() -> String:
	return ""

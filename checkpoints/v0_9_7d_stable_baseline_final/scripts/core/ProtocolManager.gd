extends Node

signal protocol_phase_changed(protocol_snapshot)

var active_protocol: Dictionary = {
	"name": "Shin_PSY497_Alpha",
	"phases": [
		{"name": "Initialization", "target_duration": 0.0},
		{"name": "Resting Baseline", "target_duration": 60.0},
		{"name": "Cognitive Stress", "target_duration": 180.0},
		{"name": "Post-Stress Recovery", "target_duration": 120.0},
		{"name": "Somatic Flow", "target_duration": 300.0}
	]
}

var current_protocol_name: String = "UNKNOWN"
var current_phase_name: String = "UNKNOWN"
var phase_index: int = 0
var phase_start_unix: float = 0.0
var phase_elapsed_seconds: float = 0.0
var phase_target_duration: float = 0.0
var phase_completion_percent: float = 0.0

func _ready() -> void:
	load_protocol(active_protocol)

func _process(_delta: float) -> void:
	if phase_start_unix > 0.0:
		phase_elapsed_seconds = Time.get_unix_time_from_system() - phase_start_unix

		if phase_target_duration > 0.0:
			phase_completion_percent = clamp(
				(phase_elapsed_seconds / phase_target_duration) * 100.0,
				0.0,
				100.0
			)
		else:
			phase_completion_percent = 0.0

func load_protocol(protocol_dict: Dictionary) -> void:
	active_protocol = protocol_dict
	current_protocol_name = active_protocol.get("name", "Unnamed_Protocol")
	phase_index = 0
	_apply_current_phase_state()
	print("[Protocol Setup] Loaded Manifest: ", current_protocol_name)

func _apply_current_phase_state() -> void:
	var phases_list: Array = active_protocol.get("phases", [])

	if phases_list.size() == 0:
		push_error("[Protocol Error] Active protocol has no phases.")
		return

	if phase_index < 0 or phase_index >= phases_list.size():
		push_error("[Protocol Error] Phase index out of range.")
		return

	var phase_data: Dictionary = phases_list[phase_index]

	current_phase_name = phase_data.get("name", "Unknown_Phase")
	phase_target_duration = float(phase_data.get("target_duration", 0.0))
	phase_start_unix = Time.get_unix_time_from_system()
	phase_elapsed_seconds = 0.0
	phase_completion_percent = 0.0

	print("[Protocol Event] Phase Triggered -> [%d] %s (Target: %.1fs)" % [
		phase_index,
		current_phase_name,
		phase_target_duration
	])

	protocol_phase_changed.emit(get_protocol_snapshot())

func advance_phase() -> void:
	var phases_list: Array = active_protocol.get("phases", [])

	if phase_index < phases_list.size() - 1:
		phase_index += 1
		_apply_current_phase_state()
	else:
		print("[Protocol Event] Terminal experimental phase reached. Manifest execution complete.")

func set_phase(new_phase_name: String) -> void:
	var phases_list: Array = active_protocol.get("phases", [])

	for i in range(phases_list.size()):
		var phase_data: Dictionary = phases_list[i]
		if phase_data.get("name", "") == new_phase_name:
			phase_index = i
			_apply_current_phase_state()
			return

	push_error("[Protocol Error] Phase '%s' not recognized." % new_phase_name)

func get_protocol_snapshot() -> Dictionary:
	return {
		"protocol_name": current_protocol_name,
		"phase_name": current_phase_name,
		"phase_index": phase_index,
		"phase_start_unix": phase_start_unix,
		"phase_elapsed_seconds": phase_elapsed_seconds,
		"phase_target_duration": phase_target_duration,
		"phase_completion_percent": phase_completion_percent
	}

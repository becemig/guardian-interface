extends Node
class_name ProtocolManager

signal protocol_phase_changed(protocol_snapshot)

var current_protocol_name: String = "Shin_PSY497_Alpha"
var current_phase_name: String = "Initialization"
var phase_index: int = 0
var phase_start_unix: float = 0.0
var phase_elapsed_seconds: float = 0.0

var protocol_phases: Array[String] = [
	"Initialization",
	"Resting Baseline",
	"Cognitive Stress",
	"Post-Stress Recovery",
	"Somatic Flow"
]

func _ready() -> void:
	reset_protocol_state()

func _process(_delta: float) -> void:
	if phase_start_unix > 0.0:
		phase_elapsed_seconds = Time.get_unix_time_from_system() - phase_start_unix

func reset_protocol_state() -> void:
	phase_index = 0
	current_phase_name = protocol_phases[phase_index]
	phase_start_unix = Time.get_unix_time_from_system()
	phase_elapsed_seconds = 0.0
	print("[Protocol Started]")
	print(get_protocol_snapshot())
	protocol_phase_changed.emit(get_protocol_snapshot())

func set_phase(new_phase_name: String) -> void:
	if new_phase_name in protocol_phases:
		phase_index = protocol_phases.find(new_phase_name)
		current_phase_name = new_phase_name
		phase_start_unix = Time.get_unix_time_from_system()
		phase_elapsed_seconds = 0.0
		print("[Protocol Event] Explicit Phase Shift -> ", current_phase_name)
		protocol_phase_changed.emit(get_protocol_snapshot())
	else:
		push_error("[Protocol Error] Phase '%s' not recognized." % new_phase_name)

func advance_phase() -> void:
	if phase_index < protocol_phases.size() - 1:
		phase_index += 1
		current_phase_name = protocol_phases[phase_index]
		phase_start_unix = Time.get_unix_time_from_system()
		phase_elapsed_seconds = 0.0
		print("[Protocol Event] Advanced Phase -> [%d] %s" % [phase_index, current_phase_name])
		protocol_phase_changed.emit(get_protocol_snapshot())
	else:
		print("[Protocol Event] Terminal phase reached. Cannot advance further.")

func get_protocol_snapshot() -> Dictionary:
	return {
		"protocol_name": current_protocol_name,
		"phase_name": current_phase_name,
		"phase_index": phase_index,
		"phase_start_unix": phase_start_unix,
		"phase_elapsed_seconds": phase_elapsed_seconds
	}

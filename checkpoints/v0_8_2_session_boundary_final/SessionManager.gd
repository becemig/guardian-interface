extends Node
# SessionManager.gd - Pure Orchestrator (v0.8.2)

signal session_started(session_snapshot)
signal session_failed(reason)
signal session_state_changed(new_state)

enum SessionState { IDLE, INITIALIZING, RECORDING, PAUSED, FAULT }

var current_state: SessionState = SessionState.IDLE
var telemetry_model: Node = null


func bind_telemetry_model(model: Node) -> void:
	telemetry_model = model
	print("[SessionManager] TelemetryModel bound successfully.")


func request_start_session(participant_id: String = "anonymous") -> bool:
	if current_state != SessionState.IDLE:
		var reason := "Cannot start session unless SessionManager is IDLE."
		push_warning(reason)
		emit_signal("session_failed", reason)
		return false

	_set_state(SessionState.INITIALIZING)

	if telemetry_model == null:
		var reason := "TelemetryModel not bound. Session start aborted."
		push_error(reason)
		_set_state(SessionState.FAULT)
		emit_signal("session_failed", reason)
		return false

	if not telemetry_model.csv_self_test():
		var reason := "TelemetryModel CSV self-test failed. Guarding schema integrity."
		push_error(reason)
		_set_state(SessionState.FAULT)
		emit_signal("session_failed", reason)
		return false

	telemetry_model.initialize_new_session(participant_id)

	_set_state(SessionState.RECORDING)

	emit_signal("session_started", {
		"participant_id": telemetry_model.participant_id,
		"session_id": telemetry_model.current_session_id
	})

	return true


func request_end_session() -> void:
	if current_state != SessionState.RECORDING and current_state != SessionState.PAUSED:
		push_warning("No active session running to terminate.")
		return

	if telemetry_model != null:
		telemetry_model.is_session_active = false

	_set_state(SessionState.IDLE)
	print("[SessionManager] Session cleanly closed out.")


func pause_session() -> void:
	if current_state != SessionState.RECORDING:
		return
	_set_state(SessionState.PAUSED)


func resume_session() -> void:
	if current_state != SessionState.PAUSED:
		return
	_set_state(SessionState.RECORDING)


func _set_state(new_state: SessionState) -> void:
	if current_state == new_state:
		return

	current_state = new_state
	print("[SessionManager] State changed: ", SessionState.keys()[new_state])
	emit_signal("session_state_changed", current_state)

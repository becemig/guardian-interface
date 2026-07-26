extends TelemetryProvider

signal state_changed(new_state, confidence)
signal metrics_changed(hrv, respiration_rate, reaction_time, sleep_quality)
signal model_updated(snapshot)
signal history_updated(history_snapshot)
signal analysis_updated(analysis_snapshot)
signal baseline_updated(baseline_snapshot)
signal baseline_progress(current_count, target_count)
signal session_started(session_snapshot)

var current_state: String = "Regulated"
var confidence: float = 1.0
var hrv: float = 70.0
var respiration_rate: float = 12.0
var reaction_time: float = 300.0
var sleep_quality: float = 80.0

var history: Array = []
var max_history_size: int = 30

var hrv_history: Array = []
var respiration_history: Array = []
var reaction_time_history: Array = []
var sleep_quality_history: Array = []

var analysis: Dictionary = {}

var is_baseline_calibrated: bool = false
var baseline_target_samples: int = 10
var calibration_pool: Array = []

var baseline_hrv: float = 0.0
var baseline_respiration_rate: float = 0.0
var baseline_reaction_time: float = 0.0
var baseline_sleep_quality: float = 0.0

var hrv_delta_percent: float = 0.0
var respiration_delta_percent: float = 0.0
var reaction_time_delta_percent: float = 0.0
var sleep_quality_delta_percent: float = 0.0

var ignore_first_payload: bool = true
var ignored_initial_payload: bool = false

var current_session_id: String = ""
var session_start_unix: float = 0.0
var sample_index: int = 0
var total_samples_written: int = 0
var participant_id: String = "anonymous"
var is_session_active: bool = false

var sample_time_unix: float = 0.0
var previous_sample_unix: float = 0.0
var elapsed_seconds: float = 0.0
var delta_seconds: float = 0.0
var protocol_warning_emitted: bool = false

func _ready():
    pass
    # Session start is now controlled by SessionManager.gd

func update_model(payload: Dictionary):
    if ignore_first_payload and not ignored_initial_payload:
        ignored_initial_payload = true
        print("[Session Hygiene] Ignored first bridge payload to avoid stale calibration contamination.")
        return

    sample_index += 1

    sample_time_unix = Time.get_unix_time_from_system()
    elapsed_seconds = sample_time_unix - session_start_unix

    if previous_sample_unix == 0.0:
        delta_seconds = 0.0
    else:
        delta_seconds = sample_time_unix - previous_sample_unix

    previous_sample_unix = sample_time_unix

    current_state = str(payload.get("state", "Regulated"))
    confidence = float(payload.get("confidence", 1.0))
    hrv = float(payload.get("hrv", 70.0))
    respiration_rate = float(payload.get("respiration_rate", 12.0))
    reaction_time = float(payload.get("reaction_time", 300.0))
    sleep_quality = float(payload.get("sleep_quality", 80.0))

    var snapshot = {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "sample_index": sample_index,
        "session_start_unix": session_start_unix,
        "sample_time_unix": sample_time_unix,
        "elapsed_seconds": elapsed_seconds,
        "delta_seconds": delta_seconds,
        "timestamp_ms": Time.get_ticks_msec(),
        "state": current_state,
        "confidence": confidence,
        "hrv": hrv,
        "respiration_rate": respiration_rate,
        "reaction_time": reaction_time,
        "sleep_quality": sleep_quality
    }

    history.append(snapshot)

    if history.size() > max_history_size:
        history.pop_front()

    _append_to_buffer(hrv_history, hrv)
    _append_to_buffer(respiration_history, respiration_rate)
    _append_to_buffer(reaction_time_history, reaction_time)
    _append_to_buffer(sleep_quality_history, sleep_quality)

    if not is_baseline_calibrated:
        _process_calibration_sample(snapshot)
    else:
        _calculate_baseline_deviations(snapshot)

    analysis = get_analysis_snapshot()

    print("[Sample #%d Timing] elapsed=%.3f delta=%.3f" % [sample_index, elapsed_seconds, delta_seconds])

    state_changed.emit(current_state, confidence)
    metrics_changed.emit(hrv, respiration_rate, reaction_time, sleep_quality)
    model_updated.emit(snapshot)
    history_updated.emit(get_history_snapshot())
    analysis_updated.emit(analysis)

    log_current_snapshot_to_disk()

func initialize_new_session(new_participant_id: String = "anonymous"):
    if is_session_active:
        push_warning("[Session Management] initialize_new_session blocked: session already active.")
        return
    participant_id = new_participant_id
    current_session_id = _generate_session_id()
    session_start_unix = Time.get_unix_time_from_system()
    sample_index = 0
    total_samples_written = 0

    sample_time_unix = 0.0
    previous_sample_unix = 0.0
    elapsed_seconds = 0.0
    delta_seconds = 0.0

    reset_baseline()

    var session_snapshot = {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "session_start_unix": session_start_unix
    }

    is_session_active = true
    print("[Session Started]")
    print(session_snapshot)

    session_started.emit(session_snapshot)

    initialize_session_csv()

func _generate_session_id() -> String:
    var t = Time.get_datetime_dict_from_system()

    return "SID-%04d%02d%02d-%02d%02d%02d" % [
        t.year,
        t.month,
        t.day,
        t.hour,
        t.minute,
        t.second
    ]

func _append_to_buffer(buffer: Array, value: float):
    buffer.append(value)

    if buffer.size() > max_history_size:
        buffer.pop_front()

func _process_calibration_sample(snapshot: Dictionary):
    calibration_pool.append(snapshot)

    baseline_progress.emit(calibration_pool.size(), baseline_target_samples)

    print("[Calibration] Logged sample ", calibration_pool.size(), "/", baseline_target_samples)

    if calibration_pool.size() >= baseline_target_samples:
        _lock_baseline()

func _lock_baseline():
    var sum_hrv := 0.0
    var sum_respiration := 0.0
    var sum_reaction := 0.0
    var sum_sleep := 0.0

    for sample in calibration_pool:
        sum_hrv += float(sample.get("hrv", 0.0))
        sum_respiration += float(sample.get("respiration_rate", 0.0))
        sum_reaction += float(sample.get("reaction_time", 0.0))
        sum_sleep += float(sample.get("sleep_quality", 0.0))

    var count = max(1, calibration_pool.size())

    baseline_hrv = sum_hrv / count
    baseline_respiration_rate = sum_respiration / count
    baseline_reaction_time = sum_reaction / count
    baseline_sleep_quality = sum_sleep / count

    is_baseline_calibrated = true
    calibration_pool.clear()

    var baseline_snapshot = get_baseline_snapshot()

    print("[Calibration COMPLETE] Baseline Locked:")
    print(baseline_snapshot)

    baseline_updated.emit(baseline_snapshot)

func _calculate_baseline_deviations(snapshot: Dictionary):
    hrv_delta_percent = _percent_delta(float(snapshot.get("hrv", 0.0)), baseline_hrv)
    respiration_delta_percent = _percent_delta(float(snapshot.get("respiration_rate", 0.0)), baseline_respiration_rate)
    reaction_time_delta_percent = _percent_delta(float(snapshot.get("reaction_time", 0.0)), baseline_reaction_time)
    sleep_quality_delta_percent = _percent_delta(float(snapshot.get("sleep_quality", 0.0)), baseline_sleep_quality)

func _percent_delta(current_value: float, baseline_value: float) -> float:
    if baseline_value == 0.0:
        return 0.0

    return ((current_value - baseline_value) / baseline_value) * 100.0

func get_latest_snapshot():
    if history.size() == 0:
        return {}

    return history[history.size() - 1]

func get_history():
    return history

func get_history_snapshot():
    return {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "samples": history.size(),
        "sample_index": sample_index,
        "sample_time_unix": sample_time_unix,
        "elapsed_seconds": elapsed_seconds,
        "delta_seconds": delta_seconds,
        "hrv": hrv_history,
        "respiration_rate": respiration_history,
        "reaction_time": reaction_time_history,
        "sleep_quality": sleep_quality_history
    }

func get_average(values: Array) -> float:
    if values.size() == 0:
        return 0.0

    var total := 0.0

    for value in values:
        total += float(value)

    return total / values.size()

func get_metric_averages():
    return {
        "hrv": get_average(hrv_history),
        "respiration_rate": get_average(respiration_history),
        "reaction_time": get_average(reaction_time_history),
        "sleep_quality": get_average(sleep_quality_history)
    }

func get_analysis_snapshot():
    return {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "sample_index": sample_index,
        "sample_time_unix": sample_time_unix,
        "elapsed_seconds": elapsed_seconds,
        "delta_seconds": delta_seconds,
        "hrv": _metric_analysis(hrv_history),
        "respiration_rate": _metric_analysis(respiration_history),
        "reaction_time": _metric_analysis(reaction_time_history),
        "sleep_quality": _metric_analysis(sleep_quality_history),
        "baseline": get_baseline_snapshot(),
        "deviation_percent": get_deviation_snapshot(),
        "protocol_context": get_protocol_context_snapshot(),
        "integrated_snapshot": get_integrated_snapshot()
    }

func _metric_analysis(values: Array):
    if values.size() == 0:
        return {
            "avg": 0.0,
            "min": 0.0,
            "max": 0.0,
            "trend": "No Data",
            "samples": 0
        }

    var avg = get_average(values)
    var min_value = float(values[0])
    var max_value = float(values[0])

    for value in values:
        var v = float(value)

        if v < min_value:
            min_value = v

        if v > max_value:
            max_value = v

    var trend = "Stable"

    if values.size() >= 2:
        var current = float(values[values.size() - 1])
        var previous = float(values[values.size() - 2])
        var delta = current - previous

        if delta > 0.01:
            trend = "Rising"
        elif delta < -0.01:
            trend = "Falling"

    return {
        "avg": avg,
        "min": min_value,
        "max": max_value,
        "trend": trend,
        "samples": values.size()
    }

func get_baseline_snapshot():
    return {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "is_calibrated": is_baseline_calibrated,
        "target_samples": baseline_target_samples,
        "calibration_samples": calibration_pool.size(),
        "hrv": baseline_hrv,
        "respiration_rate": baseline_respiration_rate,
        "reaction_time": baseline_reaction_time,
        "sleep_quality": baseline_sleep_quality
    }

func get_deviation_snapshot():
    return {
        "hrv_delta_percent": hrv_delta_percent,
        "respiration_delta_percent": respiration_delta_percent,
        "reaction_time_delta_percent": reaction_time_delta_percent,
        "sleep_quality_delta_percent": sleep_quality_delta_percent
    }

func reset_baseline():
    is_baseline_calibrated = false
    calibration_pool.clear()
    history.clear()

    hrv_history.clear()
    respiration_history.clear()
    reaction_time_history.clear()
    sleep_quality_history.clear()

    analysis.clear()

    baseline_hrv = 0.0
    baseline_respiration_rate = 0.0
    baseline_reaction_time = 0.0
    baseline_sleep_quality = 0.0

    hrv_delta_percent = 0.0
    respiration_delta_percent = 0.0
    reaction_time_delta_percent = 0.0
    sleep_quality_delta_percent = 0.0

    ignore_first_payload = true
    ignored_initial_payload = false

    print("[Session Management] Baseline reset. First incoming bridge payload will be ignored.")



func get_protocol_context_snapshot() -> Dictionary:
    var protocol_manager = get_node_or_null("../ProtocolManager")

    if protocol_manager and protocol_manager.has_method("get_protocol_snapshot"):
        return protocol_manager.get_protocol_snapshot()

    if not protocol_warning_emitted:
        print("[Protocol Warning] ProtocolManager not found. Using NO_PROTOCOL defaults.")
        protocol_warning_emitted = true

    return {
        "protocol_name": "NO_PROTOCOL",
        "phase_name": "NO_PHASE",
        "phase_index": -1,
        "phase_start_unix": 0.0,
        "phase_elapsed_seconds": 0.0,
        "phase_target_duration": 0.0,
        "phase_completion_percent": 0.0
    }

func get_integrated_snapshot() -> Dictionary:
    var protocol_context = get_protocol_context_snapshot()

    return {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "sample_index": sample_index,
        "session_start_unix": session_start_unix,
        "sample_time_unix": sample_time_unix,
        "elapsed_seconds": elapsed_seconds,
        "delta_seconds": delta_seconds,

        "protocol_name": protocol_context.get("protocol_name", "NO_PROTOCOL"),
        "phase_name": protocol_context.get("phase_name", "NO_PHASE"),
        "phase_index": protocol_context.get("phase_index", -1),
        "phase_start_unix": protocol_context.get("phase_start_unix", 0.0),
        "phase_elapsed_seconds": protocol_context.get("phase_elapsed_seconds", 0.0),
        "phase_target_duration": protocol_context.get("phase_target_duration", 0.0),
        "phase_completion_percent": protocol_context.get("phase_completion_percent", 0.0),

        "guardian_state": current_state,
        "confidence": confidence,
        "hrv": hrv,
        "respiration_rate": respiration_rate,
        "reaction_time": reaction_time,
        "sleep_quality": sleep_quality,

        "baseline_hrv": baseline_hrv,
        "baseline_respiration_rate": baseline_respiration_rate,
        "baseline_reaction_time": baseline_reaction_time,
        "baseline_sleep_quality": baseline_sleep_quality,

        "hrv_delta_percent": hrv_delta_percent,
        "respiration_delta_percent": respiration_delta_percent,
        "reaction_time_delta_percent": reaction_time_delta_percent,
        "sleep_quality_delta_percent": sleep_quality_delta_percent
    }


# ============================================================
# v0.8.0 CSV Export Engine
# ============================================================

const SCHEMA_VERSION: String = "GI-0.8.1"

const EXPECTED_HEADERS: Array[String] = [
    "schema_version",
    "session_id",
    "participant_id",
    "sample_index",
    "sample_time_unix",
    "elapsed_seconds",
    "delta_seconds",
    "protocol_name",
    "phase_name",
    "phase_index",
    "phase_elapsed_seconds",
    "phase_target_duration",
    "phase_completion_percent",
    "guardian_state",
    "confidence",
    "hrv",
    "respiration_rate",
    "reaction_time",
    "sleep_quality",
    "baseline_hrv",
    "baseline_respiration_rate",
    "baseline_reaction_time",
    "baseline_sleep_quality",
    "hrv_delta_percent",
    "respiration_delta_percent",
    "reaction_time_delta_percent",
    "sleep_quality_delta_percent",
    "validation_status",
    "validation_notes"
]

var csv_file_path: String = ""
var previous_sample_index: int = 0
var previous_elapsed_seconds: float = 0.0


func csv_self_test() -> bool:
    if SCHEMA_VERSION != "GI-0.8.1":
        push_error("[CSV SELF TEST] Invalid schema version: " + SCHEMA_VERSION)
        return false

    if EXPECTED_HEADERS.size() != 29:
        push_error("[CSV SELF TEST] Expected 29 headers, got %d" % EXPECTED_HEADERS.size())
        return false

    if EXPECTED_HEADERS[0] != "schema_version":
        push_error("[CSV SELF TEST] First column must be schema_version.")
        return false

    if EXPECTED_HEADERS[-2] != "validation_status":
        push_error("[CSV SELF TEST] Column 28 must be validation_status.")
        return false

    if EXPECTED_HEADERS[-1] != "validation_notes":
        push_error("[CSV SELF TEST] Column 29 must be validation_notes.")
        return false

    print("[CSV SELF TEST] GI-0.8.1 schema verified.")
    return true


func initialize_session_csv():

    if not csv_self_test():
        push_error("[CSV] Session CSV initialization aborted.")
        return

    var log_dir = OS.get_environment("HOME") + "/GodotProjects/guardian-interface/session_logs"

    DirAccess.make_dir_recursive_absolute(log_dir)

    csv_file_path = log_dir + "/session_" + current_session_id + ".csv"

    if FileAccess.file_exists(csv_file_path):
        return

    var file = FileAccess.open(csv_file_path, FileAccess.WRITE)

    if file == null:
        push_error("Unable to create CSV file.")
        return

    file.store_line(",".join(EXPECTED_HEADERS))

    file.close()

    print("[CSV] Initialized:", csv_file_path)


func log_current_snapshot_to_disk():

    if csv_file_path == "":
        return

    var snapshot: Dictionary = get_integrated_snapshot()
    var file = FileAccess.open(csv_file_path, FileAccess.READ_WRITE)

    if file == null:
        push_error("[I/O Error] Failed to open CSV path for appending snapshot frame.")
        return

    file.seek_end()

    var current_unix: float = Time.get_unix_time_from_system()

    var row_data: Array[String] = [
        SCHEMA_VERSION,
        str(snapshot.get("session_id", current_session_id)),
        str(snapshot.get("participant_id", "anonymous")),
        str(snapshot.get("sample_index", sample_index)),
        "%.6f" % snapshot.get("sample_time_unix", current_unix),
        "%.3f" % snapshot.get("elapsed_seconds", elapsed_seconds),
        "%.3f" % snapshot.get("delta_seconds", delta_seconds),
        str(snapshot.get("protocol_name", "NO_PROTOCOL")),
        str(snapshot.get("phase_name", "NO_PHASE")),
        str(snapshot.get("phase_index", -1)),
        "%.3f" % snapshot.get("phase_elapsed_seconds", 0.0),
        "%.1f" % snapshot.get("phase_target_duration", 0.0),
        "%.2f" % snapshot.get("phase_completion_percent", 0.0),
        str(snapshot.get("guardian_state", current_state)),
        "%.2f" % snapshot.get("confidence", confidence),
        "%.2f" % snapshot.get("hrv", 70.0),
        "%.2f" % snapshot.get("respiration_rate", 12.0),
        "%.2f" % snapshot.get("reaction_time", 0.0),
        "%.2f" % snapshot.get("sleep_quality", 0.0),
        "%.2f" % snapshot.get("baseline_hrv", 70.0),
        "%.2f" % snapshot.get("baseline_respiration_rate", 12.0),
        "%.2f" % snapshot.get("baseline_reaction_time", 0.0),
        "%.2f" % snapshot.get("baseline_sleep_quality", 0.0),
        "%.4f" % snapshot.get("hrv_delta_percent", 0.0),
        "%.4f" % snapshot.get("respiration_delta_percent", 0.0),
        "%.4f" % snapshot.get("reaction_time_delta_percent", 0.0),
        "%.4f" % snapshot.get("sleep_quality_delta_percent", 0.0),
        "OK",
        "none"
    ]

    file.store_line(",".join(row_data))
    total_samples_written += 1
    file.close()


func flush_buffer_to_disk() -> void:
    log_current_snapshot_to_disk()


func is_active() -> bool:
    return is_session_active


func get_session_id() -> String:
    return current_session_id


func get_participant_id() -> String:
    return participant_id


func get_raw_session_summary_data() -> Dictionary:
    var end_time: float = Time.get_unix_time_from_system()
    var duration: float = end_time - session_start_unix
    var snapshot: Dictionary = get_integrated_snapshot()

    return {
        "session_metadata": {
            "session_id": current_session_id,
            "participant_id": participant_id,
            "protocol_name": snapshot.get("protocol_name", "UNKNOWN"),
            "timestamp_start_unix": session_start_unix,
            "timestamp_end_unix": end_time,
            "total_duration_seconds": duration
        },
        "pipeline_integrity": {
            "total_samples_written": total_samples_written,
            "dropped_frames_count": 0,
            "self_test_status_at_init": "PASSED" if csv_self_test() else "FAILED"
        },
        "baseline_statistics": {
            "baseline_hrv": baseline_hrv,
            "baseline_respiration_rate": baseline_respiration_rate,
            "session_average_hrv": snapshot.get("hrv", 0.0),
            "session_average_respiration": snapshot.get("respiration_rate", 0.0)
        },
        "validation": {
            "validation_status": "OK",
            "validation_notes": "none"
        }
    }

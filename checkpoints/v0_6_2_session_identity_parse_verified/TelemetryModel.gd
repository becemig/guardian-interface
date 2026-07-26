extends Node

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
var participant_id: String = "anonymous"

func _ready():
    initialize_new_session("anonymous")

func update_model(payload: Dictionary):
    if ignore_first_payload and not ignored_initial_payload:
        ignored_initial_payload = true
        print("[Session Hygiene] Ignored first bridge payload to avoid stale calibration contamination.")
        return

    sample_index += 1

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

    state_changed.emit(current_state, confidence)
    metrics_changed.emit(hrv, respiration_rate, reaction_time, sleep_quality)
    model_updated.emit(snapshot)
    history_updated.emit(get_history_snapshot())
    analysis_updated.emit(analysis)

func initialize_new_session(new_participant_id: String = "anonymous"):
    participant_id = new_participant_id
    current_session_id = _generate_session_id()
    session_start_unix = Time.get_unix_time_from_system()
    sample_index = 0

    reset_baseline()

    var session_snapshot = {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "session_start_unix": session_start_unix
    }

    print("[Session Started]")
    print(session_snapshot)

    session_started.emit(session_snapshot)

func _generate_session_id() -> String:
    var unix_time = int(Time.get_unix_time_from_system())
    var ticks = Time.get_ticks_msec()
    return "SID-" + str(unix_time) + "-" + str(ticks)

func _append_to_buffer(buffer: Array, value: float):
    buffer.append(value)

    if buffer.size() > max_history_size:
        buffer.pop_front()

func _process_calibration_sample(snapshot: Dictionary):
    calibration_pool.append(snapshot)

    baseline_progress.emit(calibration_pool.size(), baseline_target_samples)

    print(
        "[Calibration] Logged sample ",
        calibration_pool.size(),
        "/",
        baseline_target_samples
    )

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
    hrv_delta_percent = _percent_delta(
        float(snapshot.get("hrv", 0.0)),
        baseline_hrv
    )

    respiration_delta_percent = _percent_delta(
        float(snapshot.get("respiration_rate", 0.0)),
        baseline_respiration_rate
    )

    reaction_time_delta_percent = _percent_delta(
        float(snapshot.get("reaction_time", 0.0)),
        baseline_reaction_time
    )

    sleep_quality_delta_percent = _percent_delta(
        float(snapshot.get("sleep_quality", 0.0)),
        baseline_sleep_quality
    )

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
        "hrv": _metric_analysis(hrv_history),
        "respiration_rate": _metric_analysis(respiration_history),
        "reaction_time": _metric_analysis(reaction_time_history),
        "sleep_quality": _metric_analysis(sleep_quality_history),
        "baseline": get_baseline_snapshot(),
        "deviation_percent": get_deviation_snapshot()
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


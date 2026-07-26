extends Node

signal state_changed(new_state, confidence)
signal metrics_changed(hrv, respiration_rate, reaction_time, sleep_quality)
signal model_updated(snapshot)
signal history_updated(history_snapshot)
signal analysis_updated(analysis_snapshot)

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

func update_model(payload: Dictionary):
    current_state = str(payload.get("state", "Regulated"))
    confidence = float(payload.get("confidence", 1.0))
    hrv = float(payload.get("hrv", 70.0))
    respiration_rate = float(payload.get("respiration_rate", 12.0))
    reaction_time = float(payload.get("reaction_time", 300.0))
    sleep_quality = float(payload.get("sleep_quality", 80.0))

    var snapshot = {
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

    analysis = get_analysis_snapshot()

    state_changed.emit(current_state, confidence)
    metrics_changed.emit(hrv, respiration_rate, reaction_time, sleep_quality)
    model_updated.emit(snapshot)
    history_updated.emit(get_history_snapshot())
    analysis_updated.emit(analysis)

func _append_to_buffer(buffer: Array, value: float):
    buffer.append(value)

    if buffer.size() > max_history_size:
        buffer.pop_front()

func get_latest_snapshot():
    if history.size() == 0:
        return {}

    return history[history.size() - 1]

func get_history():
    return history

func get_history_snapshot():
    return {
        "hrv": hrv_history,
        "respiration_rate": respiration_history,
        "reaction_time": reaction_time_history,
        "sleep_quality": sleep_quality_history,
        "samples": history.size()
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
        "hrv": _metric_analysis(hrv_history),
        "respiration_rate": _metric_analysis(respiration_history),
        "reaction_time": _metric_analysis(reaction_time_history),
        "sleep_quality": _metric_analysis(sleep_quality_history)
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


extends Node

signal state_changed(new_state, confidence)
signal metrics_changed(hrv, respiration_rate, reaction_time, sleep_quality)
signal model_updated(snapshot)
signal history_updated(history_snapshot)

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

    state_changed.emit(current_state, confidence)
    metrics_changed.emit(hrv, respiration_rate, reaction_time, sleep_quality)
    model_updated.emit(snapshot)
    history_updated.emit(get_history_snapshot())

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


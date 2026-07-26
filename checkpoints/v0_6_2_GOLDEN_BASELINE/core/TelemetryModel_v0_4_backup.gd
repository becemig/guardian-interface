extends Node

signal state_changed(new_state, confidence)
signal metrics_changed(hrv, respiration_rate, reaction_time, sleep_quality)
signal model_updated(snapshot)

var current_state: String = "Regulated"
var confidence: float = 1.0
var hrv: float = 70.0
var respiration_rate: float = 12.0
var reaction_time: float = 300.0
var sleep_quality: float = 80.0

var history := []
var max_history_size := 30

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

    state_changed.emit(current_state, confidence)
    metrics_changed.emit(hrv, respiration_rate, reaction_time, sleep_quality)
    model_updated.emit(snapshot)

func get_latest_snapshot():
    if history.size() == 0:
        return {}
    return history[history.size() - 1]

func get_history():
    return history

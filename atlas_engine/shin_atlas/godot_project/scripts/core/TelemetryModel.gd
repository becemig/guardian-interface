class_name TelemetryModel
extends Node

signal data_processed(summary: Dictionary)
signal dashboard_data_updated(summary: Dictionary)
signal packet_broadcast(packet: RefCounted)

var total_samples_written: int = 0
var hrv_accumulator: float = 0.0
var respiration_accumulator: float = 0.0

var session_average_hrv: float = 0.0
var session_average_respiration: float = 0.0

var current_session_id: String = "N/A"
var participant_id: String = "N/A"
var guardian_state: String = "Unknown"
var confidence: float = 0.0
var current_hrv: float = 0.0
var current_respiration_rate: float = 0.0
var playback_speed: float = 1.0

var current_packet: RefCounted = null


func ingest_replay_snapshot(snapshot: Dictionary) -> void:
    total_samples_written += 1

    var packet_script = load("res://scripts/types/TelemetryPacket.gd")
    var packet: RefCounted = packet_script.new()

    current_session_id = str(snapshot.get("session_id", "N/A"))
    participant_id = str(snapshot.get("participant_id", "N/A"))
    guardian_state = str(snapshot.get("guardian_state", "Unknown"))
    confidence = float(snapshot.get("confidence", 0.0))

    current_hrv = float(snapshot.get("hrv", 0.0))
    current_respiration_rate = float(snapshot.get("respiration_rate", 0.0))

    hrv_accumulator += current_hrv
    respiration_accumulator += current_respiration_rate

    session_average_hrv = hrv_accumulator / float(total_samples_written)
    session_average_respiration = respiration_accumulator / float(total_samples_written)

    var summary := get_dashboard_summary()

    data_processed.emit(summary)
    dashboard_data_updated.emit(summary)

    packet.session_id = current_session_id
    packet.participant_id = participant_id
    packet.sample_index = total_samples_written
    packet.playback_speed = playback_speed
    packet.guardian_state = guardian_state
    packet.confidence = confidence
    packet.hrv = current_hrv
    packet.avg_hrv = session_average_hrv
    packet.respiration_rate = current_respiration_rate
    packet.avg_respiration = session_average_respiration

    current_packet = packet
    packet_broadcast.emit(packet)

    print("[TELEMETRY MODEL] Replay snapshot processed. Avg HRV: ", session_average_hrv, " Avg Resp: ", session_average_respiration)


func get_dashboard_summary() -> Dictionary:
    return {
        "session_id": current_session_id,
        "participant_id": participant_id,
        "sample_index": total_samples_written,
        "playback_speed": playback_speed,
        "guardian_state": guardian_state,
        "confidence": confidence,
        "hrv": current_hrv,
        "avg_hrv": session_average_hrv,
        "respiration_rate": current_respiration_rate,
        "avg_respiration": session_average_respiration
    }


func set_playback_speed(speed: float) -> void:
    playback_speed = speed

    if current_packet != null:
        current_packet.playback_speed = playback_speed
        packet_broadcast.emit(current_packet)

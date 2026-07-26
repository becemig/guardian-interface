class_name TelemetryPacket
extends RefCounted

var session_id: String = "N/A"
var participant_id: String = "N/A"
var sample_index: int = 0
var playback_speed: float = 1.0

var guardian_state: String = "Unknown"
var confidence: float = 0.0

var hrv: float = 0.0
var avg_hrv: float = 0.0

var respiration_rate: float = 0.0
var avg_respiration: float = 0.0


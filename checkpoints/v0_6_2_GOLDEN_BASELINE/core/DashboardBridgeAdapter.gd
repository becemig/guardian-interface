extends Node

signal dashboard_payload_updated(payload)

var controller: Node = null

func _ready():
    print("DashboardBridgeAdapter initialized.")

    controller = preload("res://scripts/core/TelemetryBridgeController.gd").new()
    add_child(controller)

    controller.telemetry_bridge_updated.connect(_on_bridge_updated)

func _on_bridge_updated(payload):
    var dashboard_payload = {
        "state": payload.get("guardian_state", "Unknown"),
        "confidence": payload.get("confidence", 0.0),
        "hrv": payload.get("hrv", 0),
        "respiration_rate": payload.get("respiration_rate", 0),
        "reaction_time": payload.get("reaction_time", 0),
        "sleep_quality": payload.get("sleep_quality", 0)
    }

    dashboard_payload_updated.emit(dashboard_payload)

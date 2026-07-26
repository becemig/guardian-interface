extends Node

var controller: Node = null

func _ready():
    print("TelemetryBridgeTest subscriber initialized.")

    controller = preload("res://scripts/core/TelemetryBridgeController.gd").new()
    add_child(controller)

    controller.telemetry_bridge_updated.connect(_on_bridge_updated)
    controller.telemetry_bridge_stale.connect(_on_bridge_stale)

func _on_bridge_updated(payload):
    print(
        "Signal Received: State=",
        payload.get("guardian_state", "unknown"),
        " Confidence=",
        payload.get("confidence", 0),
        " HRV=",
        payload.get("hrv", 0),
        " Respiration=",
        payload.get("respiration_rate", 0)
    )

func _on_bridge_stale(age_ms):
    print("Warning: Bridge stale for ", age_ms, " ms")

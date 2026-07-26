extends Control

var controller: Node = null

var state_label: Label
var confidence_label: Label
var hrv_label: Label
var respiration_label: Label
var health_label: Label
var stale_bar: ProgressBar

func _ready():
    print("TelemetryBridgeView initialized.")

    controller = preload("res://scripts/core/TelemetryBridgeController.gd").new()
    add_child(controller)

    state_label = $VBoxContainer/StateLabel
    confidence_label = $VBoxContainer/ConfidenceLabel
    hrv_label = $VBoxContainer/HRVLabel
    respiration_label = $VBoxContainer/RespirationLabel
    health_label = $VBoxContainer/HealthLabel
    stale_bar = $VBoxContainer/StaleBar

    controller.telemetry_bridge_updated.connect(_on_updated)
    controller.telemetry_bridge_stale.connect(_on_stale)

func _on_updated(payload):
    var state = str(payload.get("guardian_state", "N/A"))

    state_label.text = "State: " + state
    confidence_label.text = "Confidence: " + str(payload.get("confidence", 0))
    hrv_label.text = "HRV: " + str(payload.get("hrv", 0))
    respiration_label.text = "Respiration: " + str(payload.get("respiration_rate", 0))

    health_label.text = "Bridge: LIVE"
    stale_bar.value = 0

    _apply_state_color(state)

func _on_stale(age_ms):
    health_label.text = "Bridge: STALE " + str(age_ms) + " ms"
    stale_bar.value = min(age_ms, stale_bar.max_value)

    if age_ms > 5000:
        state_label.text = "State: DISCONNECTED"
        modulate = Color.DIM_GRAY

func _apply_state_color(state):
    if state == "Regulated":
        modulate = Color.MEDIUM_SEA_GREEN
    elif state == "Adaptive":
        modulate = Color.CORNFLOWER_BLUE
    elif state == "Stress Load":
        modulate = Color.ORANGE_RED
    elif state == "Cognitive Load":
        modulate = Color.GOLD
    elif state == "Fatigued":
        modulate = Color.SLATE_GRAY
    else:
        modulate = Color.WHITE

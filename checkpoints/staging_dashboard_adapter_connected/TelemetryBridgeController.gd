extends Node

signal telemetry_bridge_updated(payload)
signal telemetry_bridge_stale(age_ms)

var bridge_file := "/home/becemig/GodotProjects/guardian-interface/sandbox_engine/godot_bridge_payload.json"

var last_timestamp := ""
var last_update_ms := 0
var stale_after_ms := 3000
var last_stale_emit_ms := 0
var stale_emit_interval_ms := 3000

func _ready():
    print("TelemetryBridgeController initialized.")
    last_update_ms = Time.get_ticks_msec()

func _process(_delta):
    var payload = _read_bridge()

    if payload == null:
        _emit_stale()
        return

    var timestamp = str(payload.get("timestamp", ""))

    if timestamp != last_timestamp:
        last_timestamp = timestamp
        last_update_ms = Time.get_ticks_msec()
        telemetry_bridge_updated.emit(payload)

    var age_ms = Time.get_ticks_msec() - last_update_ms

    if age_ms > stale_after_ms:
        _emit_stale()

func _read_bridge():
    if not FileAccess.file_exists(bridge_file):
        return null

    var file := FileAccess.open(bridge_file, FileAccess.READ)

    if file == null:
        return null

    var text := file.get_as_text()
    file.close()

    var json := JSON.new()
    var err := json.parse(text)

    if err != OK:
        return null

    return json.data

func _emit_stale():
    var now_ms = Time.get_ticks_msec()

    if now_ms - last_stale_emit_ms > stale_emit_interval_ms:
        var age_ms = now_ms - last_update_ms
        telemetry_bridge_stale.emit(age_ms)
        last_stale_emit_ms = now_ms

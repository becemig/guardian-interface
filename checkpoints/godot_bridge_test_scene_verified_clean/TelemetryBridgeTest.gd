extends Node

var bridge_file := "/home/becemig/GodotProjects/guardian-interface/sandbox_engine/godot_bridge_payload.json"

var last_timestamp := ""
var last_seen_ms := 0
var stale_after_ms := 3000

var last_stale_print_ms := 0
var stale_print_interval_ms := 3000

func _ready():
    print("TelemetryBridgeTest initialized.")
    last_seen_ms = Time.get_ticks_msec()

func _process(_delta):
    if not FileAccess.file_exists(bridge_file):
        _print_stale("Bridge file missing.")
        return

    var file := FileAccess.open(bridge_file, FileAccess.READ)

    if file == null:
        _print_stale("Bridge file unreadable.")
        return

    var text := file.get_as_text()
    file.close()

    var json := JSON.new()
    var err := json.parse(text)

    if err != OK:
        _print_stale("Bridge JSON parse error.")
        return

    var payload = json.data
    var timestamp = str(payload.get("timestamp", ""))

    if timestamp != last_timestamp:
        last_timestamp = timestamp
        last_seen_ms = Time.get_ticks_msec()

        print(
            "State=",
            payload.get("guardian_state", "unknown"),
            " Confidence=",
            payload.get("confidence", 0),
            " HRV=",
            payload.get("hrv", 0),
            " Respiration=",
            payload.get("respiration_rate", 0)
        )

    var age_ms = Time.get_ticks_msec() - last_seen_ms

    if age_ms > stale_after_ms:
        _print_stale("Bridge stale: no update for " + str(age_ms) + " ms")

func _print_stale(message):
    var now_ms = Time.get_ticks_msec()

    if now_ms - last_stale_print_ms > stale_print_interval_ms:
        print(message)
        last_stale_print_ms = now_ms

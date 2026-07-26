extends Node

var bridge_file := "/home/becemig/GodotProjects/guardian-interface/sandbox_engine/godot_bridge_payload.json"
var last_timestamp := ""
var last_seen_ms := 0
var stale_after_ms := 3000

func _ready():
    print("TelemetryBridgeTest initialized.")
    last_seen_ms = Time.get_ticks_msec()

func _process(_delta):
    if not FileAccess.file_exists(bridge_file):
        print("Bridge file missing.")
        return

    var file := FileAccess.open(bridge_file, FileAccess.READ)
    if file == null:
        print("Bridge file unreadable.")
        return

    var text := file.get_as_text()
    file.close()

    var json := JSON.new()
    var err := json.parse(text)

    if err != OK:
        print("Bridge JSON parse error.")
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
        print("Bridge stale: no update for ", age_ms, " ms")


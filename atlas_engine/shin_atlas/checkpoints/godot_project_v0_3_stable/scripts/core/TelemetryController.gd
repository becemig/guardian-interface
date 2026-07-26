extends Node

# Define the signal that your Label is waiting for
signal telemetry_payload_updated(payload: Dictionary)

const TELEMETRY_PATH := "/home/becemig/GodotProjects/guardian-interface/atlas_engine/shin_atlas/godot_project/data/live_telemetry.json"

func _process(_delta):
    if FileAccess.file_exists(TELEMETRY_PATH):
        var file = FileAccess.open(TELEMETRY_PATH, FileAccess.READ)
        var content = file.get_as_text()
        var json = JSON.new()
        if json.parse(content) == OK:
            # Emit the signal so the Label catches it
            telemetry_payload_updated.emit(json.data)
        file.close()

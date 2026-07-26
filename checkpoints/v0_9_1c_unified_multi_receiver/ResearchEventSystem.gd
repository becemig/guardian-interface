class_name ResearchEventSystem
extends Node

signal events_matched(frame_index: int, events: Array)

var events: Array[Dictionary] = []


func add_event(frame_index: int, event_type: String, label: String, notes: String = "") -> void:
    events.append({
        "frame_index": frame_index,
        "event_type": event_type,
        "label": label,
        "notes": notes
    })


func get_events_for_frame(frame_index: int) -> Array:
    var matches: Array = []

    for event in events:
        if int(event.get("frame_index", -1)) == frame_index:
            matches.append(event)

    return matches


func handle_replay_index_changed(frame_index: int, total_frames: int) -> void:
    var matches: Array = get_events_for_frame(frame_index)

    if not matches.is_empty():
        emit_signal("events_matched", frame_index, matches)

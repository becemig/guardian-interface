class_name ReplayController
extends Node

signal replay_frame(snapshot: Dictionary)
signal playback_state_changed(is_playing: bool)
signal index_changed(current_index: int, total_frames: int)

var snapshots: Array[Dictionary] = []
var current_frame_index: int = -1
var is_playing: bool = false
var playback_speed: float = 1.0

var frame_duration: float = 1.0
var _time_accumulator: float = 0.0


func _process(delta: float) -> void:
    if not is_playing or snapshots.is_empty():
        return

    _time_accumulator += delta * playback_speed

    while _time_accumulator >= frame_duration:
        _time_accumulator -= frame_duration

        if current_frame_index < snapshots.size() - 1:
            step_forward()
        else:
            pause()
            break


func load_snapshots(new_snapshots: Array[Dictionary]) -> void:
    snapshots = new_snapshots
    current_frame_index = -1
    _time_accumulator = 0.0
    is_playing = false

    print("[REPLAY CONTROLLER] Mounted %d snapshots into state machine." % snapshots.size())
    emit_signal("index_changed", current_frame_index, snapshots.size())


func play() -> void:
    if snapshots.is_empty():
        push_warning("[REPLAY CONTROLLER] Cannot play: No snapshots loaded.")
        return

    if current_frame_index >= snapshots.size() - 1:
        seek_to_frame(0)

    is_playing = true
    emit_signal("playback_state_changed", is_playing)
    print("[REPLAY CONTROLLER] Playback: STARTED (Speed: ", playback_speed, "x)")


func pause() -> void:
    is_playing = false
    emit_signal("playback_state_changed", is_playing)
    print("[REPLAY CONTROLLER] Playback: PAUSED")


func step_forward() -> void:
    if snapshots.is_empty():
        return

    if current_frame_index < snapshots.size() - 1:
        current_frame_index += 1
        _emit_current_frame()

        if current_frame_index == snapshots.size() - 1 and is_playing:
            pause()
    else:
        print("[REPLAY CONTROLLER] Terminal boundary reached. Cannot step forward.")
        if is_playing:
            pause()


func step_backward() -> void:
    if snapshots.is_empty():
        return

    if current_frame_index > 0:
        current_frame_index -= 1
        _emit_current_frame()
    else:
        print("[REPLAY CONTROLLER] Initial boundary reached. Cannot step backward.")


func seek_to_frame(index: int) -> void:
    if snapshots.is_empty():
        return

    var target_index := clampi(index, 0, snapshots.size() - 1)
    current_frame_index = target_index
    _time_accumulator = 0.0

    print("[REPLAY CONTROLLER] Explicit seek executed to index: ", current_frame_index)
    _emit_current_frame()


func set_playback_speed(speed: float) -> void:
    playback_speed = maxf(speed, 0.0)
    print("[REPLAY CONTROLLER] Playback speed updated to: ", playback_speed, "x")


func _emit_current_frame() -> void:
    if current_frame_index >= 0 and current_frame_index < snapshots.size():
        var snapshot: Dictionary = snapshots[current_frame_index]
        emit_signal("replay_frame", snapshot)
        emit_signal("index_changed", current_frame_index, snapshots.size())

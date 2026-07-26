class_name ReplayTimelinePanel
extends Panel

var _controller: Node = null
var _updating_slider: bool = false

var button_play_pause: Button = null
var button_step_back: Button = null
var button_step_forward: Button = null
var hslider_timeline: HSlider = null
var option_button_speed: OptionButton = null
var label_status: Label = null


func _ready() -> void:
    _cache_nodes()
    option_button_speed.clear()
    option_button_speed.add_item("0.25x")
    option_button_speed.add_item("0.5x")
    option_button_speed.add_item("1.0x")
    option_button_speed.add_item("2.0x")
    option_button_speed.select(2)

    button_play_pause.pressed.connect(_on_play_pause_pressed)
    button_step_back.pressed.connect(_on_step_back_pressed)
    button_step_forward.pressed.connect(_on_step_forward_pressed)
    hslider_timeline.value_changed.connect(_on_slider_value_changed)
    option_button_speed.item_selected.connect(_on_speed_selected)


func _cache_nodes() -> void:
    button_play_pause = get_node_or_null("HBoxContainer/Button_PlayPause")
    button_step_back = get_node_or_null("HBoxContainer/Button_StepBack")
    button_step_forward = get_node_or_null("HBoxContainer/Button_StepForward")
    hslider_timeline = get_node_or_null("HBoxContainer/HSlider_Timeline")
    option_button_speed = get_node_or_null("HBoxContainer/OptionButton_Speed")
    label_status = get_node_or_null("Label_Status")


func bind_replay_controller(controller: Node) -> void:
    _cache_nodes()
    _controller = controller

    if _controller.has_signal("index_changed"):
        _controller.index_changed.connect(_handle_index_changed)

    if _controller.has_signal("playback_state_changed"):
        _controller.playback_state_changed.connect(_handle_playback_state_changed)

    _handle_playback_state_changed(false)


func _on_play_pause_pressed() -> void:
    if _controller == null:
        return

    var playing: bool = bool(_controller.get("is_playing"))

    if playing:
        if _controller.has_method("pause"):
            _controller.pause()
    else:
        if _controller.has_method("play"):
            _controller.play()


func _on_step_back_pressed() -> void:
    if _controller != null and _controller.has_method("step_backward"):
        _controller.step_backward()


func _on_step_forward_pressed() -> void:
    if _controller != null and _controller.has_method("step_forward"):
        _controller.step_forward()


func _on_slider_value_changed(value: float) -> void:
    if _updating_slider:
        return

    if _controller != null and _controller.has_method("seek_to_frame"):
        _controller.seek_to_frame(int(value))


func _on_speed_selected(index: int) -> void:
    if _controller == null:
        return

    var target_speed: float = 1.0

    match index:
        0:
            target_speed = 0.25
        1:
            target_speed = 0.5
        2:
            target_speed = 1.0
        3:
            target_speed = 2.0

    if _controller.has_method("set_playback_speed"):
        _controller.set_playback_speed(target_speed)


func _handle_index_changed(frame_index: int, total_frames: int) -> void:
    _updating_slider = true
    hslider_timeline.max_value = max(0, total_frames - 1)
    hslider_timeline.value = frame_index
    _updating_slider = false

    label_status.text = "Frame: %d / %d" % [frame_index + 1, total_frames]


func _handle_playback_state_changed(is_playing: bool) -> void:
    button_play_pause.text = "Pause" if is_playing else "Play"

    if not label_status.text.begins_with("Frame:"):
        label_status.text = "Status: %s" % ("PLAYING" if is_playing else "PAUSED")

class_name VisualizationManager
extends Node

signal visual_frame_updated(summary: Dictionary)
signal visual_packet_routed(packet: RefCounted)
signal visual_event_triggered(frame_index: int, events: Array)

var last_telemetry_summary: Dictionary = {}
var last_packet: RefCounted = null
var last_matched_events: Array = []


func bind_visualization_targets(telemetry_model: Node, research_event_system: Node) -> void:
    if telemetry_model != null:
        if telemetry_model.has_signal("dashboard_data_updated"):
            if not telemetry_model.dashboard_data_updated.is_connected(_on_telemetry_updated):
                telemetry_model.dashboard_data_updated.connect(_on_telemetry_updated)

        if telemetry_model.has_signal("packet_broadcast"):
            if not telemetry_model.packet_broadcast.is_connected(_on_packet_received):
                telemetry_model.packet_broadcast.connect(_on_packet_received)

    if research_event_system != null:
        if research_event_system.has_signal("events_matched"):
            if not research_event_system.events_matched.is_connected(_on_events_matched):
                research_event_system.events_matched.connect(_on_events_matched)

    print("[VISUALIZATION MANAGER] Backward-compatible packet routing active.")


func _on_telemetry_updated(summary: Dictionary) -> void:
    last_telemetry_summary = summary
    visual_frame_updated.emit(summary)


func _on_packet_received(packet: RefCounted) -> void:
    last_packet = packet
    visual_packet_routed.emit(packet)


func _on_events_matched(frame_index: int, events: Array) -> void:
    last_matched_events = events
    visual_event_triggered.emit(frame_index, events)


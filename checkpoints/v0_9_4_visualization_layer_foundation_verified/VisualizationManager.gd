class_name VisualizationManager
extends Node

# ============================================================
# Guardian Interface
# VisualizationManager.gd
# Version: v0.9.4
#
# Purpose:
# Pure routing layer between backend processing
# and future visualization systems.
#
# No calculations.
# No rendering.
# No state evaluation.
#
# Only receives signals,
# caches the newest data,
# and forwards visualization events.
# ============================================================


# ------------------------------------------------------------
# Visualization Signals
# ------------------------------------------------------------

signal visual_frame_updated(summary: Dictionary)
signal visual_event_triggered(frame_index: int, events: Array)


# ------------------------------------------------------------
# Cached State
# ------------------------------------------------------------

var last_telemetry_summary: Dictionary = {}
var last_matched_events: Array = []


# ------------------------------------------------------------
# Bind Backend Systems
# ------------------------------------------------------------

func bind_visualization_targets(
        telemetry_model: Node,
        research_event_system: Node
    ) -> void:

    # Quantitative Stream
    if telemetry_model != null:
        if telemetry_model.has_signal("dashboard_data_updated"):
            if not telemetry_model.dashboard_data_updated.is_connected(_on_telemetry_updated):
                telemetry_model.dashboard_data_updated.connect(_on_telemetry_updated)

    # Qualitative Stream
    if research_event_system != null:
        if research_event_system.has_signal("events_matched"):
            if not research_event_system.events_matched.is_connected(_on_events_matched):
                research_event_system.events_matched.connect(_on_events_matched)

    print("[VISUALIZATION MANAGER] Connected to Quantitative and Qualitative telemetry streams.")


# ------------------------------------------------------------
# Quantitative Update
# ------------------------------------------------------------

func _on_telemetry_updated(summary: Dictionary) -> void:

    last_telemetry_summary = summary

    # Pure pass-through
    visual_frame_updated.emit(summary)


# ------------------------------------------------------------
# Qualitative Update
# ------------------------------------------------------------

func _on_events_matched(frame_index: int, events: Array) -> void:

    last_matched_events = events

    # Pure pass-through
    visual_event_triggered.emit(frame_index, events)


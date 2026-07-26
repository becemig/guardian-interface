extends Node

@onready var replay_engine: Node = $ReplayEngine
@onready var replay_controller: Node = $ReplayController
@onready var telemetry_model: Node = $TelemetryModel
@onready var research_event_system: Node = $ResearchEventSystem
@onready var replay_timeline_panel: Panel = $CanvasLayer/ReplayTimelinePanel

func _ready() -> void:
    replay_controller.replay_frame.connect(telemetry_model.ingest_replay_snapshot)
    replay_controller.index_changed.connect(research_event_system.handle_replay_index_changed)
    replay_timeline_panel.bind_replay_controller(replay_controller)

    research_event_system.add_event(0, "Protocol", "Frolics Begin", "Initial Tiger Form stance engagement.")
    research_event_system.add_event(1, "Somatic", "Qi Flow Balance", "Yin-Yang normalization sequence stabilization.")

    _initialize_demo_dataset()

func _initialize_demo_dataset() -> void:
    var target_file := "res://demo_runtime_track.csv"

    var header_row := "schema_version,session_id,participant_id,sample_index,sample_time_unix,elapsed_seconds,delta_seconds,protocol_name,phase_name,phase_index,phase_elapsed_seconds,phase_target_duration,phase_completion_percent,guardian_state,confidence,hrv,respiration_rate,reaction_time,sleep_quality,baseline_hrv,baseline_respiration_rate,baseline_reaction_time,baseline_sleep_quality,hrv_delta_percent,respiration_delta_percent,reaction_time_delta_percent,sleep_quality_delta_percent,validation_status,validation_notes"
    var row0 := "GI-0.8.1,SESS_LIVE_DEMO,P_001,0,1782928560,0.0,0.0,Five_Animal_Frolics,Active,1,0.0,300.0,0.0,Regulated,0.95,78.0,14.0,300.0,80.0,60.0,14.0,300.0,80.0,0.0,0.0,0.0,0.0,OK,Nominal"
    var row1 := "GI-0.8.1,SESS_LIVE_DEMO,P_001,1,1782928561,1.0,1.0,Five_Animal_Frolics,Active,1,1.0,300.0,0.0,Regulated,0.95,84.0,12.0,300.0,80.0,60.0,14.0,300.0,80.0,0.0,0.0,0.0,0.0,OK,Nominal"

    var f := FileAccess.open(target_file, FileAccess.WRITE)
    if f:
        f.store_string(header_row + "\n" + row0 + "\n" + row1 + "\n")
        f.close()

    var parsed_snapshots: Array[Dictionary] = replay_engine.load_replay_as_snapshots(target_file)
    replay_controller.load_snapshots(parsed_snapshots)

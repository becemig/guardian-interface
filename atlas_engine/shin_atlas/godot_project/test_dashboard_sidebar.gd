extends SceneTree

func _initialize():
    var dashboard = load("res://scenes/ui/TelemetryDashboard.tscn").instantiate()
    var model = load("res://scripts/core/TelemetryModel.gd").new()

    get_root().add_child(dashboard)
    get_root().add_child(model)

    await process_frame

    dashboard.bind_dashboard_targets(model)

    model.ingest_replay_snapshot({
        "session_id": "SESS_TEST_A",
        "participant_id": "PARTICIPANT_1",
        "guardian_state": "Regulated",
        "confidence": 0.98,
        "hrv": 70.0,
        "respiration_rate": 12.0
    })

    model.ingest_replay_snapshot({
        "session_id": "SESS_TEST_A",
        "participant_id": "PARTICIPANT_1",
        "guardian_state": "Stressed",
        "confidence": 0.85,
        "hrv": 90.0,
        "respiration_rate": 16.0
    })

    print("Guardian State =", dashboard.label_guardian_state.text)
    print("Confidence     =", dashboard.label_confidence.text)
    print("Current HRV    =", dashboard.label_current_hrv.text)
    print("Average HRV    =", dashboard.label_avg_hrv.text)
    print("Average Resp   =", dashboard.label_avg_resp.text)

    quit()

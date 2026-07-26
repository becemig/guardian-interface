class_name ReplayEngine
extends Node

# ReplayEngine.gd - Row-to-Dictionary Replay Mapper (v0.8.4b)

const EXPECTED_HEADERS: Array[String] = [
    "schema_version",
    "session_id",
    "participant_id",
    "sample_index",
    "sample_time_unix",
    "elapsed_seconds",
    "delta_seconds",
    "protocol_name",
    "phase_name",
    "phase_index",
    "phase_elapsed_seconds",
    "phase_target_duration",
    "phase_completion_percent",
    "guardian_state",
    "confidence",
    "hrv",
    "respiration_rate",
    "reaction_time",
    "sleep_quality",
    "baseline_hrv",
    "baseline_respiration_rate",
    "baseline_reaction_time",
    "baseline_sleep_quality",
    "hrv_delta_percent",
    "respiration_delta_percent",
    "reaction_time_delta_percent",
    "sleep_quality_delta_percent",
    "validation_status",
    "validation_notes"
]


func parse_row_to_dict(row_columns: Array) -> Dictionary:
    var snapshot := {}

    if row_columns.size() != EXPECTED_HEADERS.size():
        push_error("[REPLAY ENGINE] Row column count mismatch during parsing.")
        return snapshot

    for i in range(EXPECTED_HEADERS.size()):
        var key := EXPECTED_HEADERS[i]
        var raw_value := str(row_columns[i]).strip_edges()

        if key in ["sample_index", "sample_time_unix", "phase_index"]:
            snapshot[key] = raw_value.to_int()
        elif key in [
            "schema_version",
            "session_id",
            "participant_id",
            "protocol_name",
            "phase_name",
            "guardian_state",
            "validation_status",
            "validation_notes"
        ]:
            snapshot[key] = raw_value
        else:
            snapshot[key] = raw_value.to_float()

    return snapshot


func load_replay_as_snapshots(file_path: String) -> Array[Dictionary]:
    print("\n====================================================")
    print("[REPLAY ENGINE] EXECUTING v0.8.4b DICTIONARY MAPPER")
    print("====================================================")
    print("[REPLAY ENGINE] Reading raw source data: ", file_path)

    var typed_snapshots: Array[Dictionary] = []

    if not FileAccess.file_exists(file_path):
        push_error("[REPLAY ENGINE] File not found: " + file_path)
        print("====================================================\n")
        return typed_snapshots

    var file := FileAccess.open(file_path, FileAccess.READ)

    if file == null:
        push_error("[REPLAY ENGINE] File access failed.")
        print("====================================================\n")
        return typed_snapshots

    var header_line := file.get_line().strip_edges()
    var header_columns := header_line.split(",")

    if header_columns.size() != EXPECTED_HEADERS.size():
        push_error("[REPLAY ENGINE] Header dimension mismatch.")
        file.close()
        print("====================================================\n")
        return typed_snapshots

    for i in range(EXPECTED_HEADERS.size()):
        if header_columns[i] != EXPECTED_HEADERS[i]:
            push_error("[REPLAY ENGINE] Header mismatch at index %d." % i)
            file.close()
            print("====================================================\n")
            return typed_snapshots

    print("[REPLAY ENGINE] CSV Layout Configuration: VERIFIED")

    while not file.eof_reached():
        var line := file.get_line().strip_edges()

        if line == "":
            continue

        var columns := line.split(",")

        if columns.size() != EXPECTED_HEADERS.size():
            print("[REPLAY ENGINE] Skipping unbalanced data row.")
            continue

        var parsed_dict := parse_row_to_dict(columns)

        if not parsed_dict.is_empty():
            typed_snapshots.append(parsed_dict)

    file.close()

    print("[REPLAY ENGINE] SUCCESS: Data logs extracted and converted to typed snapshot payloads.")
    print("[REPLAY ENGINE] Total In-Memory Dictionary Objects: ", typed_snapshots.size())
    print("====================================================\n")

    return typed_snapshots


func stream_snapshots_to_model(snapshots: Array[Dictionary], telemetry_model: Node) -> void:
    print("\n====================================================")
    print("[REPLAY ENGINE] INITIATING STREAM EMULATION")
    print("====================================================")
    print("[REPLAY ENGINE] Pumping %d snapshots into pipeline..." % snapshots.size())

    if telemetry_model == null:
        push_error("[REPLAY ENGINE] CRITICAL: Target TelemetryModel reference is null.")
        print("====================================================\n")
        return

    var ingested_count := 0

    for snapshot in snapshots:
        if telemetry_model.has_method("ingest_replay_snapshot"):
            telemetry_model.ingest_replay_snapshot(snapshot)
            ingested_count += 1
        else:
            push_error("[REPLAY ENGINE] Target model missing ingest_replay_snapshot().")
            break

    print("[REPLAY ENGINE] SUCCESS: Stream emulation cycle concluded.")
    print("[REPLAY ENGINE] Total Snapshots Processed: ", ingested_count)
    print("====================================================\n")

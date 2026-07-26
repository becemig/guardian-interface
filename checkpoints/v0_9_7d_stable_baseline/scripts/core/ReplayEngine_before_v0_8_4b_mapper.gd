class_name ReplayEngine
extends Node

# ReplayEngine.gd - Skeleton & CSV Reader (v0.8.4a)

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


func load_replay_file(file_path: String) -> Array:
    print("\n====================================================")
    print("[REPLAY ENGINE] EXECUTING v0.8.4a REPLAY CORE")
    print("====================================================")
    print("[REPLAY ENGINE] Attempting to mount data log: ", file_path)

    var data_records: Array = []

    if not FileAccess.file_exists(file_path):
        push_error("[REPLAY ENGINE] CRITICAL: File does not exist: " + file_path)
        print("====================================================\n")
        return data_records

    var file := FileAccess.open(file_path, FileAccess.READ)

    if file == null:
        push_error("[REPLAY ENGINE] CRITICAL: File IO handle allocation failed.")
        print("====================================================\n")
        return data_records

    var header_line := file.get_line().strip_edges()
    var header_columns := header_line.split(",")

    if header_columns.size() != EXPECTED_HEADERS.size():
        push_error("[REPLAY ENGINE] Header count mismatch. Expected %d, got %d." % [
            EXPECTED_HEADERS.size(),
            header_columns.size()
        ])
        file.close()
        print("====================================================\n")
        return data_records

    for i in range(EXPECTED_HEADERS.size()):
        if header_columns[i] != EXPECTED_HEADERS[i]:
            push_error("[REPLAY ENGINE] Header mismatch at column %d. Expected '%s', got '%s'." % [
                i,
                EXPECTED_HEADERS[i],
                header_columns[i]
            ])
            file.close()
            print("====================================================\n")
            return data_records

    print("[REPLAY ENGINE] Schema structural verification: VALID")

    while not file.eof_reached():
        var line := file.get_line().strip_edges()

        if line == "":
            continue

        var columns := line.split(",")

        if columns.size() != EXPECTED_HEADERS.size():
            print("[REPLAY ENGINE] WARNING: Skipping malformed row: ", line)
            continue

        data_records.append(columns)

    file.close()

    print("[REPLAY ENGINE] SUCCESS: Data file buffered into memory.")
    print("[REPLAY ENGINE] Total Time-Series Rows Retained: ", data_records.size())
    print("====================================================\n")

    return data_records

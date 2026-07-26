class_name SummaryEngine
extends Node

const SUMMARY_SCHEMA_VERSION: String = "GIS-0.1.0"

func build_summary_contract(raw_data: Dictionary) -> Dictionary:
    print("\n====================================================")
    print("[SUMMARY ENGINE] EXECUTING v0.8.3e FILE WRITER")
    print("====================================================")

    var required_blocks := [
        "session_metadata",
        "pipeline_integrity",
        "baseline_statistics",
        "validation"
    ]

    var structural_integrity := "VALID"

    for block in required_blocks:
        if not raw_data.has(block):
            push_error("[SUMMARY ENGINE] Missing required contract block: " + block)
            structural_integrity = "MALFORMED"

    print("Contract Structure Status: ", structural_integrity)

    if structural_integrity == "VALID":
        var json_string := JSON.stringify(raw_data, "    ")
        var metadata: Dictionary = raw_data.get("session_metadata", {})
        var session_id: String = metadata.get("session_id", "UNKNOWN_SESSION")
        var target_dir: String = metadata.get("log_directory", "user://")

        if target_dir != "" and not target_dir.ends_with("/"):
            target_dir += "/"

        var full_path := target_dir + "session_" + session_id + "_summary.json"

        print("[SUMMARY ENGINE] Writing file to: ", full_path)

        var file := FileAccess.open(full_path, FileAccess.WRITE)
        if file:
            file.store_string(json_string)
            file.close()
            print("[SUMMARY ENGINE] SUCCESS: Companion metadata file written cleanly.")
        else:
            var error_code := FileAccess.get_open_error()
            push_error("[SUMMARY ENGINE] CRITICAL: File write failed. Error code: %s" % error_code)
    else:
        print("[SUMMARY ENGINE] Execution halted due to malformed payload.")

    print("====================================================\n")
    return raw_data

class_name SummaryEngine
extends Node

const SUMMARY_SCHEMA_VERSION: String = "GIS-0.1.0"

func build_summary_contract(raw_data: Dictionary) -> Dictionary:
    print("\n====================================================")
    print("[SUMMARY ENGINE] EXECUTING v0.8.3f INTEGRITY SEAL")
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
        var metadata: Dictionary = raw_data.get("session_metadata", {})
        var session_id: String = metadata.get("session_id", "UNKNOWN_SESSION")
        var target_dir: String = metadata.get("log_directory", "user://")
        var csv_path: String = metadata.get("csv_file_path", "")

        if target_dir != "" and not target_dir.ends_with("/"):
            target_dir += "/"

        var csv_checksum := "NOT_FOUND"

        if csv_path != "" and FileAccess.file_exists(csv_path):
            csv_checksum = FileAccess.get_sha256(csv_path)
            print("[SUMMARY ENGINE] SHA-256 Checksum Successfully Generated.")
        else:
            print("[SUMMARY ENGINE] WARNING: CSV file not found: ", csv_path)

        raw_data["security_validation"] = {
            "csv_sha256_checksum": csv_checksum
        }

        var json_string := JSON.stringify(raw_data, "    ")
        var full_path := target_dir + "session_" + session_id + "_summary.json"

        print("[SUMMARY ENGINE] Writing sealed summary file to: ", full_path)

        var file := FileAccess.open(full_path, FileAccess.WRITE)
        if file:
            file.store_string(json_string)
            file.close()
            print("[SUMMARY ENGINE] SUCCESS: Secure cryptographic companion package written cleanly.")
        else:
            push_error("[SUMMARY ENGINE] CRITICAL: Summary file write failed. Error code: %s" % FileAccess.get_open_error())

    print("====================================================\n")
    return raw_data

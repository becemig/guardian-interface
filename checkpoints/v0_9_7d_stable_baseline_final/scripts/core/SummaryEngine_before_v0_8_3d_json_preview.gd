class_name SummaryEngine
extends Node

const SUMMARY_SCHEMA_VERSION: String = "GIS-0.1.0"

func build_summary(session_data: Dictionary) -> Dictionary:
    return {
        "summary_schema_version": SUMMARY_SCHEMA_VERSION,
        "data_contract_version": session_data.get("data_contract_version", "GI-0.8.1"),
        "session_metadata": session_data.get("session_metadata", {}),
        "pipeline_integrity": session_data.get("pipeline_integrity", {}),
        "somatic_baseline_deltas": session_data.get("somatic_baseline_deltas", {}),
        "security_validation": session_data.get("security_validation", {})
    }


func build_summary_contract(raw_data: Dictionary) -> Dictionary:
    print("\n====================================================")
    print("[SUMMARY ENGINE] VERIFYING v0.8.3c DATA CONTRACT")
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
    print("Session ID: ", raw_data.get("session_metadata", {}).get("session_id", "N/A"))
    print("Participant: ", raw_data.get("session_metadata", {}).get("participant_id", "N/A"))
    print("Total Rows Captured: ", raw_data.get("pipeline_integrity", {}).get("total_samples_written", 0))
    print("Validation Status: ", raw_data.get("validation", {}).get("validation_status", "N/A"))
    print("Average HRV: ", raw_data.get("baseline_statistics", {}).get("session_average_hrv", 0.0))
    print("Average Respiration: ", raw_data.get("baseline_statistics", {}).get("session_average_respiration", 0.0))
    print("====================================================\n")

    return raw_data

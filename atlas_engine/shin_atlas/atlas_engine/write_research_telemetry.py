import json
import time
from pathlib import Path
from guardian_state_engine import derive_guardian_state
from session_logger import log_payload

OUT = Path("/home/becemig/GodotProjects/guardian-interface/atlas_engine/shin_atlas/godot_project/data/live_telemetry.json")

test_frames = [
    {
        "active_signal": "hrv",
        "hrv": 0.72,
        "respiration_rate": 12,
        "reaction_time": 0.41,
        "sleep_quality": 0.63,
        "matched_count": 9,
        "top_node_labels": [
            "[Shin-ISU] Stress and Strain",
            "[Shin-ISU] Slow Wave Sleep",
            "[Shin-ISU] Autonomic Regulation",
            "[Shin-ISU] Neuroplasticity",
            "[Shin-ISU] Sleep Recovery"
        ]
    },
    {
        "active_signal": "respiration_rate",
        "hrv": 0.55,
        "respiration_rate": 18,
        "reaction_time": 0.52,
        "sleep_quality": 0.44,
        "matched_count": 9,
        "top_node_labels": [
            "[Shin-ISU] Autonomic Regulation",
            "[Shin-ISU] Stress and Strain",
            "[Shin-ISU] Breathing Regulation",
            "[Shin-ISU] Sleep Recovery",
            "[Shin-ISU] Vagal Regulation"
        ]
    }
]

def cluster_from_nodes(labels):
    text = " ".join(labels).lower()

    if "stress" in text or "strain" in text:
        return "Stress/Strain"
    if "sleep" in text or "wave" in text or "recovery" in text:
        return "Sleep/Recovery"
    if "autonomic" in text or "vagus" in text or "regulation" in text:
        return "Autonomic"

    return "General/Mixed"

def main():
    OUT.parent.mkdir(parents=True, exist_ok=True)
    print("Research Telemetry Writer started.")

    while True:
        for frame in test_frames:
            state = derive_guardian_state(frame)

            payload = dict(frame)
            payload.update(state)
            payload["matched_cluster"] = cluster_from_nodes(payload["top_node_labels"])

            with open(OUT, "w") as f:
                json.dump(payload, f, indent=2)

            log_payload(payload)

            print(
                f"{payload['active_signal']} | "
                f"{payload['guardian_state']} | "
                f"{payload['matched_cluster']} | "
                f"confidence {payload['confidence']}"
            )

            time.sleep(2)

if __name__ == "__main__":
    main()

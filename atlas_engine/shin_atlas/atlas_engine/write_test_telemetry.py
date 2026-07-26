import json
import time
from pathlib import Path

OUT = Path("godot_project/data/live_telemetry.json")

test_frames = [
    {
        "active_signal": "hrv",
        "hrv": 0.72,
        "respiration_rate": 12,
        "reaction_time": 0.41,
        "sleep_quality": 0.63,
        "matched_nodes": ["psy_cbt_grounding", "psy_sleep_circadian"],
        "guardian_modules": ["Attention Lab", "Aging Lab"]
    },
    {
        "active_signal": "respiration_rate",
        "hrv": 0.55,
        "respiration_rate": 18,
        "reaction_time": 0.52,
        "sleep_quality": 0.44,
        "matched_nodes": ["psy_dbt_emotional_regulation", "psy_cbt_cognitive_reframing"],
        "guardian_modules": ["Attention Lab", "Rehabilitation Lab"]
    },
    {
        "active_signal": "sleep_quality",
        "hrv": 0.48,
        "respiration_rate": 16,
        "reaction_time": 0.61,
        "sleep_quality": 0.31,
        "matched_nodes": ["psy_sleep_insomnia", "psy_dream_emotional_processing"],
        "guardian_modules": ["Aging Lab", "Adaptation Lab"]
    }
]

def main():
    OUT.parent.mkdir(parents=True, exist_ok=True)

    print("Starting Telemetry Simulation. Press Ctrl+C to stop.")
    while True:
        for frame in test_frames:
            with open(OUT, "w") as f:
                json.dump(frame, f, indent=2)

            print(f"Wrote telemetry frame: {frame['active_signal']}")
            time.sleep(2)

if __name__ == "__main__":
    main()


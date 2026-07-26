import json
import os

def inject_concept(concept_id, intensity):
    # This simulates a bridge: updating the JSON that Godot monitors
    sync_file = 'godot_project/data/live_influence.json'
    data = {"concept": concept_id, "intensity": intensity}
    with open(sync_file, 'w') as f:
        json.dump(data, f)
    print(f"Injected {concept_id} at intensity {intensity} into Godot.")

if __name__ == "__main__":
    import sys
    inject_concept(sys.argv[1], sys.argv[2])

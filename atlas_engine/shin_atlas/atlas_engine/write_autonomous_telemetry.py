import json
import time
from pathlib import Path

CONFIG = Path("data/current_universe.json")
OUT = "/home/becemig/GodotProjects/guardian-interface/atlas_engine/shin_atlas/godot_project/data/live_telemetry.json"
MAX_MATCHED_NODES = 5

def get_active_universe_path():
    with open(CONFIG, "r") as f:
        config = json.load(f)
    return Path(config["source_file"])

def load_universe(path):
    with open(path, "r") as f:
        return json.load(f).get("nodes", [])

def get_signals(node):
    top = node.get("observable_signals", [])
    nested = node.get("properties", {}).get("observable_signals", [])
    return list(set(top + nested))

def match_nodes_by_signal(nodes, signal):
    matches = []
    for node in nodes:
        if signal in get_signals(node):
            matches.append(node)
    return matches

def build_payload(frame, nodes):
    active_signal = frame["active_signal"]
    matched = match_nodes_by_signal(nodes, active_signal)
    
    # Sort by specificity (fewer signals = higher relevance)
    matched.sort(key=lambda n: len(get_signals(n)))
    top_matches = matched[:MAX_MATCHED_NODES]

    payload = dict(frame)
    payload["matched_count"] = len(matched)
    payload["top_node_labels"] = [n.get("label", "Unknown") for n in top_matches]
    payload["matched_cluster"] = list(set([n.get("subdomain", "Autonomic") for n in matched]))
    return payload

def main():
    nodes = load_universe(get_active_universe_path())
    test_frames = [{"active_signal": "hrv"}, {"active_signal": "respiration_rate"}]
    
    print("Telemetry Writer Active | Cluster Mode: ON")
    while True:
        for frame in test_frames:
            payload = build_payload(frame, nodes)
            with open(OUT, "w") as f:
                json.dump(payload, f, indent=2)
            time.sleep(2)

if __name__ == "__main__":
    main()

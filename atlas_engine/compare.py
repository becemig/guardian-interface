import json
import sys

MASTER_FILE = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"

def compare_nodes(node_ids):
    # Ensure we only work with unique IDs provided
    node_ids = list(set(node_ids))
    with open(MASTER_FILE, 'r') as f:
        data = json.load(f)
    
    nodes = [n for n in data.get("nodes", []) if n.get("id") in node_ids]
    
    print(f"\n--- Comparing Subgroup: {len(nodes)} nodes ---")
    
    # 1. Find Shared Tags
    all_tags = [set(n.get("tags", [])) for n in nodes if "tags" in n]
    if all_tags:
        shared = set.intersection(*all_tags) if all_tags else set()
        print(f"Shared Tags/Themes: {shared}")
    
    # 2. Map the "Heuristic Web"
    print("\n--- Network of Heuristic Bridges ---")
    for n in nodes:
        bridges = n.get("bridges", [])
        for b in bridges:
            if b['target'] in node_ids:
                print(f"  [Internal Link] {n.get('id')} -> {b['target']} via {b['heuristic_note']}")
            else:
                print(f"  [External Branch] {n.get('id')} -> {b['target']}")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        compare_nodes(sys.argv[1:])

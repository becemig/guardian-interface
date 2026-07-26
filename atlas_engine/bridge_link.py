import json
import sys
import os

MASTER_FILE = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"

def get_data():
    if not os.path.exists(MASTER_FILE): return {"nodes": []}
    with open(MASTER_FILE, 'r') as f:
        return json.load(f)

def add_bridge(source_id, target_id, link_type, heuristic):
    data = get_data()
    found = False
    for node in data.get("nodes", []):
        if node.get("id") == source_id:
            if "bridges" not in node: node["bridges"] = []
            node["bridges"].append({"target": target_id, "link_type": link_type, "heuristic_note": heuristic})
            found = True
            break
    if found:
        with open(MASTER_FILE, 'w') as f:
            json.dump(data, f, indent=4)
        print(f"--- Success: Linked {source_id} -> {target_id} ---")
    else:
        print(f"--- Error: Node '{source_id}' not found. ---")

def query_bridges(heuristic_query):
    data = get_data()
    print(f"--- Querying nodes linked via: '{heuristic_query}' ---")
    found = False
    for node in data.get("nodes", []):
        for bridge in node.get("bridges", []):
            if heuristic_query.lower() in bridge["heuristic_note"].lower():
                print(f"Match found in '{node.get('label', node['id'])}': {bridge['target']} via {bridge['heuristic_note']}")
                found = True
    if not found: print("--- No matches found. ---")

if __name__ == "__main__":
    if len(sys.argv) == 5:
        add_bridge(sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4])
    elif len(sys.argv) == 3 and sys.argv[1] == "--query":
        query_bridges(sys.argv[2])

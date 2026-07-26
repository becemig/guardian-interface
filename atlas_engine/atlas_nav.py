import json
import sys
import os
import datetime
import difflib

BASE = "/home/becemig/GodotProjects/guardian-interface/master_data/"
MASTER_NODES = os.path.join(BASE, "master_study_nodes.json")
FOCUS = os.path.join(BASE, "current_focus.json")
HISTORY = os.path.join(BASE, "history_stack.json")
LOG = os.path.join(BASE, "research_log.txt")

def find_node_by_label(query):
    with open(MASTER_NODES, 'r') as f:
        data = json.load(f)["nodes"]
    labels = {node["label"]: node["id"] for node in data}
    matches = difflib.get_close_matches(query, labels.keys(), n=1, cutoff=0.3)
    return labels[matches[0]] if matches else None

def update_focus(node_id):
    with open(FOCUS, 'w') as f:
        json.dump({"focus_node": node_id}, f)

def jump(node_id):
    update_focus(node_id)
    # Log and history logic...
    with open(LOG, 'a') as f:
        f.write(f"{datetime.datetime.now()} | NAVIGATED TO: {node_id}\n")
    print(f"--- Jumped to: {node_id} ---")

if __name__ == "__main__":
    if "--node" in sys.argv:
        jump(sys.argv[sys.argv.index("--node") + 1])
    elif "--search" in sys.argv:
        query = " ".join(sys.argv[sys.argv.index("--search") + 1:])
        node_id = find_node_by_label(query)
        if node_id:
            jump(node_id)
        else:
            print(f"--- No node found matching: {query} ---")
    elif "--back" in sys.argv:
        # (Include your previous back logic here)
        pass

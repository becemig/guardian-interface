import json
import sys
import os

FOCUS_PATH = "/home/becemig/GodotProjects/guardian-interface/master_data/current_focus.json"

def set_focus(node_id):
    # Ensure directory exists
    os.makedirs(os.path.dirname(FOCUS_PATH), exist_ok=True)
    
    # Write the intent to a bridge file
    with open(FOCUS_PATH, 'w') as f:
        json.dump({"focus_node": node_id, "timestamp": os.path.getmtime(FOCUS_PATH) if os.path.exists(FOCUS_PATH) else 0}, f)
    
    print(f"--- Signal Sent: Navigation focus shifted to {node_id} ---")

if __name__ == "__main__":
    if len(sys.argv) == 3 and sys.argv[1] == "--node":
        set_focus(sys.argv[2])
    else:
        print("Usage: python3 jump_to.py --node <node_id>")

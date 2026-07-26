import json
import sys

MASTER_FILE = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"

def inspect(node_id):
    with open(MASTER_FILE, 'r') as f:
        data = json.load(f)
    
    node = next((n for n in data.get("nodes", []) if n.get("id") == node_id), None)
    
    if not node:
        print(f"--- Node '{node_id}' not found ---")
        return

    print(f"\n[NODE]: {node.get('label')} ({node.get('id')})")
    print(f"Domain: {node.get('domain')} | Subdomain: {node.get('subdomain')}")
    print(f"Summary: {node.get('summary')}")
    
    # Branching/Forking View
    print("\n--- Branching/Edges ---")
    for edge in node.get("edges", []):
        print(f"  └──> {edge} (Branch)")
        
    print("\n--- Heuristic Bridges (Web Links) ---")
    bridges = node.get("bridges", [])
    if not bridges:
        print("  (No active bridges)")
    for b in bridges:
        print(f"  [Link] -> {b['target']}")
        print(f"     Heuristic: {b['heuristic_note']}")
        print(f"     Type: {b['link_type']}")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        inspect(sys.argv[1])

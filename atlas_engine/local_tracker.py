import json

NODES_PATH = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"
# Add your registry path here if it differs
REGISTRY_PATH = "/home/becemig/GodotProjects/guardian-interface/data/registry/meta_config.json"

def get_progress():
    try:
        with open(NODES_PATH, 'r') as f:
            nodes = json.load(f)["nodes"]
        
        print(f"--- Guardian Interface: Progress Tracker ---")
        print(f"{'Node ID':<20} | {'Domain':<15} | {'Deps Count':<10}")
        print("-" * 50)
        
        for node in nodes:
            deps = len(node.get("dependencies", []))
            print(f"{node['id']:<20} | {node['domain']:<15} | {deps:<10}")
            
    except FileNotFoundError:
        print("Error: Master nodes file not found. Run sync_atlas.sh first.")

if __name__ == "__main__":
    get_progress()

import json

# Path to your nodes
NODES_PATH = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"

def show_progress():
    with open(NODES_PATH, 'r') as f:
        data = json.load(f)["nodes"]
    
    print(f"{'Node ID':<20} | {'Domain':<15} | {'Dependencies':<10}")
    print("-" * 50)
    for node in data:
        deps_count = len(node.get("dependencies", []))
        print(f"{node['id']:<20} | {node['domain']:<15} | {deps_count:<10}")

if __name__ == "__main__":
    show_progress()

import json
import os

# Define all your source project paths here
SOURCE_PROJECTS = [
    "/home/becemig/GodotProjects/guardian-interface",
    "/home/becemig/GodotProjects/other_project_one",
    "/home/becemig/GodotProjects/other_project_two"
]

def load_all_nodes():
    """Aggregates nodes into memory without modifying the original files."""
    all_nodes = []
    for project_path in SOURCE_PROJECTS:
        path = os.path.join(project_path, "data/study_nodes.json")
        if os.path.exists(path):
            with open(path, 'r') as f:
                data = json.load(f)
                all_nodes.extend(data.get("nodes", []))
    return all_nodes

if __name__ == "__main__":
    # Test: Print the number of nodes found
    nodes = load_all_nodes()
    print(f"Loaded {len(nodes)} nodes from all projects.")

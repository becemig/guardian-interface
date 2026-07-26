import json
import os

# Configuration: Paths to your project folders
SOURCE_PROJECTS = [
    "/home/becemig/GodotProjects/guardian-interface",
    "/home/becemig/GodotProjects/other_project_one",
    "/home/becemig/GodotProjects/other_project_two"
]
MASTER_DESTINATION = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"

def aggregate_to_master():
    master_collection = {"nodes": []}
    seen_ids = set()

    for project in SOURCE_PROJECTS:
        source_path = os.path.join(project, "data/study_nodes.json")
        if os.path.exists(source_path):
            with open(source_path, 'r') as f:
                data = json.load(f)
                for node in data.get("nodes", []):
                    if node["id"] not in seen_ids:
                        master_collection["nodes"].append(node)
                        seen_ids.add(node["id"])
    
    with open(MASTER_DESTINATION, 'w') as f:
        json.dump(master_collection, f, indent=4)
    print(f"Master Library synced: {len(master_collection['nodes'])} nodes.")

if __name__ == "__main__":
    aggregate_to_master()

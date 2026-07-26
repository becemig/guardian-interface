import json
import os

def merge_projects(project_paths):
    master_nodes = []
    seen_ids = set()
    
    # Ensure master directory exists
    os.makedirs("/home/becemig/GodotProjects/guardian-interface/data/", exist_ok=True)
    
    for path in project_paths:
        file_path = os.path.join(path, "data/study_nodes.json")
        if os.path.exists(file_path):
            print(f"Extracting from: {file_path}")
            with open(file_path, 'r') as f:
                data = json.load(f).get("nodes", [])
                for node in data:
                    if node["id"] not in seen_ids:
                        master_nodes.append(node)
                        seen_ids.add(node["id"])
        else:
            print(f"Skipping (no data found): {file_path}")
    
    with open("/home/becemig/GodotProjects/guardian-interface/data/study_nodes.json", 'w') as f:
        json.dump({"nodes": master_nodes}, f, indent=4)
    print(f"Merge Complete: {len(master_nodes)} nodes in Master Library.")

if __name__ == "__main__":
    # Define your project paths here
    projects = [
        "/home/becemig/GodotProjects/guardian-interface",
        "/home/becemig/GodotProjects/other_project_name_here" 
    ]
    merge_projects(projects)

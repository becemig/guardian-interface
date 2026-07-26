import json
import os

MASTER_PATH = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"

def create_node():
    print("--- Guardian Interface: Node Generator ---")
    node_id = input("Enter Node ID: ")
    label = input("Enter Node Label: ")
    domain = input("Enter Domain: ")
    deps = input("Enter Dependencies (comma separated): ")
    link = input("Enter Corridor Link (Fascial Meridian): ")
    
    dependencies = [d.strip() for d in deps.split(",")] if deps else []
    corridor_link = [l.strip() for l in link.split(",")] if link else []
    
    new_node = {
        "id": node_id,
        "label": label,
        "domain": domain,
        "dependencies": dependencies,
        "corridor_link": corridor_link
    }
    
    with open(MASTER_PATH, 'r+') as f:
        data = json.load(f)
        data["nodes"].append(new_node)
        f.seek(0)
        json.dump(data, f, indent=4)
        f.truncate()
        
    print(f"Successfully linked {node_id} to {corridor_link} in Master Library.")

if __name__ == "__main__":
    create_node()

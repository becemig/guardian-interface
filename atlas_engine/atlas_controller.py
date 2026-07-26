import json
import sys

REGISTRY_PATH = "/home/becemig/GodotProjects/guardian-interface/atlas_engine/registry.json"

def load_registry():
    with open(REGISTRY_PATH, 'r') as f: return json.load(f)

def save_registry(registry):
    with open(REGISTRY_PATH, 'w') as f: json.dump(registry, f, indent=4)

def auto_link_nodes():
    """Automatically cross-links nodes sharing the same domain/tags, safely."""
    source_path = "/home/becemig/GodotProjects/guardian-interface/data/study_nodes.json"
    with open(source_path, 'r') as f: source_data = json.load(f)["nodes"]
    
    reg = load_registry()
    for node in reg["nodes"]:
        src_node = next((n for n in source_data if n["id"] == node["original_id"]), None)
        if not src_node: continue
        
        # Use .get() to prevent KeyError if 'domain' is missing
        node_domain = src_node.get("domain")
        if node_domain:
            matches = [n["id"] for n in source_data if n.get("domain") == node_domain and n["id"] != src_node["id"]]
            node["workspace_metadata"]["comparison_links"] = matches[:3]
            
    save_registry(reg)
    print("Nodes successfully cross-linked.")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        if sys.argv[1] == "auto_link": auto_link_nodes()

def assign_corridors():
    """Maps nodes to comparative corridors based on PDF definitions."""
    reg = load_registry()
    # Define mapping rules
    corridor_map = {
        "clin": "Clinical Medicine Corridor",
        "psych": "Mental Health Corridor",
        "chem": "Chemistry/Pharmacology Corridor"
    }
    
    for node in reg["nodes"]:
        nid = node["original_id"]
        # Assign corridor based on ID prefix
        for prefix, corridor in corridor_map.items():
            if nid.startswith(prefix):
                node["workspace_metadata"]["active_corridor"] = corridor
                break
    
    save_registry(reg)
    print("Nodes assigned to comparison corridors.")

def update_mastery(node_id, success_factor):
    """Updates mastery score. success_factor: -1 (failure), +1 (success)."""
    reg = load_registry()
    for node in reg["nodes"]:
        if node["original_id"] == node_id:
            score = node["workspace_metadata"]["mastery_score"]
            node["workspace_metadata"]["mastery_score"] = max(0, min(1.0, score + (success_factor * 0.1)))
            print(f"Node {node_id} mastery updated: {node['workspace_metadata']['mastery_score']}")
    save_registry(reg)

def get_study_list():
    """Identifies nodes needing attention."""
    reg = load_registry()
    needs_study = [n for n in reg["nodes"] if n["workspace_metadata"]["mastery_score"] < 0.5]
    print(f"Study Focus List ({len(needs_study)} nodes):")
    for n in needs_study: print(f"- {n['original_id']} (Score: {n['workspace_metadata']['mastery_score']})")

def update_mastery(node_id, success_factor):
    """Updates mastery score. success_factor: -1 (failure), +1 (success)."""
    reg = load_registry()
    for node in reg["nodes"]:
        if node["original_id"] == node_id:
            score = node["workspace_metadata"]["mastery_score"]
            node["workspace_metadata"]["mastery_score"] = max(0, min(1.0, score + (success_factor * 0.1)))
            print(f"Node {node_id} mastery updated: {node['workspace_metadata']['mastery_score']}")
    save_registry(reg)

def get_study_list():
    """Identifies nodes needing attention."""
    reg = load_registry()
    needs_study = [n for n in reg["nodes"] if n["workspace_metadata"]["mastery_score"] < 0.5]
    print(f"Study Focus List ({len(needs_study)} nodes):")
    for n in needs_study: print(f"- {n['original_id']} (Score: {n['workspace_metadata']['mastery_score']})")

def export_for_godot():
    """Merges registry and glyph metadata for Godot consumption."""
    reg = load_registry()
    with open('/home/becemig/GodotProjects/guardian-interface/atlas_engine/glyph_bridge.json', 'r') as f:
        glyphs = json.load(f)["glyph_map"]
    
    godot_export = []
    for node in reg["nodes"]:
        corridor = node["workspace_metadata"].get("active_corridor", "General")
        export_node = {
            "id": node["original_id"],
            "state": node["workspace_metadata"]["interaction_state"],
            "mastery": node["workspace_metadata"]["mastery_score"],
            "visuals": glyphs.get(corridor, {"color": "#FFFFFF", "symbol": "●", "sound": "default"})
        }
        godot_export.append(export_node)
    
    with open('/home/becemig/GodotProjects/guardian-interface/atlas_engine/godot_atlas_export.json', 'w') as f:
        json.dump(godot_export, f, indent=4)
    print("Export complete: godot_atlas_export.json generated.")

import shutil
import time

def create_snapshot(label):
    """Saves the current registry state as a named base point."""
    timestamp = time.strftime("%Y%m%d_%H%M%S")
    backup_path = f"/home/becemig/GodotProjects/guardian-interface/atlas_engine/history/registry_{label}_{timestamp}.json"
    
    # Ensure history directory exists
    import os
    if not os.path.exists("/home/becemig/GodotProjects/guardian-interface/atlas_engine/history/"):
        os.makedirs("/home/becemig/GodotProjects/guardian-interface/atlas_engine/history/")
        
    shutil.copy(REGISTRY_PATH, backup_path)
    print(f"Base Point created: {backup_path}")

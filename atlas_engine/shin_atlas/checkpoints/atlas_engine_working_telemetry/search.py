import json
import glob
from pathlib import Path
import sys

def search_layers(query):
    query = query.lower()
    # Scans all data.json files in the atlas_layers subdirectories
    layers = glob.glob("atlas_layers/*/data.json")
    
    print(f"--- Searching for: '{query}' across {len(layers)} layers ---")
    
    for layer_file in layers:
        layer_name = Path(layer_file).parent.name
        try:
            with open(layer_file, "r") as f:
                nodes = json.load(f)
                # Ensure it's a list (array)
                if isinstance(nodes, list):
                    for node in nodes:
                        if query in str(node).lower():
                            print(f"[{layer_name}] Found in {node.get('id', 'Unknown')}: {node.get('title')}")
                else:
                    print(f"!!! Error in {layer_file}: Data is not a JSON array")
        except json.JSONDecodeError:
            print(f"!!! Error reading {layer_file}: Invalid JSON format (Check for missing commas or brackets)")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        search_layers(sys.argv[1])
    else:
        print("Usage: python3 atlas_engine/search.py <keyword>")

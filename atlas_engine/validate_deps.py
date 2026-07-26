import json

with open("/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json", 'r') as f:
    data = json.load(f)["nodes"]
    all_ids = {node["id"] for node in data}

for node in data:
    deps = node.get("dependencies", [])
    for dep in deps:
        if dep not in all_ids:
            print(f"CRITICAL: Node '{node['id']}' references missing dependency: '{dep}'")

print("Validation complete.")

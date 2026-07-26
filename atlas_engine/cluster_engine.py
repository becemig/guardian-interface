import json

def group_by_subject(nodes):
    clusters = {}
    for node in nodes:
        subject = node.get("domain", "General")
        if subject not in clusters:
            clusters[subject] = {"nodes": [], "prereqs": []}
        
        # Sort by prerequisite dependency
        if node.get("is_prerequisite"):
            clusters[subject]["prereqs"].append(node)
        else:
            clusters[subject]["nodes"].append(node)
    return clusters

# Load your master container
with open("/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json", 'r') as f:
    data = json.load(f)["nodes"]
    
clusters = group_by_subject(data)

# Export for Studybook UI
with open("/home/becemig/GodotProjects/guardian-interface/master_data/studybook_volumes.json", 'w') as f:
    json.dump(clusters, f, indent=4)
print("Clusters generated for Interactive Studybook.")

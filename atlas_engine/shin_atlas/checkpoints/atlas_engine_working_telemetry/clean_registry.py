import json

path = "/home/becemig/GodotProjects/guardian-interface/data/dr_shin_isu_universe.json"
output_path = "/home/becemig/GodotProjects/guardian-interface/data/dr_shin_isu_universe_cleaned.json"

with open(path, "r") as f:
    data = json.load(f)

TELEMETRY_RELEVANT_DOMAINS = ["biology", "autonomic", "neurology", "psychology"]

for n in data["nodes"]:
    domain = n.get("domain", "").lower()
    label = n.get("label", "").lower()
    
    # If not in relevant domain AND not a known critical node (vagus/stress/sleep), strip signals
    if domain not in TELEMETRY_RELEVANT_DOMAINS and not any(word in label for word in ["vagus", "stress", "sleep"]):
        if "observable_signals" in n:
            n["observable_signals"] = [s for s in n["observable_signals"] if s not in ["hrv", "respiration_rate"]]
        if "properties" in n and "observable_signals" in n["properties"]:
            n["properties"]["observable_signals"] = [s for s in n["properties"]["observable_signals"] if s not in ["hrv", "respiration_rate"]]

with open(output_path, "w") as f:
    json.dump(data, f, indent=2)

print(f"Registry cleaned. Saved to: {output_path}")

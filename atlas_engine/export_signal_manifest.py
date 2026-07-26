import json
from collections import defaultdict
from pathlib import Path

# Updated path to the Shin ISU Universe
SOURCE = Path("/home/becemig/GodotProjects/guardian-interface/data/dr_shin_isu_universe.json")
OUT = Path("/home/becemig/GodotProjects/guardian-interface/reports/signal_manifest.md")

def main():
    if not SOURCE.exists():
        print(f"Error: Source file {SOURCE} not found.")
        return

    data = json.load(open(SOURCE))
    signals = defaultdict(list)

    for node in data.get("nodes", []):
        # Accessing signals directly from the node object as per our new schema
        for sig in node.get("observable_signals", []):
            signals[sig].append({
                "id": node.get("id"),
                "label": node.get("label"),
                "subdomain": node.get("subdomain", "General")
            })

    OUT.parent.mkdir(exist_ok=True)

    with open(OUT, "w") as f:
        f.write("# Guardian Suit Signal Manifest\n\n")
        f.write(f"Generated from: {SOURCE.name}\n\n")
        for sig, nodes in sorted(signals.items()):
            f.write(f"## {sig}\n\n")
            for n in nodes:
                f.write(f"- **{n['id']}** | {n['label']} | *{n['subdomain']}*\n")
            f.write("\n")

    print(f"Signal manifest successfully written to {OUT}")

if __name__ == "__main__":
    main()


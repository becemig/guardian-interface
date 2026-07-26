import json
import sys
from pathlib import Path

# Updated to target the Dr. Shin ISU Universe registry
SOURCE = Path("/home/becemig/GodotProjects/guardian-interface/data/dr_shin_isu_universe.json")

def main():
    if len(sys.argv) < 2:
        print("Usage: python3 atlas_engine/query_by_signal.py <signal>")
        print("Example: python3 atlas_engine/query_by_signal.py hrv")
        return

    query = sys.argv[1].lower()
    data = json.load(open(SOURCE))

    matches = []
    for node in data.get("nodes", []):
        # Signals are now at the root level of the node schema
        signals = node.get("observable_signals", [])
        if any(query in sig.lower() for sig in signals):
            matches.append(node)

    print(f"\nSignal query: {query}")
    print(f"Matches: {len(matches)}\n")

    for node in matches:
        print(f"- {node['id']} | {node['label']} | {node.get('subdomain', 'N/A')}")
        print(f"  signals: {', '.join(node.get('observable_signals', []))}")
        print(f"  edges: {', '.join(node.get('edges', []))}")
        print()

if __name__ == "__main__":
    main()


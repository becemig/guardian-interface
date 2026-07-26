import json
from collections import Counter
from pathlib import Path

CURRENT = Path("atlas_layers/layer_01_shin_core/data.json")
SNAPSHOT = Path("reports/atlas_snapshot.json")
DELTA_REPORT = Path("reports/atlas_delta_report.md")

def load_nodes(path):
    with open(path, "r") as f:
        return json.load(f)

def summarize(nodes):
    modules = Counter()
    domains = Counter()
    methods = Counter()

    for node in nodes:
        for x in node.get("guardian_modules", []):
            modules[x] += 1
        for x in node.get("domain_clusters", []):
            domains[x] += 1
        for x in node.get("key_methods", []):
            methods[x] += 1

    return {
        "modules": dict(modules),
        "domains": dict(domains),
        "methods": dict(methods)
    }

def diff_dict(old, new):
    keys = set(old) | set(new)
    return {k: new.get(k, 0) - old.get(k, 0) for k in keys if new.get(k, 0) - old.get(k, 0) != 0}

def write_delta(old_summary, new_summary):
    DELTA_REPORT.parent.mkdir(exist_ok=True)

    with open(DELTA_REPORT, "w") as f:
        f.write("# Atlas Delta Report\n\n")

        for section in ["modules", "domains", "methods"]:
            f.write(f"## {section.title()} Delta\n\n")
            delta = diff_dict(old_summary.get(section, {}), new_summary.get(section, {}))

            if not delta:
                f.write("No change.\n\n")
                continue

            for key, change in sorted(delta.items(), key=lambda x: abs(x[1]), reverse=True):
                sign = "+" if change > 0 else ""
                f.write(f"- **{key}**: {sign}{change}\n")
            f.write("\n")

def main():
    current_nodes = load_nodes(CURRENT)
    current_summary = summarize(current_nodes)

    if SNAPSHOT.exists():
        old_summary = load_nodes(SNAPSHOT)
        write_delta(old_summary, current_summary)
        print(f"Delta report written: {DELTA_REPORT}")
    else:
        print("No previous snapshot found. Creating first snapshot.")

    with open(SNAPSHOT, "w") as f:
        json.dump(current_summary, f, indent=2)

    print(f"Snapshot updated: {SNAPSHOT}")

if __name__ == "__main__":
    main()

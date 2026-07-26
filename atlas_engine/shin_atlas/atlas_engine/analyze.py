import json
from collections import Counter, defaultdict
from pathlib import Path

DATA_PATH = Path("atlas_layers/layer_01_shin_core/data.json")
REPORT_PATH = Path("reports/atlas_analysis_report.md")

def load_nodes():
    with open(DATA_PATH, "r") as f:
        return json.load(f)

def analyze(nodes):
    module_counts = Counter()
    domain_counts = Counter()
    method_counts = Counter()
    module_methods = defaultdict(Counter)

    for node in nodes:
        modules = node.get("guardian_modules", [])
        domains = node.get("domain_clusters", [])
        methods = node.get("key_methods", [])

        for module in modules:
            module_counts[module] += 1

        for domain in domains:
            domain_counts[domain] += 1

        for method in methods:
            method_counts[method] += 1
            for module in modules:
                module_methods[module][method] += 1

    return module_counts, domain_counts, method_counts, module_methods

def write_report(module_counts, domain_counts, method_counts, module_methods):
    REPORT_PATH.parent.mkdir(exist_ok=True)

    with open(REPORT_PATH, "w") as f:
        f.write("# Shin Evidence Atlas Analysis Report\n\n")

        f.write("## Guardian Module Frequency\n\n")
        for module, count in module_counts.most_common():
            f.write(f"- **{module}**: {count}\n")

        f.write("\n## Domain Cluster Centrality\n\n")
        for domain, count in domain_counts.most_common():
            f.write(f"- **{domain}**: {count}\n")

        f.write("\n## Telemetry / Method Priority\n\n")
        for method, count in method_counts.most_common():
            f.write(f"- **{method}**: {count}\n")

        f.write("\n## Module → Method Mapping\n\n")
        for module, methods in module_methods.items():
            f.write(f"### {module}\n")
            for method, count in methods.most_common():
                f.write(f"- {method}: {count}\n")
            f.write("\n")

def main():
    nodes = load_nodes()
    results = analyze(nodes)
    write_report(*results)
    print(f"Analysis complete: {REPORT_PATH}")

if __name__ == "__main__":
    main()

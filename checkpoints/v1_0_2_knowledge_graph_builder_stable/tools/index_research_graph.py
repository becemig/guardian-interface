#!/usr/bin/env python3

from pathlib import Path
import json
import re
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parents[1]
ARCHIVE_DIR = ROOT / "master_research_archive"
OUTPUT_PATH = ARCHIVE_DIR / "knowledge_graph_index.json"


def parse_scalar(value: str):
    value = value.strip()

    if value == "":
        return ""

    if value.startswith("[") and value.endswith("]"):
        inner = value[1:-1].strip()
        if not inner:
            return []
        return [parse_scalar(part.strip()) for part in inner.split(",")]

    if value.startswith('"') and value.endswith('"'):
        return value[1:-1]

    if value.startswith("'") and value.endswith("'"):
        return value[1:-1]

    if value.lower() == "true":
        return True

    if value.lower() == "false":
        return False

    try:
        return int(value)
    except ValueError:
        pass

    try:
        return float(value)
    except ValueError:
        pass

    return value


def parse_front_matter(text: str):
    """
    Very small YAML-like parser for Guardian research notes.
    Supports:
        key: value
        key: [a, b, c]
        key:
          - item1
          - item2
    """

    if not text.startswith("---"):
        return {}, text

    parts = text.split("---", 2)

    if len(parts) < 3:
        return {}, text

    raw_yaml = parts[1]
    body = parts[2].lstrip("\n")

    metadata = {}
    current_list_key = None

    for raw_line in raw_yaml.splitlines():

        line = raw_line.rstrip()

        if not line.strip():
            continue

        if re.match(r"^[A-Za-z0-9_]+:", line):

            key, value = line.split(":", 1)

            key = key.strip()
            value = value.strip()

            if value == "":
                metadata[key] = []
                current_list_key = key
            else:
                metadata[key] = parse_scalar(value)
                current_list_key = None

        elif line.strip().startswith("- ") and current_list_key:

            item = line.strip()[2:].strip()

            metadata[current_list_key].append(
                parse_scalar(item)
            )

    return metadata, body
    
def extract_heading_summary(body: str):
    headings = []

    for line in body.splitlines():
        if line.startswith("#"):
            headings.append(line.strip())

    first_paragraph = ""
    blocks = [block.strip() for block in body.split("\n\n") if block.strip()]

    for block in blocks:
        if not block.startswith("#"):
            first_paragraph = block.replace("\n", " ")
            break

    return {
        "headings": headings,
        "summary": first_paragraph[:500],
    }


def build_index():
    nodes = []
    somatic_node_index = {}
    topic_index = {}
    category_index = {}
    evidence_index = {}

    for md_path in sorted(ARCHIVE_DIR.rglob("*.md")):
        text = md_path.read_text(encoding="utf-8")
        metadata, body = parse_front_matter(text)
        extracted = extract_heading_summary(body)

        node = {
            "uid": metadata.get("uid", md_path.stem),
            "title": metadata.get("title", md_path.stem.replace("_", " ")),
            "category": metadata.get("category", "Uncategorized"),
            "sub_category": metadata.get("sub_category", ""),
            "source_type": metadata.get("source_type", "Unknown"),
            "status": metadata.get("status", "Draft"),
            "associated_somatic_nodes": metadata.get("associated_somatic_nodes", []),
            "target_meridians": metadata.get("target_meridians", []),
            "telemetry_triggers": metadata.get("telemetry_triggers", []),
            "tags": metadata.get("tags", []),
            "citations": metadata.get("citations", []),
            "relative_path": str(md_path.relative_to(ARCHIVE_DIR)),
            "headings": extracted["headings"],
            "summary": extracted["summary"],
        }

        nodes.append(node)

        for somatic_id in node.get("associated_somatic_nodes", []):
            key = str(somatic_id)
            if key not in somatic_node_index:
                somatic_node_index[key] = []
            somatic_node_index[key].append(node["uid"])

        for tag in node.get("tags", []):
            topic_index.setdefault(str(tag), []).append(node["uid"])

        cat = node.get("category", "Uncategorized")
        category_index.setdefault(str(cat), []).append(node["uid"])

        for cite in node.get("citations", []):
            if isinstance(cite, dict):
                author = cite.get("author", "Unknown")
                year = str(cite.get("year", ""))
                key = f"{author} {year}".strip()
            else:
                key = str(cite)
            evidence_index.setdefault(key, []).append(node["uid"])

    graph = {
        "schema_version": "knowledge_graph_index.v2",
        "generated_at": datetime.now(timezone.utc).isoformat(),
        "archive_root": str(ARCHIVE_DIR.relative_to(ROOT)),
        "node_count": len(nodes),
        "nodes": nodes,
        "somatic_node_index": somatic_node_index,
        "topic_index": topic_index,
        "category_index": category_index,
        "evidence_index": evidence_index,
    }

    OUTPUT_PATH.write_text(
        json.dumps(graph, indent=2),
        encoding="utf-8",
    )

    print(f"Wrote {OUTPUT_PATH}")
    print(f"Indexed {len(nodes)} knowledge nodes.")


if __name__ == "__main__":
    build_index()


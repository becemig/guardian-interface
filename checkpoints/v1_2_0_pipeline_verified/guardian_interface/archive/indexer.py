"""
guardian_interface.archive.indexer
Builds the knowledge_graph_index.json from the research archive.
"""

from pathlib import Path
from datetime import datetime, timezone
import json

from guardian_interface.archive.parser import parse_front_matter, extract_heading_summary


def build_index(archive_dir: Path, output_path: Path):
    nodes = []
    somatic_node_index = {}
    topic_index = {}
    category_index = {}
    evidence_index = {}

    for md_path in sorted(archive_dir.rglob("*.md")):
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
            "relative_path": str(md_path.relative_to(archive_dir)),
            "headings": extracted["headings"],
            "summary": extracted["summary"],
        }

        nodes.append(node)

        for somatic_id in node.get("associated_somatic_nodes", []):
            key = str(somatic_id)
            somatic_node_index.setdefault(key, []).append(node["uid"])

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
        "archive_root": str(archive_dir),
        "node_count": len(nodes),
        "nodes": nodes,
        "somatic_node_index": somatic_node_index,
        "topic_index": topic_index,
        "category_index": category_index,
        "evidence_index": evidence_index,
    }

    output_path.write_text(json.dumps(graph, indent=2), encoding="utf-8")
    return len(nodes)

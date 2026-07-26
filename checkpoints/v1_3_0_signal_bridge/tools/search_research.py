#!/usr/bin/env python3

import argparse
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
INDEX_PATH = ROOT / "master_research_archive" / "knowledge_graph_index.json"


def load_index():
    if not INDEX_PATH.exists():
        print("ERROR: knowledge_graph_index.json not found.")
        print("Run: python3 tools/index_research_graph.py")
        raise SystemExit(1)
    return json.loads(INDEX_PATH.read_text(encoding="utf-8"))


def print_node(node):
    print(f"  UID      : {node['uid']}")
    print(f"  Title    : {node['title']}")
    print(f"  Category : {node['category']}")
    print(f"  Path     : {node['relative_path']}")
    print(f"  Somatic  : {node['associated_somatic_nodes']}")
    print(f"  Tags     : {node['tags']}")
    print()


def search_somatic(data, somatic_id):
    key = str(somatic_id)
    uid_list = data["somatic_node_index"].get(key, [])
    nodes = {n["uid"]: n for n in data["nodes"]}
    print(f"Somatic node {somatic_id} -> {len(uid_list)} document(s):")
    print()
    for uid in uid_list:
        if uid in nodes:
            print_node(nodes[uid])


def search_tag(data, tag):
    uid_list = data["topic_index"].get(tag, [])
    nodes = {n["uid"]: n for n in data["nodes"]}
    print(f"Tag '{tag}' -> {len(uid_list)} document(s):")
    print()
    for uid in uid_list:
        if uid in nodes:
            print_node(nodes[uid])


def search_category(data, category):
    uid_list = data["category_index"].get(category, [])
    nodes = {n["uid"]: n for n in data["nodes"]}
    print(f"Category '{category}' -> {len(uid_list)} document(s):")
    print()
    for uid in uid_list:
        if uid in nodes:
            print_node(nodes[uid])


def search_evidence(data, author):
    nodes = {n["uid"]: n for n in data["nodes"]}
    matches = {k: v for k, v in data["evidence_index"].items()
               if author.lower() in k.lower()}
    if not matches:
        print(f"No evidence found matching: {author}")
        return
    for key, uid_list in matches.items():
        print(f"Evidence '{key}' -> {len(uid_list)} document(s):")
        print()
        for uid in uid_list:
            if uid in nodes:
                print_node(nodes[uid])


def search_uid(data, uid):
    nodes = {n["uid"]: n for n in data["nodes"]}
    if uid not in nodes:
        print(f"No node found with UID: {uid}")
        return
    node = nodes[uid]
    print(f"UID      : {node['uid']}")
    print(f"Title    : {node['title']}")
    print(f"Category : {node['category']}")
    print(f"Sub      : {node.get('sub_category','')}")
    print(f"Source   : {node.get('source_type','')}")
    print(f"Status   : {node.get('status','')}")
    print(f"Somatic  : {node['associated_somatic_nodes']}")
    print(f"Meridians: {node.get('target_meridians','')}")
    print(f"Telemetry: {node.get('telemetry_triggers','')}")
    print(f"Tags     : {node['tags']}")
    print(f"Citations: {node.get('citations','')}")
    print(f"Path     : {node['relative_path']}")
    print(f"Summary  : {node.get('summary','')}")

def list_all(data):
    print(f"Archive contains {data['node_count']} knowledge node(s):")
    print()
    for node in data["nodes"]:
        print_node(node)


def main():
    parser = argparse.ArgumentParser(
        description="Search the Guardian Interface research archive."
    )
    parser.add_argument("--somatic", type=int, help="Search by somatic node ID")
    parser.add_argument("--tag", type=str, help="Search by tag")
    parser.add_argument("--category", type=str, help="Search by category")
    parser.add_argument("--evidence", type=str, help="Search by author/citation")
    parser.add_argument("--list", action="store_true", help="List all nodes")
    parser.add_argument("--uid", type=str, help="Look up a node by UID")
    args = parser.parse_args()

    data = load_index()

    if args.uid:
        search_uid(data, args.uid)
    elif args.somatic is not None:
        search_somatic(data, args.somatic)
    elif args.tag:
        search_tag(data, args.tag)
    elif args.category:
        search_category(data, args.category)
    elif args.evidence:
        search_evidence(data, args.evidence)
    elif args.list:
        list_all(data)
    else:
        parser.print_help()


if __name__ == "__main__":
    main()

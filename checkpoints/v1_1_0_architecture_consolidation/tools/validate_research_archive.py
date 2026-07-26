#!/usr/bin/env python3

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ARCHIVE_DIR = ROOT / "master_research_archive"

REQUIRED_FIELDS = [
    "uid",
    "title",
    "category",
    "associated_somatic_nodes",
    "tags",
]

ALLOWED_STATUSES = {
    "Draft",
    "In-Progress Draft",
    "Review",
    "Verified",
    "Published",
}


def parse_front_matter(text):
    if not text.startswith("---"):
        return None, "Missing opening --- front matter delimiter"
    parts = text.split("---", 2)
    if len(parts) < 3:
        return None, "Missing closing --- front matter delimiter"
    raw_yaml = parts[1]
    metadata = {}
    current_list_key = None
    current_dict_item = None
    for raw_line in raw_yaml.splitlines():
        line = raw_line.rstrip()
        if not line.strip():
            continue
        import re
        if re.match(r"^[A-Za-z0-9_]+:", line):
            key, value = line.split(":", 1)
            key = key.strip()
            value = value.strip()
            if value == "":
                metadata[key] = []
                current_list_key = key
                current_dict_item = None
            else:
                if value.startswith("[") and value.endswith("]"):
                    inner = value[1:-1].strip()
                    if not inner:
                        metadata[key] = []
                    else:
                        metadata[key] = [p.strip() for p in inner.split(",")]
                else:
                    metadata[key] = value
                current_list_key = None
                current_dict_item = None
        elif line.strip().startswith("- ") and current_list_key is not None:
            item_str = line.strip()[2:].strip()
            if ":" in item_str:
                sub_key, sub_val = item_str.split(":", 1)
                current_dict_item = {sub_key.strip(): sub_val.strip()}
                metadata[current_list_key].append(current_dict_item)
            else:
                current_dict_item = None
                metadata[current_list_key].append(item_str)
        elif re.match(r"^\s+[A-Za-z0-9_]+:", line) and current_dict_item is not None:
            sub_key, sub_val = line.strip().split(":", 1)
            current_dict_item[sub_key.strip()] = sub_val.strip()
    return metadata, None


def validate_file(md_path):
    errors = []
    text = md_path.read_text(encoding="utf-8")
    metadata, parse_error = parse_front_matter(text)
    if parse_error:
        return [parse_error]
    for field in REQUIRED_FIELDS:
        if field not in metadata:
            errors.append(f"Missing required field: {field}")
    if "uid" in metadata and not metadata["uid"].strip():
        errors.append("Field uid is empty")
    if "title" in metadata and not metadata["title"].strip():
        errors.append("Field title is empty")
    if "associated_somatic_nodes" in metadata:
        nodes = metadata["associated_somatic_nodes"]
        if not isinstance(nodes, list):
            errors.append("associated_somatic_nodes must be a list")
    if "status" in metadata:
        status = metadata["status"].strip()
        if status not in ALLOWED_STATUSES:
            errors.append(f"Unknown status: {status!r}. Allowed: {sorted(ALLOWED_STATUSES)}")
    return errors


def main():
    if not ARCHIVE_DIR.exists():
        print(f"ERROR: archive directory not found: {ARCHIVE_DIR}")
        sys.exit(1)

    md_files = sorted(ARCHIVE_DIR.rglob("*.md"))
    md_files = [f for f in md_files if f.name != "knowledge_graph_index.json"]

    total = 0
    passed = 0
    failed = 0

    print(f"Validating {len(md_files)} document(s) in {ARCHIVE_DIR.relative_to(ROOT)}")
    print()

    for md_path in md_files:
        total += 1
        rel = md_path.relative_to(ARCHIVE_DIR)
        errors = validate_file(md_path)
        if errors:
            failed += 1
            print(f"  FAIL: {rel}")
            for e in errors:
                print(f"        - {e}")
        else:
            passed += 1
            print(f"  PASS: {rel}")

    print()
    print(f"Results: {passed} passed, {failed} failed, {total} total")

    if failed > 0:
        sys.exit(1)
    sys.exit(0)


if __name__ == "__main__":
    main()

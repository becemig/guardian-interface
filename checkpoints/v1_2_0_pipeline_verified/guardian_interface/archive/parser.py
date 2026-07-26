"""
guardian_interface.archive.parser
Shared YAML-like front matter parser for Guardian research notes.
"""

import re


def parse_scalar(value: str):
    value = value.strip()
    if value == "":
        return ""
    if value.startswith("[") and value.endswith("]"):
        inner = value[1:-1].strip()
        if not inner:
            return []
        return [parse_scalar(p.strip()) for p in inner.split(",")]
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
    Parse YAML-like front matter from a Guardian research note.
    Supports: scalar values, inline lists, block lists, block dicts under lists.
    Returns (metadata dict, body str).
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
    current_dict_item = None
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
                current_dict_item = None
            else:
                metadata[key] = parse_scalar(value)
                current_list_key = None
                current_dict_item = None
        elif line.strip().startswith("- ") and current_list_key is not None:
            item_str = line.strip()[2:].strip()
            if ":" in item_str:
                sub_key, sub_val = item_str.split(":", 1)
                current_dict_item = {sub_key.strip(): parse_scalar(sub_val.strip())}
                metadata[current_list_key].append(current_dict_item)
            else:
                current_dict_item = None
                metadata[current_list_key].append(parse_scalar(item_str))
        elif re.match(r"^\s+[A-Za-z0-9_]+:", line) and current_dict_item is not None:
            sub_key, sub_val = line.strip().split(":", 1)
            current_dict_item[sub_key.strip()] = parse_scalar(sub_val.strip())
    return metadata, body


def extract_heading_summary(body: str):
    headings = []
    for line in body.splitlines():
        if line.startswith("#"):
            headings.append(line.strip())
    first_paragraph = ""
    blocks = [b.strip() for b in body.split("\n\n") if b.strip()]
    for block in blocks:
        if not block.startswith("#"):
            first_paragraph = block.replace("\n", " ")
            break
    return {
        "headings": headings,
        "summary": first_paragraph[:500],
    }

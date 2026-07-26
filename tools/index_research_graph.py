#!/usr/bin/env python3
"""
tools/index_research_graph.py
CLI entry point for building the Guardian research archive index.
Delegates all logic to guardian_interface.archive.indexer.
"""

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from guardian_interface.archive.indexer import build_index

ARCHIVE_DIR = ROOT / "master_research_archive"
OUTPUT_PATH = ARCHIVE_DIR / "knowledge_graph_index.json"


if __name__ == "__main__":
    count = build_index(ARCHIVE_DIR, OUTPUT_PATH)
    print(f"Wrote {OUTPUT_PATH}")
    print(f"Indexed {count} knowledge nodes.")

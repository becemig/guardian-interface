#!/usr/bin/env python3
"""
tools/validate_research_archive.py
CLI entry point for validating the Guardian research archive.
Delegates all logic to guardian_interface.archive.validator.
"""

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from guardian_interface.archive.validator import validate_archive

ARCHIVE_DIR = ROOT / "master_research_archive"


if __name__ == "__main__":
    if not ARCHIVE_DIR.exists():
        print(f"ERROR: archive directory not found: {ARCHIVE_DIR}")
        sys.exit(1)

    md_files = list(ARCHIVE_DIR.rglob("*.md"))
    print(f"Validating {len(md_files)} document(s) in {ARCHIVE_DIR.name}")
    print()

    results = validate_archive(ARCHIVE_DIR)

    for path in results["passed"]:
        rel = Path(path).relative_to(ARCHIVE_DIR)
        print(f"  PASS: {rel}")

    for item in results["failed"]:
        rel = Path(item["path"]).relative_to(ARCHIVE_DIR)
        print(f"  FAIL: {rel}")
        for e in item["errors"]:
            print(f"        - {e}")

    passed = len(results["passed"])
    failed = len(results["failed"])
    total = passed + failed
    print()
    print(f"Results: {passed} passed, {failed} failed, {total} total")

    sys.exit(1 if failed > 0 else 0)

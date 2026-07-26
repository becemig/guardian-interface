#!/usr/bin/env python3

from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ARCHIVE_DIR = ROOT / "master_research_archive"


def main():
    print("Guardian Interface Archive Validator")
    print("====================================")
    print()
    print("Scanning...")
    print()

    markdown_files = sorted(ARCHIVE_DIR.rglob("*.md"))

    print(f"Found {len(markdown_files)} Markdown documents.")
    print()

    for md_path in markdown_files:
        relative_path = md_path.relative_to(ARCHIVE_DIR)
        print(f"✓ {relative_path}")

    print()
    print("Archive scan complete.")


if __name__ == "__main__":
    main()


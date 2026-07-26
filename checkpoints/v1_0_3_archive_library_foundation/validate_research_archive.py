#!/usr/bin/env python3

from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from guardian_interface.archive.scanner import scan_archive, relative_archive_path

def main():
    print("Guardian Interface Archive Validator")
    print("====================================")
    print()
    print("Scanning...")
    print()

    markdown_files = scan_archive()

    print(f"Found {len(markdown_files)} Markdown documents.")
    print()

    for md_path in markdown_files:
        print(f"✓ {relative_archive_path(md_path)}")

    print()
    print("Archive scan complete.")


if __name__ == "__main__":
    main()


from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
ARCHIVE_DIR = ROOT / "master_research_archive"


def scan_archive():
    """
    Return all Markdown research documents in the archive.
    """
    return sorted(ARCHIVE_DIR.rglob("*.md"))


def relative_archive_path(path: Path):
    """
    Return a path relative to the research archive root.
    """
    return path.relative_to(ARCHIVE_DIR)


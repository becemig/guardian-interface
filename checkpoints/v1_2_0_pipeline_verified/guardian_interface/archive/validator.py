"""
guardian_interface.archive.validator
Validation logic for Guardian research archive documents.
Importable from any tool or test.
"""

from pathlib import Path
from guardian_interface.archive.parser import parse_front_matter

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


def validate_file(md_path: Path):
    errors = []
    try:
        text = md_path.read_text(encoding="utf-8")
    except IOError as e:
        return False, [f"Cannot read file: {e}"]

    if not text.startswith("---"):
        return False, ["Missing opening --- front matter delimiter"]

    parts = text.split("---", 2)
    if len(parts) < 3:
        return False, ["Missing closing --- front matter delimiter"]

    metadata, body = parse_front_matter(text)

    if not metadata:
        return False, ["Front matter parsed but returned empty metadata"]

    for field in REQUIRED_FIELDS:
        if field not in metadata:
            errors.append(f"Missing required field: {field}")

    if "uid" in metadata and not str(metadata["uid"]).strip():
        errors.append("Field uid is empty")

    if "title" in metadata and not str(metadata["title"]).strip():
        errors.append("Field title is empty")

    if "associated_somatic_nodes" in metadata:
        if not isinstance(metadata["associated_somatic_nodes"], list):
            errors.append("associated_somatic_nodes must be a list")

    if "status" in metadata:
        status = str(metadata["status"]).strip()
        if status not in ALLOWED_STATUSES:
            errors.append(
                f"Unknown status: {status!r}. Allowed: {sorted(ALLOWED_STATUSES)}"
            )

    passed = len(errors) == 0
    return passed, errors


def validate_archive(archive_dir: Path):
    results = {"passed": [], "failed": []}
    for md_path in sorted(archive_dir.rglob("*.md")):
        passed, errors = validate_file(md_path)
        if passed:
            results["passed"].append(str(md_path))
        else:
            results["failed"].append({"path": str(md_path), "errors": errors})
    return results

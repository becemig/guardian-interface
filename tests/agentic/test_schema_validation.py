from __future__ import annotations

import json
import sys
import uuid
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(PROJECT_ROOT))

from langgraph.checkpoint.memory import MemorySaver

from agentic.audit.log import AUDIT_PATH, verify_log
from agentic.review.queue import QUEUE_DIR, load_review_item
from agentic.workflows.evidence_to_lesson import build_graph


def queue_ids() -> set[str]:
    return {
        path.stem
        for path in QUEUE_DIR.glob("review_*.json")
    }


def audit_entries() -> list[dict]:
    if not AUDIT_PATH.exists():
        return []

    return [
        json.loads(line)
        for line in AUDIT_PATH.read_text(encoding="utf-8").splitlines()
        if line.strip()
    ]


valid_graph = build_graph(MemorySaver())
valid_thread_id = f"schema-valid-{uuid.uuid4().hex}"
valid_config = {
    "configurable": {
        "thread_id": valid_thread_id
    }
}

valid_graph.invoke(
    {
        "request_id": f"schema_valid_{uuid.uuid4().hex[:10]}",
        "learning_objective": (
            "Explain that the rendered load values are synthetic "
            "and are not direct force measurements."
        ),
        "display_layer": "biomedical_anatomy"
    },
    config=valid_config
)

valid_snapshot = valid_graph.get_state(valid_config)
valid_review_id = valid_snapshot.values["review_id"]
valid_item = load_review_item(valid_review_id)

assert valid_item["status"] == "needs_human_review"

queue_before_invalid = queue_ids()

invalid_graph = build_graph(MemorySaver())
invalid_thread_id = f"schema-invalid-{uuid.uuid4().hex}"
invalid_config = {
    "configurable": {
        "thread_id": invalid_thread_id
    }
}

invalid_result = invalid_graph.invoke(
    {
        "request_id": "x",
        "learning_objective": "Too short identifier test.",
        "display_layer": "biomedical_anatomy"
    },
    config=invalid_config
)

invalid_snapshot = invalid_graph.get_state(invalid_config)
queue_after_invalid = queue_ids()

assert invalid_result["status"] == "validation_failed"
assert "review_id" not in invalid_snapshot.values
assert queue_before_invalid == queue_after_invalid

blocked_validation_events = [
    entry
    for entry in audit_entries()
    if entry["action"] == "validate_draft"
    and entry["target"]["id"] == "x"
    and entry["outcome"] == "blocked"
]

assert blocked_validation_events, (
    "Expected an audited blocked validation event for invalid draft."
)

audit_valid, audit_message = verify_log()

print({
    "valid_review_id": valid_review_id,
    "valid_review_status": valid_item["status"],
    "invalid_workflow_status": invalid_result["status"],
    "invalid_review_created": "review_id" in invalid_snapshot.values,
    "audit_blocked_event_found": bool(blocked_validation_events),
    "audit_valid": audit_valid,
    "audit_message": audit_message
})

if not audit_valid:
    raise SystemExit(1)

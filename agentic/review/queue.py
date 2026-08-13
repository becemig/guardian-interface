from __future__ import annotations

import fcntl
import json
import os
import time
import uuid
from contextlib import contextmanager
from pathlib import Path
from typing import Any, Iterator

from agentic.audit.log import append_event

QUEUE_DIR = Path("var/review_queue")
LOCK_PATH = QUEUE_DIR / ".queue.lock"

ALLOWED_DECISIONS = {
    "approve",
    "revision_requested",
    "reject"
}


def _now_ms() -> int:
    return time.time_ns() // 1_000_000


def _item_path(review_id: str) -> Path:
    if not review_id.startswith("review_"):
        raise ValueError("review_id must begin with 'review_'.")
    return QUEUE_DIR / f"{review_id}.json"


@contextmanager
def _queue_lock() -> Iterator[None]:
    QUEUE_DIR.mkdir(parents=True, exist_ok=True)

    with LOCK_PATH.open("a+", encoding="utf-8") as handle:
        fcntl.flock(handle.fileno(), fcntl.LOCK_EX)
        try:
            yield
        finally:
            fcntl.flock(handle.fileno(), fcntl.LOCK_UN)


def _atomic_write(path: Path, payload: dict[str, Any]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    temporary_path = path.with_suffix(".json.tmp")

    with temporary_path.open("w", encoding="utf-8") as handle:
        json.dump(payload, handle, indent=2, ensure_ascii=False)
        handle.write("\n")
        handle.flush()
        os.fsync(handle.fileno())

    os.replace(temporary_path, path)


def _load_unlocked(review_id: str) -> dict[str, Any]:
    path = _item_path(review_id)

    if not path.exists():
        raise FileNotFoundError(f"Review item does not exist: {review_id}")

    with path.open(encoding="utf-8") as handle:
        return json.load(handle)


def _approved_roles(item: dict[str, Any]) -> set[str]:
    return {
        decision["reviewer_role"]
        for decision in item["decisions"]
        if decision["decision"] == "approve"
    }


def _derive_status(item: dict[str, Any]) -> str:
    decisions = item["decisions"]

    if any(decision["decision"] == "reject" for decision in decisions):
        return "rejected"

    if any(
        decision["decision"] == "revision_requested"
        for decision in decisions
    ):
        return "revision_requested"

    required_roles = set(item["required_roles"])

    if required_roles.issubset(_approved_roles(item)):
        return "approved"

    return "needs_human_review"


def create_review_item(
    *,
    workflow_thread_id: str,
    module_id: str,
    draft: dict[str, Any],
    risk_reasons: list[str],
    required_roles: list[str],
) -> dict[str, Any]:
    if not required_roles:
        raise ValueError("At least one required reviewer role is required.")

    review_id = f"review_{uuid.uuid4().hex}"
    created_at_ms = _now_ms()

    item = {
        "review_id": review_id,
        "workflow_thread_id": workflow_thread_id,
        "module_id": module_id,
        "status": "needs_human_review",
        "risk_reasons": sorted(set(risk_reasons)),
        "required_roles": sorted(set(required_roles)),
        "draft": draft,
        "created_at_ms": created_at_ms,
        "updated_at_ms": created_at_ms,
        "decisions": [],
        "history": [
            {
                "at_ms": created_at_ms,
                "actor_id": "guardian-agent",
                "action": "created",
                "from_status": None,
                "to_status": "needs_human_review",
                "reason": "Draft requires human review."
            }
        ]
    }

    with _queue_lock():
        _atomic_write(_item_path(review_id), item)

    append_event(
        actor_type="workflow",
        actor_id="guardian-agent",
        action="create_review_item",
        target_type="review_item",
        target_id=review_id,
        outcome="success",
        details={
            "module_id": module_id,
            "workflow_thread_id": workflow_thread_id,
            "risk_reasons": item["risk_reasons"],
            "required_roles": item["required_roles"]
        }
    )

    return item


def load_review_item(review_id: str) -> dict[str, Any]:
    with _queue_lock():
        return _load_unlocked(review_id)


def submit_review_decision(
    *,
    review_id: str,
    decision: str,
    reviewer_id: str,
    reviewer_role: str,
    reason: str,
) -> dict[str, Any]:
    if decision not in ALLOWED_DECISIONS:
        raise ValueError(f"Unsupported decision: {decision}")

    with _queue_lock():
        item = _load_unlocked(review_id)

        if item["status"] in {"approved", "rejected"}:
            raise ValueError(
                f"Closed review item cannot accept decisions: {review_id}"
            )

        if reviewer_role not in item["required_roles"]:
            raise PermissionError(
                f"Role '{reviewer_role}' is not authorized for {review_id}."
            )

        if any(
            prior["reviewer_role"] == reviewer_role
            for prior in item["decisions"]
        ):
            raise ValueError(
                f"Role '{reviewer_role}' already submitted a decision."
            )

        previous_status = item["status"]
        now_ms = _now_ms()

        review_decision = {
            "reviewer_id": reviewer_id,
            "reviewer_role": reviewer_role,
            "decision": decision,
            "reason": reason,
            "at_ms": now_ms
        }

        item["decisions"].append(review_decision)
        item["status"] = _derive_status(item)
        item["updated_at_ms"] = now_ms
        item["history"].append(
            {
                "at_ms": now_ms,
                "actor_id": reviewer_id,
                "actor_role": reviewer_role,
                "action": "submit_review_decision",
                "from_status": previous_status,
                "to_status": item["status"],
                "decision": decision,
                "reason": reason
            }
        )

        _atomic_write(_item_path(review_id), item)

    append_event(
        actor_type="human_reviewer",
        actor_id=reviewer_id,
        action="submit_review_decision",
        target_type="review_item",
        target_id=review_id,
        outcome="success",
        details={
            "reviewer_role": reviewer_role,
            "decision": decision,
            "resulting_status": item["status"],
            "reason": reason
        }
    )

    return item

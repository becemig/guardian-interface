from __future__ import annotations

import fcntl
import hashlib
import hmac
import json
import os
import time
import uuid
from pathlib import Path
from typing import Any

AUDIT_PATH = Path("var/audit/guardian_audit.jsonl")
GENESIS_HASH = "0" * 64


def _key() -> bytes:
    value = os.environ.get("GUARDIAN_AUDIT_HMAC_KEY")
    if not value:
        raise RuntimeError("GUARDIAN_AUDIT_HMAC_KEY is not set.")
    return value.encode("utf-8")


def _canonical_json(value: dict[str, Any]) -> bytes:
    return json.dumps(
        value,
        sort_keys=True,
        ensure_ascii=False,
        separators=(",", ":"),
    ).encode("utf-8")


def _last_hash(path: Path) -> str:
    if not path.exists() or path.stat().st_size == 0:
        return GENESIS_HASH

    with path.open("rb") as handle:
        handle.seek(0, os.SEEK_END)
        offset = min(handle.tell(), 8192)
        handle.seek(-offset, os.SEEK_END)
        return json.loads(handle.read().splitlines()[-1])["entry_hash"]


def append_event(
    *,
    actor_type: str,
    actor_id: str,
    action: str,
    target_type: str,
    target_id: str,
    outcome: str,
    details: dict[str, Any],
    correlation_id: str | None = None,
) -> dict[str, Any]:
    AUDIT_PATH.parent.mkdir(parents=True, exist_ok=True)

    with AUDIT_PATH.open("a+", encoding="utf-8") as handle:
        fcntl.flock(handle.fileno(), fcntl.LOCK_EX)
        try:
            entry = {
                "event_id": str(uuid.uuid4()),
                "timestamp_ms": time.time_ns() // 1_000_000,
                "correlation_id": correlation_id or str(uuid.uuid4()),
                "actor": {"type": actor_type, "id": actor_id},
                "action": action,
                "target": {"type": target_type, "id": target_id},
                "outcome": outcome,
                "details": details,
                "previous_hash": _last_hash(AUDIT_PATH),
            }

            entry["entry_hash"] = hmac.new(
                _key(),
                _canonical_json(entry),
                hashlib.sha256,
            ).hexdigest()

            handle.seek(0, os.SEEK_END)
            handle.write(json.dumps(entry, ensure_ascii=False) + "\n")
            handle.flush()
            os.fsync(handle.fileno())
            return entry
        finally:
            fcntl.flock(handle.fileno(), fcntl.LOCK_UN)


def verify_log(path: Path = AUDIT_PATH) -> tuple[bool, str]:
    if not path.exists():
        return True, "No audit events exist yet."

    previous_hash = GENESIS_HASH

    with path.open(encoding="utf-8") as handle:
        for line_number, line in enumerate(handle, start=1):
            entry = json.loads(line)
            stored_hash = entry.pop("entry_hash", "")

            if entry.get("previous_hash") != previous_hash:
                return False, f"Broken chain at line {line_number}."

            expected_hash = hmac.new(
                _key(),
                _canonical_json(entry),
                hashlib.sha256,
            ).hexdigest()

            if not hmac.compare_digest(stored_hash, expected_hash):
                return False, f"Invalid HMAC at line {line_number}."

            previous_hash = stored_hash

    return True, "Audit chain verified."

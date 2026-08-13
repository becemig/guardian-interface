from pathlib import Path
import sys

PROJECT_ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(PROJECT_ROOT))

from agentic.audit.log import append_event, verify_log

append_event(
    actor_type="test",
    actor_id="local",
    action="audit_smoke_test",
    target_type="test_record",
    target_id="audit-001",
    outcome="success",
    details={"purpose": "verify append-only HMAC chain"}
)

valid, message = verify_log()
print({"valid": valid, "message": message})

if not valid:
    raise SystemExit(1)

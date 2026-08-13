from pathlib import Path
import sys

PROJECT_ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(PROJECT_ROOT))

from langgraph.types import Command

from agentic.audit.log import verify_log
from agentic.review.queue import (
    load_review_item,
    submit_review_decision,
)
from agentic.workflows.evidence_to_lesson import graph


config = {
    "configurable": {
        "thread_id": "langgraph-review-bridge-test-001"
    }
}

initial_result = graph.invoke(
    {
        "request_id": "tai_chi_weight_shift_bridge_v1",
        "learning_objective": (
            "Explain synthetic left and right load transfer "
            "without presenting it as direct force measurement."
        ),
        "display_layer": "biomedical_anatomy"
    },
    config=config
)

snapshot = graph.get_state(config)
review_id = snapshot.values["review_id"]

item_before_review = load_review_item(review_id)

assert item_before_review["status"] == "needs_human_review"
assert item_before_review["workflow_thread_id"] == (
    "tai_chi_weight_shift_bridge_v1"
)
assert initial_result.get("status") != "approved_for_manual_import"

first_decision = submit_review_decision(
    review_id=review_id,
    decision="approve",
    reviewer_id="bridge-content-owner",
    reviewer_role="content_owner",
    reason="The objective and synthetic-data limitation are clear."
)

assert first_decision["status"] == "needs_human_review"

second_decision = submit_review_decision(
    review_id=review_id,
    decision="approve",
    reviewer_id="bridge-biomedical-reviewer",
    reviewer_role="biomedical_reviewer",
    reason="The biomedical terminology is conservative and scoped."
)

assert second_decision["status"] == "approved"

completed_result = graph.invoke(
    Command(resume={"review_id": review_id}),
    config=config
)

assert completed_result["status"] == "approved_for_manual_import"
assert completed_result["review_id"] == review_id

final_item = load_review_item(review_id)
assert final_item["status"] == "approved"

audit_valid, audit_message = verify_log()

print({
    "review_id": review_id,
    "initial_review_status": item_before_review["status"],
    "first_decision_status": first_decision["status"],
    "final_review_status": final_item["status"],
    "workflow_status": completed_result["status"],
    "audit_valid": audit_valid,
    "audit_message": audit_message
})

if not audit_valid:
    raise SystemExit(1)

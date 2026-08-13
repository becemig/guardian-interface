from pathlib import Path
import sys

PROJECT_ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(PROJECT_ROOT))

from agentic.audit.log import verify_log
from agentic.review.queue import (
    create_review_item,
    submit_review_decision,
)

draft = {
    "contract_version": "study_module_draft.v1",
    "module_id": "tai_chi_weight_shift_v1",
    "learning_objective": "Explain simulated left and right load transfer.",
    "display_layer": "biomedical_anatomy",
    "provenance": "synthetic",
    "claims": [
        {
            "claim_id": "claim_synthetic_load",
            "text": "This display depicts synthetic left and right load values.",
            "claim_class": "visualization_status",
            "source_refs": [],
            "limitations": [
                "This visualization is not a force-plate measurement."
            ]
        }
    ],
    "review_status": "needs_human_approval"
}

item = create_review_item(
    workflow_thread_id="lesson-multi-role-test-001",
    module_id=draft["module_id"],
    draft=draft,
    risk_reasons=[
        "synthetic_visualization",
        "biomedical_teaching_content"
    ],
    required_roles=[
        "content_owner",
        "biomedical_reviewer"
    ]
)

review_id = item["review_id"]

first_review = submit_review_decision(
    review_id=review_id,
    decision="approve",
    reviewer_id="local-content-reviewer",
    reviewer_role="content_owner",
    reason="Learning objective and visualization label are appropriate."
)

assert first_review["status"] == "needs_human_review"
assert len(first_review["decisions"]) == 1

final_review = submit_review_decision(
    review_id=review_id,
    decision="approve",
    reviewer_id="local-biomedical-reviewer",
    reviewer_role="biomedical_reviewer",
    reason="Biomedical wording is conservative and clearly limited."
)

assert final_review["status"] == "approved"
assert len(final_review["decisions"]) == 2

valid, message = verify_log()

print({
    "review_id": review_id,
    "first_status": first_review["status"],
    "final_status": final_review["status"],
    "approved_roles": [
        decision["reviewer_role"]
        for decision in final_review["decisions"]
    ],
    "audit_valid": valid,
    "audit_message": message
})

if not valid:
    raise SystemExit(1)

from __future__ import annotations

import json
from pathlib import Path
from typing import Literal, TypedDict

from jsonschema import Draft202012Validator
from langgraph.checkpoint.memory import MemorySaver
from langgraph.graph import END, START, StateGraph
from langgraph.types import interrupt

from agentic.audit.log import append_event
from agentic.review.queue import (
    create_review_item,
    load_review_item,
)


class LessonState(TypedDict, total=False):
    request_id: str
    learning_objective: str
    display_layer: str
    draft: dict
    validation_errors: list[str]
    review_id: str
    review_status: Literal[
        "needs_human_review",
        "approved",
        "revision_requested",
        "rejected",
        "validation_failed"
    ]
    status: str


def create_mock_draft(state: LessonState) -> LessonState:
    draft = {
        "contract_version": "study_module_draft.v1",
        "module_id": state["request_id"],
        "learning_objective": state["learning_objective"],
        "display_layer": state["display_layer"],
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

    append_event(
        actor_type="workflow",
        actor_id="evidence_to_lesson",
        action="create_draft",
        target_type="study_module",
        target_id=state["request_id"],
        outcome="success",
        details={"display_layer": state["display_layer"]}
    )

    return {
        "draft": draft,
        "status": "draft_created"
    }


def validate_draft(state: LessonState) -> LessonState:
    schema_path = (
        Path(__file__).resolve().parents[2]
        / "contracts"
        / "study_module_draft.v1.schema.json"
    )

    schema = json.loads(schema_path.read_text(encoding="utf-8"))
    validator = Draft202012Validator(schema)

    errors = [
        {
            "path": ".".join(str(part) for part in error.absolute_path)
            or "<root>",
            "message": error.message
        }
        for error in sorted(
            validator.iter_errors(state["draft"]),
            key=lambda error: list(error.absolute_path)
        )
    ]

    append_event(
        actor_type="validator",
        actor_id="study_module_schema_validator",
        action="validate_draft",
        target_type="study_module",
        target_id=state["request_id"],
        outcome="success" if not errors else "blocked",
        details={
            "schema": "study_module_draft.v1",
            "validation_errors": errors
        }
    )

    return {
        "validation_errors": errors,
        "status": "validated" if not errors else "validation_failed"
    }


def route_after_validation(state: LessonState) -> str:
    return "valid" if not state["validation_errors"] else "invalid"


def validation_failed(state: LessonState) -> LessonState:
    return {
        "status": "validation_failed"
    }

def create_review_queue_item(state: LessonState) -> LessonState:
    if state.get("review_id"):
        return {
            "status": "review_item_exists"
        }

    required_roles = [
        "content_owner",
        "biomedical_reviewer"
    ]

    risk_reasons = [
        "synthetic_visualization",
        "biomedical_teaching_content"
    ]

    if state["display_layer"] == "jing_jin":
        required_roles.append("traditional_framework_reviewer")
        risk_reasons.append("traditional_framework_content")

    item = create_review_item(
        workflow_thread_id=state["request_id"],
        module_id=state["draft"]["module_id"],
        draft=state["draft"],
        risk_reasons=risk_reasons,
        required_roles=required_roles
    )

    return {
        "review_id": item["review_id"],
        "review_status": item["status"],
        "status": "waiting_for_human_review"
    }


def wait_for_human_review(state: LessonState) -> LessonState:
    review_id = state["review_id"]
    item = load_review_item(review_id)

    if item["status"] == "needs_human_review":
        interrupt({
            "review_type": "study_module",
            "review_id": review_id,
            "module_id": item["module_id"],
            "required_roles": item["required_roles"],
            "current_status": item["status"],
            "instruction": (
                "Submit all required review decisions through the queue, "
                "then resume this workflow thread."
            )
        })

    item = load_review_item(review_id)

    return {
        "review_status": item["status"],
        "status": f"review_{item['status']}"
    }


def route_after_review(state: LessonState) -> str:
    return state["review_status"]


def approved_for_manual_import(state: LessonState) -> LessonState:
    append_event(
        actor_type="workflow",
        actor_id="evidence_to_lesson",
        action="review_workflow_approved",
        target_type="study_module",
        target_id=state["request_id"],
        outcome="success",
        details={
            "review_id": state["review_id"],
            "next_step": "manual_import_only"
        }
    )

    return {
        "status": "approved_for_manual_import"
    }


def revision_requested(state: LessonState) -> LessonState:
    return {
        "status": "revision_requested"
    }


def rejected(state: LessonState) -> LessonState:
    return {
        "status": "rejected"
    }


def still_waiting(state: LessonState) -> LessonState:
    return {
        "status": "waiting_for_human_review"
    }


def build_graph(checkpointer):
    builder = StateGraph(LessonState)

    builder.add_node("create_mock_draft", create_mock_draft)
    builder.add_node("validate_draft", validate_draft)
    builder.add_node("validation_failed", validation_failed)
    builder.add_node(
        "create_review_queue_item",
        create_review_queue_item
    )
    builder.add_node("wait_for_human_review", wait_for_human_review)
    builder.add_node(
        "approved_for_manual_import",
        approved_for_manual_import
    )
    builder.add_node("revision_requested", revision_requested)
    builder.add_node("rejected", rejected)
    builder.add_node("still_waiting", still_waiting)

    builder.add_edge(START, "create_mock_draft")
    builder.add_edge("create_mock_draft", "validate_draft")

    builder.add_conditional_edges(
        "validate_draft",
        route_after_validation,
        {
            "valid": "create_review_queue_item",
            "invalid": "validation_failed"
        }
    )

    builder.add_edge(
        "create_review_queue_item",
        "wait_for_human_review"
    )

    builder.add_conditional_edges(
        "wait_for_human_review",
        route_after_review,
        {
            "approved": "approved_for_manual_import",
            "revision_requested": "revision_requested",
            "rejected": "rejected",
            "needs_human_review": "still_waiting"
        }
    )

    builder.add_edge("approved_for_manual_import", END)
    builder.add_edge("validation_failed", END)
    builder.add_edge("revision_requested", END)
    builder.add_edge("rejected", END)
    builder.add_edge("still_waiting", END)

    return builder.compile(checkpointer=checkpointer)


# In-memory graph remains available for fast local/unit tests.
graph = build_graph(MemorySaver())

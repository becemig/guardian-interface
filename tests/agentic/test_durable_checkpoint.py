from __future__ import annotations

import json
import os
import sqlite3
import subprocess
import sys
import uuid
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(PROJECT_ROOT))

from langgraph.checkpoint.sqlite import SqliteSaver
from langgraph.types import Command

from agentic.audit.log import verify_log
from agentic.review.queue import (
    load_review_item,
    submit_review_decision,
)
from agentic.workflows.evidence_to_lesson import build_graph

CHECKPOINT_DIR = PROJECT_ROOT / "var/checkpoints"
DATABASE_PATH = CHECKPOINT_DIR / "guardian_langgraph.sqlite"
CONTEXT_PATH = CHECKPOINT_DIR / "durable_test_context.json"


def make_graph():
    connection = sqlite3.connect(
        DATABASE_PATH,
        check_same_thread=False
    )
    checkpointer = SqliteSaver(connection)
    checkpointer.setup()
    return build_graph(checkpointer), connection


def start_workflow() -> None:
    CHECKPOINT_DIR.mkdir(parents=True, exist_ok=True)

    thread_id = f"durable-checkpoint-{uuid.uuid4().hex}"
    config = {"configurable": {"thread_id": thread_id}}

    graph, connection = make_graph()

    try:
        result = graph.invoke(
            {
                "request_id": f"durable_lesson_{uuid.uuid4().hex[:10]}",
                "learning_objective": (
                    "Demonstrate that a review-gated Guardian lesson "
                    "workflow survives a Python process restart."
                ),
                "display_layer": "biomedical_anatomy"
            },
            config=config
        )

        snapshot = graph.get_state(config)
        review_id = snapshot.values["review_id"]
        item = load_review_item(review_id)

        assert item["status"] == "needs_human_review"
        assert result.get("status") != "approved_for_manual_import"

        CONTEXT_PATH.write_text(
            json.dumps(
                {
                    "thread_id": thread_id,
                    "review_id": review_id
                },
                indent=2
            ) + "\n"
        )

        print({
            "phase": "started_and_paused",
            "thread_id": thread_id,
            "review_id": review_id,
            "review_status": item["status"]
        })
    finally:
        connection.close()


def resume_workflow() -> None:
    context = json.loads(CONTEXT_PATH.read_text())
    config = {
        "configurable": {
            "thread_id": context["thread_id"]
        }
    }

    first_decision = submit_review_decision(
        review_id=context["review_id"],
        decision="approve",
        reviewer_id="durable-content-owner",
        reviewer_role="content_owner",
        reason="The objective and synthetic-data limitation are explicit."
    )

    assert first_decision["status"] == "needs_human_review"

    second_decision = submit_review_decision(
        review_id=context["review_id"],
        decision="approve",
        reviewer_id="durable-biomedical-reviewer",
        reviewer_role="biomedical_reviewer",
        reason="The biomedical language remains educational and scoped."
    )

    assert second_decision["status"] == "approved"

    graph, connection = make_graph()

    try:
        completed = graph.invoke(
            Command(resume={"review_id": context["review_id"]}),
            config=config
        )

        assert completed["status"] == "approved_for_manual_import"
        assert completed["review_id"] == context["review_id"]

        audit_valid, audit_message = verify_log()

        print({
            "phase": "resumed_in_fresh_python_process",
            "thread_id": context["thread_id"],
            "review_id": context["review_id"],
            "workflow_status": completed["status"],
            "audit_valid": audit_valid,
            "audit_message": audit_message
        })

        if not audit_valid:
            raise SystemExit(1)
    finally:
        connection.close()


def run_full_restart_test() -> None:
    environment = os.environ.copy()
    environment["PYTHONPATH"] = str(PROJECT_ROOT)

    subprocess.run(
        [sys.executable, __file__, "start"],
        cwd=PROJECT_ROOT,
        env=environment,
        check=True
    )

    subprocess.run(
        [sys.executable, __file__, "resume"],
        cwd=PROJECT_ROOT,
        env=environment,
        check=True
    )


if __name__ == "__main__":
    mode = sys.argv[1] if len(sys.argv) > 1 else "full"

    if mode == "start":
        start_workflow()
    elif mode == "resume":
        resume_workflow()
    elif mode == "full":
        run_full_restart_test()
    else:
        raise SystemExit(f"Unknown mode: {mode}")

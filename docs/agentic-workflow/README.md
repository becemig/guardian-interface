# Guardian Agentic Workflow

## First workflow
Evidence-to-Lesson:
research objective → controlled draft → validation → human review → manual Godot import.

## Non-negotiable safety constraints
- No autonomous Godot scene, script, project-setting, Git, or release changes.
- No agent-generated executable assets.
- No unreviewed biomedical, CNS, or TCM teaching content.
- Godot consumes approved data packages only.
- Every workflow transition produces an audit event.

## Runtime directories
- var/audit: HMAC-signed append-only JSONL audit log
- var/drafts: agent-generated draft packages
- var/review_queue: pending human decisions
- var/approved: reviewed content packages

import json
from datetime import datetime
from pathlib import Path

LOG_DIR = Path("research_logs")
LOG_FILE = LOG_DIR / "session_001.jsonl"

def log_payload(payload):
    LOG_DIR.mkdir(exist_ok=True)

    entry = {
        "timestamp": datetime.now().isoformat(timespec="seconds"),
        "active_signal": payload.get("active_signal"),
        "guardian_state": payload.get("guardian_state"),
        "confidence": payload.get("confidence"),
        "matched_count": payload.get("matched_count"),
        "top_node_labels": payload.get("top_node_labels", [])
    }

    with open(LOG_FILE, "a") as f:
        f.write(json.dumps(entry) + "\n")

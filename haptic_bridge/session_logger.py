import asyncio
import json
import time
import uuid
from pathlib import Path

LOG_DIR = Path.home() / "GodotProjects" / "guardian-interface" / "logs"

class SessionLogger:
    """
    Research-grade session logger with UUID4 session IDs.
    Logs node selections, coherence events, and zone activations
    to a per-session JSONL file for IRB-compliant data collection.
    """

    def __init__(self, mode: str = "PureResearch"):
        self.session_id = str(uuid.uuid4())
        self.mode = mode
        self.start_time = time.time()
        LOG_DIR.mkdir(parents=True, exist_ok=True)
        self.log_path = LOG_DIR / f"session_{self.session_id}.jsonl"
        self._write({
            "event": "session_start",
            "session_id": self.session_id,
            "mode": self.mode,
            "start_time": self.start_time,
        })
        print(f"[SessionLogger] Session {self.session_id} started -> {self.log_path}")

    def log_node_selected(self, node: dict, confidence: float = 1.0):
        self._write({
            "event": "node_selected",
            "node_id": node.get("id",""),
            "label": node.get("label",""),
            "subdomain": node.get("subdomain",""),
            "domain": node.get("domain",""),
            "phase": node.get("phase",""),
            "confidence": confidence,
        })

    def log_coherence(self, coherence: float, phase: str = ""):
        self._write({
            "event": "coherence_changed",
            "coherence": coherence,
            "phase": phase,
        })

    def log_zone_activated(self, zone_id: str, amplitude: float, phase: str = ""):
        self._write({
            "event": "zone_activated",
            "zone_id": zone_id,
            "amplitude": amplitude,
            "phase": phase,
        })

    def log_domain_toggled(self, domain: str, enabled: bool):
        self._write({
            "event": "domain_toggled",
            "domain": domain,
            "enabled": enabled,
        })

    def log_reasoning_mode(self, mode: str):
        self._write({
            "event": "reasoning_mode_changed",
            "reasoning_mode": mode,
        })

    def end_session(self):
        duration = time.time() - self.start_time
        self._write({
            "event": "session_end",
            "session_id": self.session_id,
            "duration_seconds": round(duration, 2),
        })
        print(f"[SessionLogger] Session ended. Duration: {duration:.1f}s")

    def _write(self, record: dict):
        record["timestamp"] = time.time()
        record["session_id"] = self.session_id
        with open(self.log_path, "a") as f:
            f.write(json.dumps(record) + "\n")


if __name__ == "__main__":
    logger = SessionLogger(mode="PureResearch")
    logger.log_node_selected({"id":"pat_kidney_yang_def","label":"Kidney Yang Deficiency",
        "subdomain":"Patterns","domain":"TCM"}, confidence=0.9)
    logger.log_coherence(0.82, phase="Water")
    logger.log_zone_activated("dan_tian", 0.7, phase="Water")
    logger.log_domain_toggled("Neurology", True)
    logger.end_session()
    print("log written to:", logger.log_path)

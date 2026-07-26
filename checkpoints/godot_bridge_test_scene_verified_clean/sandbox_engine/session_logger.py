import csv
import os
import uuid


class SessionLogger:
    def __init__(self, path="guardian_session_log.csv", session_id=None):
        self.path = path
        self.session_id = session_id or str(uuid.uuid4())[:8]

        self.fields = [
            "timestamp",
            "session_id",
            "guardian_state",
            "confidence",
            "hrv",
            "respiration_rate",
            "reaction_time",
            "sleep_quality",
            "contributing_signals",
        ]

        if not os.path.exists(self.path):
            with open(self.path, "w", newline="") as f:
                writer = csv.DictWriter(f, fieldnames=self.fields)
                writer.writeheader()

    def log(self, result):
        telemetry = result.get("input", {})

        row = {
            "timestamp": result.get("timestamp"),
            "session_id": self.session_id,
            "guardian_state": result.get("guardian_state"),
            "confidence": result.get("confidence"),
            "hrv": telemetry.get("hrv"),
            "respiration_rate": telemetry.get("respiration_rate"),
            "reaction_time": telemetry.get("reaction_time"),
            "sleep_quality": telemetry.get("sleep_quality"),
            "contributing_signals": "|".join(result.get("contributing_signals", [])),
        }

        with open(self.path, "a", newline="") as f:
            writer = csv.DictWriter(f, fieldnames=self.fields)
            writer.writerow(row)

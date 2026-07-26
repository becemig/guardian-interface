import csv
import os
import uuid

class SessionLogger:
    def __init__(self, filename="guardian_session_log.csv"):
        self.filename = filename
        self.session_id = str(uuid.uuid4())[:8]
        
        # Initialize file with headers if it doesn't exist
        if not os.path.exists(self.filename):
            with open(self.filename, mode='w', newline='') as f:
                writer = csv.writer(f)
                writer.writerow(["timestamp", "session_id", "guardian_state", "confidence", "hrv", "respiration_rate", "reaction_time", "sleep_quality", "contributing_signals"])

    def log(self, telemetry_result):
        with open(self.filename, mode='a', newline='') as f:
            writer = csv.writer(f)
            writer.writerow([
                telemetry_result.get("timestamp"),
                self.session_id,
                telemetry_result.get("guardian_state"),
                telemetry_result.get("confidence"),
                telemetry_result.get("hrv"),
                telemetry_result.get("respiration_rate"),
                telemetry_result.get("reaction_time"),
                telemetry_result.get("sleep_quality"),
                telemetry_result.get("contributing_signals")
            ])

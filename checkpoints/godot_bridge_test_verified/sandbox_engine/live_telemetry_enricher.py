import time
import json
import os

from guardian_state_engine import GuardianStateEngine
from session_logger import SessionLogger


class TelemetryEnricher:
    def __init__(
        self,
        source_file="live_telemetry.json",
        output_file="enriched_telemetry.json",
        bridge_file="godot_bridge_payload.json",
        poll_seconds=0.5,
    ):
        self.source_file = source_file
        self.output_file = output_file
        self.bridge_file = bridge_file
        self.poll_seconds = poll_seconds
        self.engine = GuardianStateEngine()
        self.logger = SessionLogger()
        self.last_mtime = 0

    def watch(self):
        print(f"Watching {self.source_file} for updates...")
        print("Press Ctrl+C to stop.")

        try:
            while True:
                if os.path.exists(self.source_file):
                    current_mtime = os.path.getmtime(self.source_file)

                    if current_mtime > self.last_mtime:
                        ok = self.process_update()
                        if ok:
                            self.last_mtime = current_mtime

                time.sleep(self.poll_seconds)

        except KeyboardInterrupt:
            print("\nWatch mode stopped.")

    def process_update(self):
        try:
            with open(self.source_file, "r") as f:
                telemetry = json.load(f)

            result = self.engine.process_telemetry(telemetry)

            with open(self.output_file, "w") as f:
                json.dump(result, f, indent=2)

            bridge_payload = {
                "timestamp": result.get("timestamp"),
                "guardian_state": result.get("guardian_state"),
                "confidence": result.get("confidence"),
                "hrv": result.get("input", {}).get("hrv"),
                "respiration_rate": result.get("input", {}).get("respiration_rate"),
                "reaction_time": result.get("input", {}).get("reaction_time"),
                "sleep_quality": result.get("input", {}).get("sleep_quality"),
                "contributing_signals": result.get("contributing_signals", []),
            }

            with open(self.bridge_file, "w") as f:
                json.dump(bridge_payload, f, indent=2)

            self.logger.log(result)

            print(
                f"Processed: {result.get('guardian_state')} "
                f"confidence={result.get('confidence')} "
                f"(bridge updated)"
            )

            return True

        except json.JSONDecodeError:
            print("Skipped update: source file was incomplete JSON.")
            return False

        except FileNotFoundError:
            print("Skipped update: source file not found.")
            return False

        except Exception as e:
            print(f"Skipped update: {e}")
            return False


if __name__ == "__main__":
    enricher = TelemetryEnricher()
    enricher.watch()

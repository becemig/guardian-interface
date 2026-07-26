import csv
import time
import argparse
import json


class TelemetryReplay:
    def __init__(self, log_file="guardian_session_log.csv"):
        self.log_file = log_file

    def replay_session(self, session_id=None, playback_speed=1.0, json_output=False):
        print(f"--- Starting Replay: {'All Sessions' if not session_id else session_id} ---")

        with open(self.log_file, mode="r", newline="") as f:
            reader = csv.DictReader(f)

            for row in reader:
                if session_id and row.get("session_id") != session_id:
                    continue

                payload = {
                    "timestamp": row.get("timestamp"),
                    "guardian_state": row.get("guardian_state"),
                    "confidence": float(row.get("confidence", 0) or 0),
                    "hrv": float(row.get("hrv", 0) or 0),
                    "respiration_rate": float(row.get("respiration_rate", 0) or 0),
                    "reaction_time": float(row.get("reaction_time", 0) or 0),
                    "sleep_quality": float(row.get("sleep_quality", 0) or 0),
                    "contributing_signals": row.get("contributing_signals", "").split("|"),
                }

                if json_output:
                    print(json.dumps(payload))
                else:
                    print(
                        f"Time: {payload['timestamp']} | "
                        f"State: {payload['guardian_state']} | "
                        f"Confidence: {payload['confidence']} | "
                        f"HRV: {payload['hrv']}"
                    )

                time.sleep(1.0 / playback_speed)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Replay Guardian Session Telemetry")
    parser.add_argument("--session", type=str, help="Specific session_id to replay")
    parser.add_argument("--speed", type=float, default=1.0, help="Playback speed multiplier")
    parser.add_argument("--json", action="store_true", help="Output each row as JSON")

    args = parser.parse_args()

    replay = TelemetryReplay()
    replay.replay_session(
        session_id=args.session,
        playback_speed=args.speed,
        json_output=args.json,
    )

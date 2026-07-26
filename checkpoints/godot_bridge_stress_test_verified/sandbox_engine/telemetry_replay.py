import csv
import time

class TelemetryReplay:
    def __init__(self, log_file="guardian_session_log.csv"):
        self.log_file = log_file

    def replay_session(self, session_id=None, playback_speed=1.0):
        with open(self.log_file, mode='r') as f:
            reader = csv.DictReader(f)
            for row in reader:
                if session_id and row['session_id'] != session_id:
                    continue
                print(f"Replaying: {row['timestamp']} | State: {row['guardian_state']}")
                time.sleep(1.0 / playback_speed)

if __name__ == "__main__":
    # Quick test harness for the Replay Engine
    replay = TelemetryReplay()
    print("Starting Replay Engine...")
    # Add logic here to select session_id via CLI if desired
    replay.replay_session(playback_speed=2.0)

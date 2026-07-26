import json

from guardian_state_engine import GuardianStateEngine
from session_logger import SessionLogger


INPUT_FILE = "live_telemetry.json"
OUTPUT_FILE = "enriched_telemetry.json"


def main():
    engine = GuardianStateEngine()
    logger = SessionLogger()

    with open(INPUT_FILE, "r") as f:
        telemetry = json.load(f)

    result = engine.process_telemetry(telemetry)

    with open(OUTPUT_FILE, "w") as f:
        json.dump(result, f, indent=2)

    logger.log(result)

    print("Telemetry enriched.")
    print(f"State: {result['guardian_state']}")
    print(f"Confidence: {result['confidence']}")


if __name__ == "__main__":
    main()


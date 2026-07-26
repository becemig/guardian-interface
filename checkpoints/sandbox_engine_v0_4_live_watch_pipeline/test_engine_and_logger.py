from guardian_state_engine import GuardianStateEngine
from session_logger import SessionLogger

engine = GuardianStateEngine()
logger = SessionLogger()

samples = [
    {"hrv": 72, "respiration_rate": 9, "reaction_time": 310, "sleep_quality": 82},
    {"hrv": 51, "respiration_rate": 14, "reaction_time": 480, "sleep_quality": 66},
    {"hrv": 35, "respiration_rate": 22, "reaction_time": 710, "sleep_quality": 38},
]

for sample in samples:
    result = engine.process_telemetry(sample)
    logger.log(result)
    print(result)

print("ok session log written")

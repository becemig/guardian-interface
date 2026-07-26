import json
import time
import random

states = [
    {"hrv": 78, "respiration_rate": 8, "reaction_time": 290, "sleep_quality": 86},
    {"hrv": 55, "respiration_rate": 13, "reaction_time": 430, "sleep_quality": 70},
    {"hrv": 46, "respiration_rate": 16, "reaction_time": 560, "sleep_quality": 60},
    {"hrv": 33, "respiration_rate": 24, "reaction_time": 780, "sleep_quality": 35},
]

for i in range(30):
    sample = random.choice(states)

    with open("live_telemetry.json", "w") as f:
        json.dump(sample, f, indent=2)

    print("burst", i, sample)
    time.sleep(0.2)

print("stress test complete")

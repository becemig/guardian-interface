import csv
from datetime import datetime
from collections import defaultdict


def parse_time(value):
    return datetime.fromisoformat(value.replace("Z", "+00:00"))


def analyze_timeline(path="guardian_session_log.csv"):
    rows = []

    with open(path, newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            rows.append(row)

    if len(rows) < 2:
        print("Not enough rows to calculate durations.")
        return

    durations = defaultdict(float)
    transitions = []

    for i in range(len(rows) - 1):
        current = rows[i]
        nxt = rows[i + 1]

        state = current["guardian_state"]
        t1 = parse_time(current["timestamp"])
        t2 = parse_time(nxt["timestamp"])

        seconds = max(0, (t2 - t1).total_seconds())
        durations[state] += seconds

        if current["guardian_state"] != nxt["guardian_state"]:
            transitions.append((current["guardian_state"], nxt["guardian_state"]))

    print("\n--- Guardian Timeline Analysis ---")
    print(f"Total rows: {len(rows)}")
    print(f"Transitions: {len(transitions)}")

    print("\nState durations:")
    for state, seconds in sorted(durations.items()):
        print(f"{state}: {seconds:.3f} sec")

    if durations:
        dominant = max(durations, key=durations.get)
        print(f"\nDominant state: {dominant}")

    print("\nState transitions:")
    for old, new in transitions:
        print(f"{old} -> {new}")


if __name__ == "__main__":
    analyze_timeline()

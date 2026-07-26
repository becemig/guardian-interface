from datetime import datetime, timezone


class GuardianStateEngine:
    def __init__(self):
        self.current_state = "Monitoring"

    def process_telemetry(self, telemetry):
        scores = {
            "Regulated": 0,
            "Adaptive": 0,
            "Monitoring": 0,
            "Stress Load": 0,
            "Cognitive Load": 0,
            "Fatigued": 0,
        }

        contributing = []

        hrv = telemetry.get("hrv")
        respiration = telemetry.get("respiration_rate")
        reaction_time = telemetry.get("reaction_time")
        sleep_quality = telemetry.get("sleep_quality")

        if hrv is not None:
            if hrv >= 65:
                scores["Regulated"] += 2
                contributing.append("high_hrv")
            elif hrv >= 45:
                scores["Adaptive"] += 1
                contributing.append("moderate_hrv")
            else:
                scores["Stress Load"] += 2
                contributing.append("low_hrv")

        if respiration is not None:
            if 6 <= respiration <= 12:
                scores["Regulated"] += 2
                contributing.append("regulated_respiration")
            elif respiration >= 18:
                scores["Stress Load"] += 2
                contributing.append("elevated_respiration")
            else:
                scores["Monitoring"] += 1
                contributing.append("irregular_respiration")

        if reaction_time is not None:
            if reaction_time <= 350:
                scores["Adaptive"] += 1
                contributing.append("fast_reaction_time")
            elif reaction_time >= 650:
                scores["Cognitive Load"] += 2
                contributing.append("slow_reaction_time")

        if sleep_quality is not None:
            if sleep_quality >= 75:
                scores["Regulated"] += 1
                contributing.append("good_sleep_quality")
            elif sleep_quality < 45:
                scores["Fatigued"] += 2
                contributing.append("low_sleep_quality")

        guardian_state = max(scores, key=scores.get)
        max_score = scores[guardian_state]
        total_score = sum(scores.values()) or 1
        confidence = round(max_score / total_score, 3)

        self.current_state = guardian_state

        return {
            "timestamp": datetime.now(timezone.utc).isoformat(),
            "guardian_state": guardian_state,
            "confidence": confidence,
            "contributing_signals": contributing,
            "state_scores": scores,
            "input": telemetry,
        }


if __name__ == "__main__":
    engine = GuardianStateEngine()

    sample = {
        "hrv": 38,
        "respiration_rate": 21,
        "reaction_time": 720,
        "sleep_quality": 40,
    }

    result = engine.process_telemetry(sample)
    print(result)

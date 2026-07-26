def derive_guardian_state(payload):
    hrv = float(payload.get("hrv", -1.0))
    respiration = float(payload.get("respiration_rate", -1.0))
    reaction = float(payload.get("reaction_time", -1.0))
    sleep = float(payload.get("sleep_quality", -1.0))

    state = "Monitoring"
    confidence = 0.50
    contributing = []

    if sleep >= 0 and sleep < 0.35:
        state = "Fatigued"
        confidence = 0.80
        contributing.append("sleep_quality")

    elif hrv >= 0.70 and respiration > 0 and respiration <= 14:
        state = "Regulated"
        confidence = 0.85
        contributing.extend(["hrv", "respiration_rate"])

    elif respiration >= 18:
        state = "Stress Load"
        confidence = 0.78
        contributing.append("respiration_rate")

    elif reaction >= 0.60:
        state = "Cognitive Load"
        confidence = 0.75
        contributing.append("reaction_time")

    elif hrv >= 0.55:
        state = "Adaptive"
        confidence = 0.68
        contributing.append("hrv")

    return {
        "guardian_state": state,
        "confidence": confidence,
        "contributing_signals": contributing
    }

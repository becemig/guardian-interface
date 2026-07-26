import json

class WuXingMapper:
    PROFILES = {
        "Wood": {"freq": 12.0, "waveform": "sine", "intensity": 0.6},
        "Fire": {"freq": 20.0, "waveform": "burst", "intensity": 0.8},
        "Earth": {"freq": 8.0, "waveform": "pulse", "intensity": 0.5},
        "Metal": {"freq": 16.0, "waveform": "sharp", "intensity": 0.7},
        "Water": {"freq": 5.0, "waveform": "sine", "intensity": 0.4}
    }

    @staticmethod
    def get_profile(phase):
        return WuXingMapper.PROFILES.get(phase, WuXingMapper.PROFILES["Earth"])

class ResonanceEngine:
    def process(self, telemetry):
        profile = WuXingMapper.get_profile(telemetry.get("phase", "Earth"))
        intensity = profile["intensity"] * telemetry.get("coherence", 1.0)
        return {
            "profile_id": telemetry["phase"],
            "frequency_hz": profile["freq"],
            "amplitude": round(intensity, 2)
        }
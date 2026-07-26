import asyncio
import json
import time
from dataclasses import dataclass, field
from typing import Dict, List, Optional

# Wu Xing carrier frequencies (Hz)
WU_XING_FREQ = {
    "Water":  5.0,
    "Wood":  12.0,
    "Fire":  20.0,
    "Earth":  8.0,
    "Metal": 16.0,
}

# Subdomain -> phase mapping for automatic resonance routing
SUBDOMAIN_PHASE = {
    "Wu Xing":       None,
    "Patterns":      None,
    "Symptoms":      None,
    "Materia Medica":None,
    "Acupoints":     None,
    "Anatomy":       "Earth",
    "Physiology":    "Fire",
    "Neurology":     "Water",
    "Neurophysics":  "Water",
    "Biomechanics":  "Wood",
    "Kinesiology":   "Wood",
    "Kinesthetics":  "Wood",
    "Psychology":    "Fire",
    "Trauma Awareness": "Water",
    "Physics":       "Metal",
    "Chemistry":     "Metal",
    "Mathematics":   "Earth",
}

@dataclass
class ResonanceZone:
    """A single haptic zone on the suit."""
    zone_id: str          # e.g. "left_palm", "sternum", "dan_tian"
    meridian: str         # primary meridian association
    phase: str            # Wu Xing phase
    base_freq: float      # Hz — set from WU_XING_FREQ at init
    amplitude: float = 0.0
    active: bool = False

@dataclass
class ResonanceFrame:
    """One output frame sent to haptic dispatcher."""
    timestamp: float
    zones: Dict[str, float]  # zone_id -> amplitude 0.0-1.0
    carrier_freq: float
    phase: str
    confidence: float
    node_id: str = ""
    label: str = ""

class ResonanceEngine:
    """
    Maps study graph node selections to haptic resonance frames.
    Bidirectional: node -> zones, zones -> node suggestions.
    """

    DEFAULT_ZONES = [
        ResonanceZone("left_palm",    "Pericardium", "Fire",  20.0),
        ResonanceZone("right_palm",   "Pericardium", "Fire",  20.0),
        ResonanceZone("dan_tian",     "Ren",         "Water",  5.0),
        ResonanceZone("sternum",      "Ren",         "Fire",  20.0),
        ResonanceZone("left_forearm", "Lung",        "Metal", 16.0),
        ResonanceZone("right_forearm","Large Intestine","Metal",16.0),
        ResonanceZone("left_shin",    "Stomach",     "Earth",  8.0),
        ResonanceZone("right_shin",   "Stomach",     "Earth",  8.0),
        ResonanceZone("low_back",     "Kidney",      "Water",  5.0),
        ResonanceZone("crown",        "Du",          "Fire",  20.0),
    ]

    def __init__(self):
        self.zones: Dict[str, ResonanceZone] = {
            z.zone_id: z for z in self.DEFAULT_ZONES
        }
        self._active_phase = "Water"
        self._active_node_id = ""
        self._confidence = 0.0
        self._frame_callbacks = []

    def register_callback(self, cb):
        """Register async callback: cb(frame: ResonanceFrame)"""
        self._frame_callbacks.append(cb)

    def on_node_selected(self, node: dict):
        """Called when user selects a study node. node = dict from study_nodes.json."""
        phase = self._resolve_phase(node)
        self._active_phase = phase
        self._active_node_id = node.get("id", "")
        self._confidence = node.get("confidence", 1.0)
        frame = self._build_frame(node)
        asyncio.ensure_future(self._emit(frame))

    def _resolve_phase(self, node: dict) -> str:
        """Determine Wu Xing phase from node data."""
        # 1. Explicit tag
        for tag in node.get("tags", []):
            if tag in ("water","wood","fire","earth","metal"):
                return tag.capitalize()
        # 2. Label match for Wu Xing nodes
        label = node.get("label","")
        if label in WU_XING_FREQ:
            return label
        # 3. Subdomain lookup
        sub = node.get("subdomain","")
        mapped = SUBDOMAIN_PHASE.get(sub)
        if mapped:
            return mapped
        # 4. Pattern keyword heuristic
        summary = node.get("summary","").lower()
        if "kidney" in summary or "bone" in summary:
            return "Water"
        if "liver" in summary or "tendon" in summary:
            return "Wood"
        if "heart" in summary or "shen" in summary:
            return "Fire"
        if "spleen" in summary or "stomach" in summary:
            return "Earth"
        if "lung" in summary:
            return "Metal"
        return "Water"  # default

    def _build_frame(self, node: dict) -> "ResonanceFrame":
        phase = self._active_phase
        carrier = WU_XING_FREQ.get(phase, 5.0)
        zone_amps = {}
        for zid, zone in self.zones.items():
            amp = 0.0
            if zone.phase == phase:
                amp += 0.7
            elif zone.phase == self._generating_phase(phase):
                amp += 0.35
            elif zone.phase == self._controlled_phase(phase):
                amp += 0.15
            summary = (node.get("summary","") + " " + node.get("label","")).lower()
            if zone.meridian.lower() in summary:
                amp = min(amp + 0.2, 1.0)
            zone_amps[zid] = round(amp, 3)
        return ResonanceFrame(
            timestamp=__import__("time").time(),
            zones=zone_amps,
            carrier_freq=carrier,
            phase=phase,
            confidence=self._confidence,
            node_id=self._active_node_id,
            label=node.get("label",""),
        )

    def _generating_phase(self, phase: str) -> str:
        sheng = {"Water":"Metal","Wood":"Water","Fire":"Wood","Earth":"Fire","Metal":"Earth"}
        return sheng.get(phase, "Water")

    def _controlled_phase(self, phase: str) -> str:
        ke = {"Water":"Fire","Wood":"Earth","Fire":"Metal","Earth":"Water","Metal":"Wood"}
        return ke.get(phase, "Fire")

    def on_zone_activated(self, zone_id: str) -> list:
        zone = self.zones.get(zone_id)
        if zone is None:
            return []
        return [zone.phase.lower(), zone.meridian.lower()]

    async def _emit(self, frame: "ResonanceFrame"):
        for cb in self._frame_callbacks:
            try:
                await cb(frame)
            except Exception as e:
                print(f"[ResonanceEngine] callback error: {e}")

    def get_zone_list(self) -> list:
        return list(self.zones.keys())

    def set_zone_amplitude(self, zone_id: str, amplitude: float):
        if zone_id in self.zones:
            self.zones[zone_id].amplitude = max(0.0, min(1.0, amplitude))

    def silence_all(self):
        for zone in self.zones.values():
            zone.amplitude = 0.0
            zone.active = False

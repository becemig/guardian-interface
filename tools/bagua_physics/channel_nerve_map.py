"""
channel_nerve_map.py
Guardian Interface -- Bagua Physics Engine

Layer 10: Full 12-channel peripheral nerve overlay.
Maps each classical TCM channel to its primary peripheral nerve territory,
computes per-channel activation from segment forces and neuro-fascial state,
outputs bilateral asymmetry and dominant channel per frame.

Segment index map (16 IMU segments):
  R_FOOT=0  L_FOOT=1  R_SHAN=2  L_SHAN=3  R_THIG=4  L_THIG=5
  PELVIS=6  L_TH=7    U_TH=8    CERV=9
  R_UARM=10 L_UARM=11 R_FARM=12 L_FARM=13 R_HAND=14 L_HAND=15
"""

import numpy as np
from dataclasses import dataclass, field
from typing import List, Dict, Tuple

# ---------------------------------------------------------------------------
# 12-Channel nerve territory definitions
# Each entry: (segment indices, nerve name, element, yin/yang, arm/leg)
# ---------------------------------------------------------------------------
CHANNEL_DEFS = {
    "LU": {
        "name": "Lung",
        "nerve": "Musculocutaneous",
        "segs": [11, 13, 15],
        "element": "Metal",
        "polarity": "Yin",
        "limb": "arm",
        "color": "#BCE2E7",
    },
    "LI": {
        "name": "Large Intestine",
        "nerve": "Radial superficial",
        "segs": [10, 12, 14],
        "element": "Metal",
        "polarity": "Yang",
        "limb": "arm",
        "color": "#FFC553",
    },
    "ST": {
        "name": "Stomach",
        "nerve": "Femoral/Saphenous",
        "segs": [0, 2, 4, 6, 8, 9],
        "element": "Earth",
        "polarity": "Yang",
        "limb": "leg",
        "color": "#E8AF34",
    },
    "SP": {
        "name": "Spleen",
        "nerve": "Saphenous/Obturator",
        "segs": [1, 3, 5, 6, 7],
        "element": "Earth",
        "polarity": "Yin",
        "limb": "leg",
        "color": "#FFC553",
    },
    "HT": {
        "name": "Heart",
        "nerve": "Ulnar",
        "segs": [11, 13, 15],
        "element": "Fire",
        "polarity": "Yin",
        "limb": "arm",
        "color": "#DD6974",
    },
    "SI": {
        "name": "Small Intestine",
        "nerve": "Posterior interosseous",
        "segs": [10, 12, 14],
        "element": "Fire",
        "polarity": "Yang",
        "limb": "arm",
        "color": "#A13544",
    },
    "BL": {
        "name": "Bladder",
        "nerve": "Sciatic/Post cutaneous",
        "segs": [0, 2, 4, 6, 7, 8, 9],
        "element": "Water",
        "polarity": "Yang",
        "limb": "leg",
        "color": "#5591C7",
    },
    "KD": {
        "name": "Kidney",
        "nerve": "Saphenous/Tibial medial",
        "segs": [1, 3, 5, 6, 7, 8],
        "element": "Water",
        "polarity": "Yin",
        "limb": "leg",
        "color": "#006494",
    },
    "PC": {
        "name": "Pericardium",
        "nerve": "Median",
        "segs": [11, 13, 15],
        "element": "Fire",
        "polarity": "Yin",
        "limb": "arm",
        "color": "#D163A7",
    },
    "TW": {
        "name": "Triple Warmer",
        "nerve": "Radial posterior",
        "segs": [10, 12, 14],
        "element": "Fire",
        "polarity": "Yang",
        "limb": "arm",
        "color": "#A86FDF",
    },
    "GB": {
        "name": "Gallbladder",
        "nerve": "Common peroneal/Lat cut",
        "segs": [0, 2, 4, 6, 8, 9],
        "element": "Wood",
        "polarity": "Yang",
        "limb": "leg",
        "color": "#6DAA45",
    },
    "LV": {
        "name": "Liver",
        "nerve": "Saphenous medial/Deep",
        "segs": [1, 3, 5, 6, 7],
        "element": "Wood",
        "polarity": "Yin",
        "limb": "leg",
        "color": "#437A22",
    },
}

CHANNEL_KEYS = list(CHANNEL_DEFS.keys())

# Element pairs for coupled channel analysis
ELEMENT_PAIRS = {
    "Metal": ["LU", "LI"],
    "Earth": ["ST", "SP"],
    "Fire":  ["HT", "SI", "PC", "TW"],
    "Water": ["BL", "KD"],
    "Wood":  ["GB", "LV"],
}

# ---------------------------------------------------------------------------
# Data structures
# ---------------------------------------------------------------------------

@dataclass
class ChannelFrame:
    frame_idx: int
    activation: Dict[str, float] = field(default_factory=dict)
    asymmetry: Dict[str, float] = field(default_factory=dict)
    element_load: Dict[str, float] = field(default_factory=dict)
    dominant_channel: str = "BL"
    dominant_element: str = "Water"
    yin_total: float = 0.0
    yang_total: float = 0.0

# ---------------------------------------------------------------------------
# Core computation
# ---------------------------------------------------------------------------

def _seg_force(frames, seg_idx: int, t: int) -> float:
    """Extract scalar force proxy for a segment at timestep t."""
    # CurvatureFrame: attunement array (J=12) -- use nearest available joint
    # Segment 0-5 map to joints 0-5 (ankle/knee/hip), 6-9 -> joint 3 (spine)
    # 10-15 -> joints 4-7 (arms)
    SEG_TO_JOINT = {
        0: 0, 1: 11, 2: 1, 3: 10, 4: 2, 5: 9,
        6: 3, 7: 3, 8: 3, 9: 3,
        10: 4, 11: 7, 12: 5, 13: 6, 14: 5, 15: 6,
    }
    j = SEG_TO_JOINT.get(seg_idx, 3)
    att = frames[t].attunement
    if j < len(att):
        return float(att[j])
    return 0.0


def _channel_activation(frames, neuro_frames, ch_def: dict, t: int) -> float:
    """Compute activation for one channel at timestep t."""
    segs = ch_def["segs"]
    # Base: mean attunement across channel segments
    base = np.mean([_seg_force(frames, s, t) for s in segs])
    # Modulate by neuro propriocept_field and autonomic_tone
    nf = neuro_frames[t]
    prop_mod = 0.5 + 0.5 * nf.propriocept_field
    # Yin channels amplified by PNS (low autonomic_tone)
    # Yang channels amplified by SNS (high autonomic_tone)
    if ch_def["polarity"] == "Yin":
        auto_mod = 1.0 - 0.3 * nf.autonomic_tone
    else:
        auto_mod = 0.7 + 0.3 * nf.autonomic_tone
    act = float(np.clip(base * prop_mod * auto_mod, 0.0, 1.0))
    return round(act, 4)


def compute_channel_stream(
    curvature_frames,
    neuro_frames,
) -> List[ChannelFrame]:
    T = len(curvature_frames)
    out = []
    for t in range(T):
        activation = {}
        for key, ch_def in CHANNEL_DEFS.items():
            activation[key] = _channel_activation(curvature_frames, neuro_frames, ch_def, t)

        # Bilateral asymmetry per element pair
        asymmetry = {}
        for elem, chs in ELEMENT_PAIRS.items():
            yin_chs  = [c for c in chs if CHANNEL_DEFS[c]["polarity"] == "Yin"]
            yang_chs = [c for c in chs if CHANNEL_DEFS[c]["polarity"] == "Yang"]
            yin_mean  = np.mean([activation[c] for c in yin_chs])  if yin_chs  else 0.0
            yang_mean = np.mean([activation[c] for c in yang_chs]) if yang_chs else 0.0
            asymmetry[elem] = round(float(yang_mean - yin_mean), 4)

        # Element loads
        element_load = {}
        for elem, chs in ELEMENT_PAIRS.items():
            element_load[elem] = round(float(np.mean([activation[c] for c in chs])), 4)

        # Dominant channel
        dom_ch = max(activation, key=lambda k: activation[k])
        dom_elem = CHANNEL_DEFS[dom_ch]["element"]

        # Yin/Yang totals
        yin_keys  = [k for k, v in CHANNEL_DEFS.items() if v["polarity"] == "Yin"]
        yang_keys = [k for k, v in CHANNEL_DEFS.items() if v["polarity"] == "Yang"]
        yin_total  = round(float(np.mean([activation[k] for k in yin_keys])),  4)
        yang_total = round(float(np.mean([activation[k] for k in yang_keys])), 4)

        out.append(ChannelFrame(
            frame_idx=t,
            activation=activation,
            asymmetry=asymmetry,
            element_load=element_load,
            dominant_channel=dom_ch,
            dominant_element=dom_elem,
            yin_total=yin_total,
            yang_total=yang_total,
        ))
    return out

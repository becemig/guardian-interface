"""
fascial_activation.py
Guardian Interface -- Bagua Physics Engine

Computes fascial line activation levels from segment force decomposition.
Maps three overlays simultaneously:
  1. Anatomy Trains myofascial lines (Myers 2014)
  2. Jing Jin sinew channels (twelve classical TCM channels)
  3. Yi Jin Jing activation sequence (Shaolin tendon-changing classic)

Input:  segment longitudinal forces F_long per segment (T, S)
        from Newton-Euler inverse dynamics (BAGUA-IMU-001)
Output: FascialFrame per timestep -- activation per line per overlay
"""

import numpy as np
from dataclasses import dataclass, field
from typing import List, Dict, Tuple


# ---------------------------------------------------------------------------
# Segment index map -- 16 IMU segments
# ---------------------------------------------------------------------------
# Index : Segment name
SEG = {
    "R_FOOT": 0,  "L_FOOT": 1,
    "R_SHAN": 2,  "L_SHAN": 3,
    "R_THIG": 4,  "L_THIG": 5,
    "PELVIS": 6,
    "L_TH":  7,   "U_TH":  8,  "CERV": 9,
    "R_UARM":10,  "L_UARM":11,
    "R_FARM":12,  "L_FARM":13,
    "R_HAND":14,  "L_HAND":15,
}


# ---------------------------------------------------------------------------
# Anatomy Trains line definitions
# Each line: list of segment indices in proximal-to-distal order
# ---------------------------------------------------------------------------
ANATOMY_TRAINS = {
    "SBL": [0, 2, 4, 6, 7, 8, 9],        # Superficial Back Line
    "SFL": [0, 2, 4, 6, 7, 8, 9],        # Superficial Front Line (ant segs)
    "LL_R": [0, 2, 4, 6, 8],             # Lateral Line Right
    "LL_L": [1, 3, 5, 6, 8],             # Lateral Line Left
    "SPL_R": [14, 12, 10, 8, 6, 4, 2],   # Spiral Line Right
    "SPL_L": [15, 13, 11, 8, 6, 5, 3],   # Spiral Line Left
    "ARL_R": [14, 12, 10],               # Arm Line Right (deep front)
    "ARL_L": [15, 13, 11],               # Arm Line Left
    "FFL_R": [10, 12, 14],               # Front Functional Line Right
    "FFL_L": [11, 13, 15],               # Front Functional Line Left
    "DFL": [0, 2, 4, 6, 7, 8, 9],        # Deep Front Line (core)
}


# ---------------------------------------------------------------------------
# Jing Jin sinew channel definitions
# ---------------------------------------------------------------------------
JING_JIN = {
    "BL_JJ":  [0, 2, 4, 6, 7, 8, 9],    # Bladder Jing Jin -- posterior
    "ST_JJ":  [0, 2, 4, 6, 7, 8, 9],    # Stomach Jing Jin -- anterior
    "GB_JJ":  [0, 2, 4, 6, 8, 9],       # Gallbladder Jing Jin -- lateral
    "LV_JJ":  [1, 3, 5, 6, 7],          # Liver Jing Jin -- medial inner thigh
    "KD_JJ":  [0, 2, 4, 6, 7, 8],       # Kidney Jing Jin -- posteromedial
    "SP_JJ":  [1, 3, 5, 6, 7],          # Spleen Jing Jin -- medial
    "LU_JJ":  [11, 13, 15],             # Lung Jing Jin -- anterior arm L
    "HT_JJ":  [11, 13, 15],             # Heart Jing Jin -- posteromedial arm
    "LI_JJ":  [10, 12, 14],             # Large Intestine Jing Jin -- lateral arm
    "SI_JJ":  [10, 12, 14],             # Small Intestine Jing Jin -- posterior arm
    "PC_JJ":  [11, 13, 15],             # Pericardium Jing Jin -- middle arm
    "TH_JJ":  [10, 12, 14],             # Triple Heater Jing Jin -- dorsal arm
}


# ---------------------------------------------------------------------------
# Yi Jin Jing sequential activation stages
# ---------------------------------------------------------------------------
YI_JIN_JING_STAGES = [
    ("Stage1_Posterior", [0, 2, 4, 6, 7, 8, 9]),   # posterior chain
    ("Stage2_Anterior",  [0, 2, 4, 6, 7, 8, 9]),   # anterior chain
    ("Stage3_Lateral",   [0, 2, 4, 6, 8, 9]),      # lateral chain
    ("Stage4_Spiral",    [14,12,10,8,6,5,3]),       # spiral wrap
    ("Stage5_Arms",      [10,11,12,13,14,15]),      # arm lines
]


@dataclass
class FascialFrame:
    """Per-timestep fascial activation for all three overlay systems."""
    frame_idx: int
    anatomy_trains: Dict[str, float]   # line name -> activation [0,1]
    jing_jin: Dict[str, float]         # channel name -> activation [0,1]
    yi_jin_jing: Dict[str, float]      # stage name -> activation [0,1]
    yjj_current_stage: int             # dominant Yi Jin Jing stage 0-4
    yjj_sequence_score: float          # how well stages follow ideal sequence


# ---------------------------------------------------------------------------
# Line activation computation
# ---------------------------------------------------------------------------

def line_activation(
    f_long: np.ndarray,
    segments: List[int],
    f_max: float,
) -> float:
    """
    Compute activation level of a fascial line from segment forces.
    f_long:   (S,) longitudinal force magnitude per segment this frame
    segments: list of segment indices comprising this line
    f_max:    session maximum force for normalization
    Returns:  activation in [0, 1]
    """
    valid = [s for s in segments if s < len(f_long)]
    if not valid:
        return 0.0
    mean_f = float(np.mean(np.abs(f_long[valid])))
    return float(np.clip(mean_f / max(f_max, 1e-6), 0.0, 1.0))


def yi_jin_jing_stage(
    activations: Dict[str, float],
) -> Tuple[int, float]:
    """
    Determine dominant Yi Jin Jing stage and sequence quality score.
    Ideal sequence: stages activate in order 0->1->2->3->4 then cycle.
    Returns: (dominant_stage_idx, sequence_score)
    """
    vals = [activations.get(s[0], 0.0) for s in YI_JIN_JING_STAGES]
    dominant = int(np.argmax(vals))
    # Sequence score: reward smooth gradient -- penalize random spikes
    # Ideal: activation peaks move through stages 0->4 over time
    # Proxy: smoothness of activation vector (low variance of differences)
    diffs = np.abs(np.diff(vals))
    smoothness = float(1.0 / (1.0 + np.std(diffs)))
    return dominant, smoothness


# ---------------------------------------------------------------------------
# Activation color maps
# ---------------------------------------------------------------------------

# Anatomy Trains: warm amber glow
AT_COLOR  = np.array([0.85, 0.55, 0.10])
# Jing Jin: teal-cyan glow
JJ_COLOR  = np.array([0.10, 0.65, 0.75])
# Yi Jin Jing: gold-white glow
YJJ_COLOR = np.array([0.95, 0.90, 0.40])


def activation_to_rgb(
    activation: float,
    base_color: np.ndarray,
) -> List[float]:
    """Map scalar activation [0,1] to RGB glow color."""
    rgb = base_color * (0.15 + 0.85 * activation)
    return [round(float(c), 4) for c in np.clip(rgb, 0, 1)]


# ---------------------------------------------------------------------------
# Main pipeline
# ---------------------------------------------------------------------------

def compute_fascial_stream(
    f_long: np.ndarray,
) -> List[FascialFrame]:
    """
    Process full session longitudinal force stream.
    f_long: (T, S) longitudinal force magnitude per segment per frame
    Returns: list of FascialFrame per timestep
    """
    T, S = f_long.shape
    f_max = float(np.percentile(np.abs(f_long), 98))
    frames = []
    for t in range(T):
        ft = f_long[t]  # (S,)
        # Anatomy Trains
        at = {
            name: line_activation(ft, segs, f_max)
            for name, segs in ANATOMY_TRAINS.items()
        }
        # Jing Jin
        jj = {
            name: line_activation(ft, segs, f_max)
            for name, segs in JING_JIN.items()
        }
        # Yi Jin Jing
        yjj = {
            stage_name: line_activation(ft, segs, f_max)
            for stage_name, segs in YI_JIN_JING_STAGES
        }
        dominant, seq_score = yi_jin_jing_stage(yjj)
        frames.append(FascialFrame(
            frame_idx=t,
            anatomy_trains=at,
            jing_jin=jj,
            yi_jin_jing=yjj,
            yjj_current_stage=dominant,
            yjj_sequence_score=seq_score,
        ))
    return frames


def fascial_frame_to_json(ff: FascialFrame) -> dict:
    """Serialize FascialFrame to JSON-ready dict for BaguaViewer Layer 3."""
    return {
        "frame": ff.frame_idx,
        "anatomy_trains": {
            k: {
                "activation": round(v, 4),
                "color": activation_to_rgb(v, AT_COLOR),
            }
            for k, v in ff.anatomy_trains.items()
        },
        "jing_jin": {
            k: {
                "activation": round(v, 4),
                "color": activation_to_rgb(v, JJ_COLOR),
            }
            for k, v in ff.jing_jin.items()
        },
        "yi_jin_jing": {
            k: {
                "activation": round(v, 4),
                "color": activation_to_rgb(v, YJJ_COLOR),
            }
            for k, v in ff.yi_jin_jing.items()
        },
        "yjj_stage": ff.yjj_current_stage,
        "yjj_score": round(ff.yjj_sequence_score, 4),
    }

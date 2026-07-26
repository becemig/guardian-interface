"""
ba_gang_diagnosis.py
Guardian Interface -- Bagua Physics Engine

Eight-principle Ba Gang TCM diagnosis from movement physics.

Principles and physical derivations:
  Yin/Yang       -- curvature direction: inward vs outward
  Interior/Exterior -- force depth: structural core vs surface fascial
  Cold/Hot       -- entropy production: low vs high metabolic power
  Deficiency/Excess -- force output vs manifold capacity

Input:  scalar arrays per timestep from upstream modules
Output: BaGangFrame list -- eight meters + dominant pattern string
"""

import numpy as np
from dataclasses import dataclass
from typing import List, Dict, Optional


@dataclass
class BaGangFrame:
    """Eight-principle diagnostic readout for one timestep."""
    frame_idx: int
    yin:       float
    yang:      float
    interior:  float
    exterior:  float
    cold:      float
    hot:       float
    deficient: float
    excess:    float
    pattern:   str
    confidence: float


def compute_yin_yang(global_attunement, dominant_element, mean_lambda):
    element_yang = {"Wood":0.8,"Fire":1.0,"Earth":0.5,"Metal":0.3,"Water":0.1}
    elem_score = element_yang.get(dominant_element, 0.5)
    lambda_score = float(np.clip(mean_lambda / 3.0, 0.0, 1.0))
    attunement_mod = float(np.clip(1.0 - global_attunement / 50.0, 0.0, 1.0))
    yang = elem_score*0.4 + lambda_score*0.4 + attunement_mod*0.2
    return float(np.clip(yang, 0.0, 1.0))


def compute_interior_exterior(sbl_activation, bl_jj_activation, stability_index):
    structural_depth = sbl_activation*0.5 + bl_jj_activation*0.5
    icr_stability = float(np.clip(1.0 - stability_index / 2.0, 0.0, 1.0))
    interior = structural_depth*0.6 + icr_stability*0.4
    return float(np.clip(interior, 0.0, 1.0))


def compute_cold_hot(global_attunement, sheng_flow_score, mean_lambda):
    attunement_heat = float(np.clip(global_attunement / 80.0, 0.0, 1.0))
    sheng_heat = sheng_flow_score
    force_heat = float(np.clip(mean_lambda / 4.0, 0.0, 1.0))
    hot = attunement_heat*0.4 + sheng_heat*0.3 + force_heat*0.3
    return float(np.clip(hot, 0.0, 1.0))


def compute_deficiency_excess(mean_lambda, manifold_volume, global_attunement, session_max_volume):
    volume_ratio = float(np.clip(manifold_volume / max(session_max_volume, 1e-6), 0.0, 1.0))
    lambda_excess = float(np.clip(mean_lambda / 5.0, 0.0, 1.0))
    attunement_excess = float(np.clip(global_attunement / 100.0, 0.0, 1.0))
    excess = volume_ratio*0.4 + lambda_excess*0.3 + attunement_excess*0.3
    return float(np.clip(excess, 0.0, 1.0))


def pattern_string(yang, interior, hot, excess, threshold=0.5):
    yin_yang = "Yang" if yang     > threshold else "Yin"
    in_ex    = "Interior" if interior > threshold else "Exterior"
    ch       = "Hot"  if hot      > threshold else "Cold"
    de       = "Excess" if excess  > threshold else "Deficient"
    return yin_yang + "-" + in_ex + "-" + ch + "-" + de


def pattern_confidence(yang, interior, hot, excess):
    axes = [yang, interior, hot, excess]
    decisiveness = [abs(a - 0.5) * 2.0 for a in axes]
    return float(np.mean(decisiveness))


def compute_ba_gang_stream(
    global_attunements, mean_lambdas, dominant_elements,
    sbl_activations, bl_jj_activations, stability_indices,
    sheng_flow_scores, manifold_volumes):
    T = len(global_attunements)
    session_max_vol = float(np.max(manifold_volumes)) if T > 0 else 1.0
    frames = []
    for t in range(T):
        yang     = compute_yin_yang(global_attunements[t], dominant_elements[t], mean_lambdas[t])
        interior = compute_interior_exterior(sbl_activations[t], bl_jj_activations[t], stability_indices[t])
        hot      = compute_cold_hot(global_attunements[t], sheng_flow_scores[t], mean_lambdas[t])
        excess   = compute_deficiency_excess(mean_lambdas[t], manifold_volumes[t], global_attunements[t], session_max_vol)
        pat  = pattern_string(yang, interior, hot, excess)
        conf = pattern_confidence(yang, interior, hot, excess)
        frames.append(BaGangFrame(
            frame_idx=t,
            yin=round(1.0-yang,4),      yang=round(yang,4),
            interior=round(interior,4), exterior=round(1.0-interior,4),
            cold=round(1.0-hot,4),      hot=round(hot,4),
            deficient=round(1.0-excess,4), excess=round(excess,4),
            pattern=pat, confidence=round(conf,4),
        ))
    return frames


def ba_gang_frame_to_json(bg):
    return {
        "frame":      bg.frame_idx,
        "pattern":    bg.pattern,
        "confidence": bg.confidence,
        "meters": {
            "Yin":       bg.yin,
            "Yang":      bg.yang,
            "Interior":  bg.interior,
            "Exterior":  bg.exterior,
            "Cold":      bg.cold,
            "Hot":       bg.hot,
            "Deficient": bg.deficient,
            "Excess":    bg.excess,
        },
    }

"""mechanotransduction.py
Mechanotransduction and fascial mechanobiology overlay for BaguaViewer Layer 8.
Computes per-joint tissue stress, integrin activation threshold, piezoelectric
potential, and session mechanosensitivity index from curvature and ICR data.
"""
from __future__ import annotations
from dataclasses import dataclass, field
from typing import List
import numpy as np


# --- Thresholds (normalized 0-1 scale) ---
INTEGRIN_THRESHOLD   = 0.45   # stress level that triggers integrin clustering
PIEZO_THRESHOLD      = 0.35   # stress level that activates collagen piezoelectric
YAP_TAZ_THRESHOLD    = 0.60   # stress level for YAP/TAZ nuclear translocation
REMODEL_THRESHOLD    = 0.70   # sustained stress triggering ECM remodeling


@dataclass
class MechFrame:
    frame_idx: int
    stress: List[float]           # per joint, 0-1
    piezo: List[float]            # piezoelectric potential per joint, 0-1
    integrin_active: List[bool]   # integrin clustering threshold crossed
    yap_taz_active: List[bool]    # YAP/TAZ nuclear translocation active
    remodel_zone: List[bool]      # ECM remodeling threshold reached
    mech_index: float             # session mechanosensitivity 0-1
    dominant_zone: int            # joint index with highest stress


def _joint_stress(kappa_k, lambda_k, attune_k):
    """Combine curvature, mechanical advantage, and attunement into tissue stress proxy."""
    # kappa: bending curvature magnitude at joint
    # lambda: ICR mechanical advantage ratio (0-1)
    # attune: normalized attunement (0-1)
    kappa_norm = float(np.clip(np.abs(kappa_k) / 2.0, 0.0, 1.0))
    lam_norm   = float(np.clip(lambda_k, 0.0, 1.0))
    att_norm   = float(np.clip(attune_k, 0.0, 1.0))
    # Stress = weighted combination; curvature and lambda dominate
    stress = 0.50 * kappa_norm + 0.30 * lam_norm + 0.20 * att_norm
    return float(np.clip(stress, 0.0, 1.0))


def _piezo_potential(stress, kappa_k):
    """Collagen piezoelectric potential: rises steeply above PIEZO_THRESHOLD.
    Models mechanically-induced charge separation in collagen fibrils.
    """
    if stress < PIEZO_THRESHOLD:
        return 0.0
    # Sigmoidal rise above threshold, sharpened by local curvature
    x = (stress - PIEZO_THRESHOLD) / (1.0 - PIEZO_THRESHOLD)
    curv_boost = float(np.clip(np.abs(kappa_k) / 3.0, 0.0, 0.4))
    return float(np.clip(x * (1.0 + curv_boost), 0.0, 1.0))


def compute_mechanotransduction_stream(
    curvature_frames,
    icr_frames,
) -> List[MechFrame]:
    """
    curvature_frames: list of CurvatureFrame (from curvature_field.py)
    icr_frames:       list of ICRFrame (from icr_solver.py)
    Returns list of MechFrame per timestep.
    """
    CHAIN_NAMES = [
        None, None, None, "shoulder_girdle",
        "right_elbow", "right_wrist",
        "left_wrist", "left_elbow",
        "shoulder_girdle", None, None, None,
    ]
    results = []
    # Running stress history for sustained remodeling detection
    stress_history = np.zeros((len(curvature_frames), 12))

    for i, cf in enumerate(curvature_frames):
        icf = icr_frames[i]
        icr_by_name = {j.name: j for j in icf.joints if j.icr_valid}
        stress_list = []
        piezo_list  = []

        for k in range(12):
            kappa_k  = float(cf.kappa[k]) if k < len(cf.kappa) else 0.0
            attune_k = float(cf.attunement[k]) if k < len(cf.attunement) else 0.0
            name = CHAIN_NAMES[k]
            jicr = icr_by_name.get(name) if name else None
            lam_k = float(jicr.lambda_val) if jicr else 0.0
            s = _joint_stress(kappa_k, lam_k, attune_k)
            p = _piezo_potential(s, kappa_k)
            stress_list.append(s)
            piezo_list.append(p)

        stress_history[i] = stress_list

        # Sustained remodeling: mean stress over last 10 frames
        window = stress_history[max(0, i-10):i+1]
        sustained = np.mean(window, axis=0)

        integrin_active = [s >= INTEGRIN_THRESHOLD for s in stress_list]
        yap_taz_active  = [s >= YAP_TAZ_THRESHOLD  for s in stress_list]
        remodel_zone    = [float(sustained[k]) >= REMODEL_THRESHOLD for k in range(12)]
        mech_index      = float(np.clip(np.mean(stress_list) * 1.5, 0.0, 1.0))
        dominant_zone   = int(np.argmax(stress_list))

        results.append(MechFrame(
            frame_idx=i,
            stress=stress_list,
            piezo=piezo_list,
            integrin_active=integrin_active,
            yap_taz_active=yap_taz_active,
            remodel_zone=remodel_zone,
            mech_index=mech_index,
            dominant_zone=dominant_zone,
        ))
    return results

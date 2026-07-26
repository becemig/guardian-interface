"""neuro_fascial.py
Neuro-fascial integration overlay for BaguaViewer Layer 9.
Models peripheral mechanoreceptor firing, afferent signal propagation,
proprioceptive field, and autonomic tone proxy from movement data.
"""
from __future__ import annotations
from dataclasses import dataclass, field
from typing import List, Dict
import numpy as np


# Mechanoreceptor firing thresholds (stress-based proxy)
RUFFINI_THRESHOLD  = 0.20  # slow-adapt -- sustained load
PACINI_THRESHOLD   = 0.10  # fast-adapt -- rate of change
GOLGI_THRESHOLD    = 0.55  # Golgi tendon organ -- near max load
SPINDLE_THRESHOLD  = 0.30  # muscle spindle -- stretch

# Afferent fiber propagation delay (frames, approximate)
IA_DELAY   = 1   # Ia -- fastest, direct monosynaptic
IB_DELAY   = 2   # Ib -- Golgi tendon, disynaptic
II_DELAY   = 3   # Group II -- slower spindle afferent


@dataclass
class NeuroFrame:
    frame_idx: int
    ruffini:  List[float]       # per joint 0-1
    pacini:   List[float]       # per joint 0-1
    golgi:    List[bool]        # GTO firing per joint
    spindle:  List[float]       # spindle stretch per joint 0-1
    afferent_pulse: List[float] # propagating signal 0-1 (ankle->wrist)
    propriocept_field: float    # global proprioceptive load 0-1
    autonomic_tone: float       # 0=parasympathetic, 1=sympathetic
    dominant_receptor: str      # which receptor type is most active


def compute_neuro_fascial_stream(
    mech_frames,
    curvature_frames,
) -> List[NeuroFrame]:
    """
    mech_frames:      list of MechFrame from mechanotransduction.py
    curvature_frames: list of CurvatureFrame from curvature_field.py
    Returns list of NeuroFrame per timestep.
    """
    T = len(mech_frames)
    # Store stress history for rate-of-change (Pacini)
    prev_stress = np.zeros(12)
    results = []

    for i, mf in enumerate(mech_frames):
        cf = curvature_frames[i]
        stress = np.array(mf.stress)
        stress_rate = np.abs(stress - prev_stress)
        prev_stress = stress.copy()

        # Ruffini: slow-adapt, sustained load
        ruffini = [float(np.clip((s - RUFFINI_THRESHOLD) / (1.0 - RUFFINI_THRESHOLD), 0, 1))
                   if s >= RUFFINI_THRESHOLD else 0.0 for s in stress]

        # Pacini: fast-adapt, rate of stress change
        pacini = [float(np.clip(r / 0.1, 0, 1)) for r in stress_rate]

        # Golgi tendon organ: fires at high load
        golgi = [bool(s >= GOLGI_THRESHOLD) for s in stress]

        # Muscle spindle: driven by curvature (stretch proxy)
        kappa = np.abs(cf.kappa) if hasattr(cf, "kappa") else np.zeros(12)
        spindle = [float(np.clip(float(kappa[k]) / 2.0, 0, 1))
                   if k < len(kappa) else 0.0 for k in range(12)]

        # Afferent pulse: propagates from ankle (0) to wrist (5/6)
        # Signal strength decays with distance, boosted by stress
        pulse = np.zeros(12)
        for k in range(12):
            dist = min(k, 11-k)  # distance from extremity
            delay_factor = np.exp(-dist * 0.3)
            pulse[k] = float(stress[k] * delay_factor)
        afferent_pulse = [round(float(p), 4) for p in pulse]

        # Proprioceptive field: weighted sum of all receptor activity
        prop = float(np.mean(ruffini) * 0.4 + np.mean(pacini) * 0.3
                     + np.mean(spindle) * 0.3)
        propriocept_field = float(np.clip(prop, 0, 1))

        # Autonomic tone: high curvature + high velocity = sympathetic
        global_att = float(cf.global_attunement)
        mean_stress = float(np.mean(stress))
        autonomic_tone = float(np.clip(mean_stress * 1.2 + global_att * 0.3, 0, 1))

        # Dominant receptor
        scores = {
            "Ruffini": float(np.mean(ruffini)),
            "Pacini":  float(np.mean(pacini)),
            "Golgi":   float(np.sum(golgi)) / 12.0,
            "Spindle": float(np.mean(spindle)),
        }
        dominant_receptor = max(scores, key=scores.get)

        results.append(NeuroFrame(
            frame_idx=i,
            ruffini=ruffini,
            pacini=pacini,
            golgi=golgi,
            spindle=spindle,
            afferent_pulse=afferent_pulse,
            propriocept_field=propriocept_field,
            autonomic_tone=autonomic_tone,
            dominant_receptor=dominant_receptor,
        ))
    return results

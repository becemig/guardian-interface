"""
five_element_map.py
Guardian Interface -- Bagua Physics Engine

Maps movement physics to TCM five-element directional framework.

Pipeline:
  1. Classify velocity and force vectors into five-element sectors
  2. Track Sheng cycle progression over time
  3. Compute herb resonance from sheng-jiang-fu-chen movement quality
  4. Generate per-element flow field for BaguaViewer Layer 6

Input:  CurvatureFrame + ICRFrame + ForceManifold per timestep
Output: FiveElementFrame per timestep
"""

import numpy as np
from dataclasses import dataclass, field
from typing import List, Dict, Tuple


# ---------------------------------------------------------------------------
# Five element physics profiles
# Each element: (y_range, xz_range, name, color_rgb, organ_pair)
# Y axis = vertical (up positive), XZ = horizontal spread
# ---------------------------------------------------------------------------

ELEMENTS = {
    "Wood": {
        "y_min": 0.3,  "y_max": 1.0,
        "xz_min": 0.2, "xz_max": 1.0,
        "color": [0.18, 0.62, 0.18],
        "organs": "Liver / Gallbladder",
        "quality": "rising Yang -- upward outward expansion",
        "sheng_pos": 0,
    },
    "Fire": {
        "y_min": -0.2, "y_max": 0.4,
        "xz_min": 0.7, "xz_max": 1.0,
        "color": [0.90, 0.18, 0.08],
        "organs": "Heart / Small Intestine",
        "quality": "peak Yang -- radial outward dispersal",
        "sheng_pos": 1,
    },
    "Earth": {
        "y_min": -0.3, "y_max": 0.3,
        "xz_min": 0.0, "xz_max": 0.5,
        "color": [0.82, 0.68, 0.08],
        "organs": "Spleen / Stomach",
        "quality": "centering -- lower Dantian convergence",
        "sheng_pos": 2,
    },
    "Metal": {
        "y_min": -0.8, "y_max": -0.2,
        "xz_min": 0.0, "xz_max": 0.6,
        "color": [0.78, 0.80, 0.85],
        "organs": "Lung / Large Intestine",
        "quality": "descending inward -- sinking Qi",
        "sheng_pos": 3,
    },
    "Water": {
        "y_min": -1.0, "y_max": -0.5,
        "xz_min": 0.0, "xz_max": 0.4,
        "color": [0.05, 0.10, 0.60],
        "organs": "Kidney / Bladder",
        "quality": "deep downward -- root and potential",
        "sheng_pos": 4,
    },
}

SHENG_CYCLE = ["Wood", "Fire", "Earth", "Metal", "Water"]


# ---------------------------------------------------------------------------
# Herb resonance database -- sheng jiang fu chen movement qualities
# ---------------------------------------------------------------------------

HERB_RESONANCE = {
    "ascending": [
        ("Chai Hu",    "Bupleurum",         "Wood -- raises Yang Qi"),
        ("Sheng Ma",   "Cimicifuga",        "Wood -- lifts sunken Qi"),
        ("Huang Qi",   "Astragalus",        "Earth -- tonifies and raises"),
        ("Ge Gen",     "Kudzu Root",        "Wood -- releases and ascends"),
        ("Jie Geng",   "Platycodon",        "Metal -- opens and lifts Lung"),
    ],
    "descending": [
        ("Xuan Fu Hua","Inula Flower",      "Metal -- descends rebellious Qi"),
        ("Ban Xia",    "Pinellia",          "Earth -- descends and dries"),
        ("Zhu Shi",    "Bamboo Shavings",   "Fire -- clears and descends"),
        ("Dai Zhe Shi","Hematite",          "Metal -- heavy -- strongly descends"),
        ("Su Zi",      "Perilla Seed",      "Metal -- descends Lung Qi"),
    ],
    "floating": [
        ("Bo He",      "Peppermint",        "Wood -- disperses outward -- surface"),
        ("Jing Jie",   "Schizonepeta",      "Wood -- releases exterior"),
        ("Ma Huang",   "Ephedra",           "Metal -- opens pores -- disperses"),
        ("Fang Feng",  "Siler Root",        "Wood -- scatters Wind"),
        ("Niu Bang Zi","Burdock Seed",      "Metal -- disperses Lung Fire"),
    ],
    "sinking": [
        ("Long Gu",    "Dragon Bone",       "Water -- anchors and sedates"),
        ("Mu Li",      "Oyster Shell",      "Water -- sinks and consolidates"),
        ("Shu Di",     "Prepared Rehmannia","Water -- nourishes and sinks"),
        ("Ze Xie",     "Alisma",            "Water -- drains downward"),
        ("Ci Shi",     "Magnetite",         "Water -- heavy -- anchors Shen"),
    ],
}


@dataclass
class FiveElementFrame:
    """Per-timestep five-element physics classification."""
    frame_idx: int
    element_scores: Dict[str, float]     # element name -> dominance [0,1]
    dominant_element: str                # strongest element this frame
    sheng_position: float                # position in Sheng cycle [0,5]
    sheng_flow_score: float              # smoothness of Sheng progression
    movement_quality: str                # ascending/descending/floating/sinking
    resonant_herbs: List[Tuple]          # top 3 resonant herbs
    flow_vectors: np.ndarray             # per-element flow field (5, 3)


# ---------------------------------------------------------------------------
# Direction classification
# ---------------------------------------------------------------------------

def classify_vector_element(v: np.ndarray) -> Tuple[str, float]:
    """
    Classify a 3D velocity or force vector into the dominant five element.
    v: (3,) vector -- Y is vertical up
    Returns: (element_name, match_score)
    """
    mag = np.linalg.norm(v)
    if mag < 1e-6:
        return "Earth", 0.0
    v_n = v / mag
    y = float(v_n[1])
    xz = float(np.sqrt(v_n[0]**2 + v_n[2]**2))
    best_elem = "Earth"
    best_score = 0.0
    for name, props in ELEMENTS.items():
        y_match  = 1.0 - abs(y  - np.clip(y,  props["y_min"],  props["y_max"]))
        xz_match = 1.0 - abs(xz - np.clip(xz, props["xz_min"], props["xz_max"]))
        score = (y_match + xz_match) / 2.0
        if score > best_score:
            best_score = score
            best_elem = name
    return best_elem, best_score


def element_scores_from_velocities(
    velocities: np.ndarray,
) -> Dict[str, float]:
    """
    Score each element by fraction of joint velocities matching it.
    velocities: (J, 3) joint velocities this frame
    Returns: dict element -> score [0,1]
    """
    counts = {e: 0.0 for e in ELEMENTS}
    total = 0.0
    for v in velocities:
        elem, score = classify_vector_element(v)
        counts[elem] += score
        total += score
    if total < 1e-6:
        return {e: 0.2 for e in ELEMENTS}
    return {e: counts[e] / total for e in ELEMENTS}


def movement_quality_from_scores(
    scores: Dict[str, float],
) -> str:
    """
    Map element scores to sheng-jiang-fu-chen movement quality.
    Wood+Fire dominant -> ascending or floating
    Metal+Water dominant -> descending or sinking
    Earth dominant -> centering
    """
    wood_fire  = scores.get("Wood", 0) + scores.get("Fire", 0)
    metal_water = scores.get("Metal",0) + scores.get("Water",0)
    fire_score = scores.get("Fire", 0)
    water_score = scores.get("Water",0)
    if wood_fire > metal_water:
        return "floating" if fire_score > scores.get("Wood",0) else "ascending"
    elif metal_water > wood_fire:
        return "sinking" if water_score > scores.get("Metal",0) else "descending"
    return "centering"


def resonant_herbs(quality: str, n: int = 3) -> List[Tuple]:
    """Return top n herbs resonant with the movement quality."""
    pool = HERB_RESONANCE.get(quality, HERB_RESONANCE["ascending"])
    return pool[:n]


# ---------------------------------------------------------------------------
# Sheng cycle tracking
# ---------------------------------------------------------------------------

def sheng_position(scores: Dict[str, float]) -> float:
    """
    Compute continuous position in Sheng cycle [0, 5).
    Interpolates between element peaks based on score distribution.
    """
    vals = np.array([scores.get(e, 0.0) for e in SHENG_CYCLE])
    vals = vals / (vals.sum() + 1e-9)
    # Circular weighted mean position
    angles = np.array([i * 2 * np.pi / 5 for i in range(5)])
    sin_mean = np.sum(vals * np.sin(angles))
    cos_mean = np.sum(vals * np.cos(angles))
    angle = np.arctan2(sin_mean, cos_mean) % (2 * np.pi)
    return float(angle * 5 / (2 * np.pi))


def sheng_flow_score(
    positions: List[float],
    window: int = 30,
) -> float:
    """
    Score smoothness of Sheng cycle progression over recent window.
    Ideal: position advances steadily 0->5 (modular).
    Returns [0,1] -- 1.0 = perfect smooth Sheng progression.
    """
    if len(positions) < 2:
        return 0.5
    recent = positions[-window:]
    diffs = np.diff(recent)
    # Wrap diffs for circular space
    diffs = ((diffs + 2.5) % 5.0) - 2.5
    positive_flow = float(np.mean(diffs > 0))   # fraction advancing
    smoothness = float(1.0 / (1.0 + np.std(diffs)))
    return float((positive_flow + smoothness) / 2.0)


def element_flow_vectors(scores: Dict[str, float]) -> np.ndarray:
    """
    Generate canonical flow vector per element weighted by score.
    Returns: (5, 3) -- one vector per element in Sheng order
    Vectors are body-frame directional archetypes.
    """
    archetypes = np.array([
        [ 0.4,  0.8,  0.0],   # Wood:  up-outward
        [ 0.9,  0.1,  0.0],   # Fire:  radial horizontal
        [ 0.0, -0.1,  0.0],   # Earth: centering slight down
        [-0.3, -0.7,  0.0],   # Metal: descending inward
        [ 0.0, -1.0,  0.0],   # Water: straight down
    ])
    weights = np.array([scores.get(e, 0.0) for e in SHENG_CYCLE])
    return archetypes * weights[:, None]


# ---------------------------------------------------------------------------
# Main pipeline
# ---------------------------------------------------------------------------

def compute_five_element_stream(
    positions: np.ndarray,
    dt: float = 1.0 / 120.0,
) -> List[FiveElementFrame]:
    """
    Classify five-element character of movement over full session.
    positions: (T, J, 3) skeleton landmark positions
    Returns: list of FiveElementFrame per timestep
    """
    T, J, _ = positions.shape
    # Compute joint velocities
    vel = np.gradient(positions, dt, axis=0)        # (T, J, 3)
    sheng_history = []
    frames = []
    for t in range(T):
        vt = vel[t]                                 # (J, 3)
        scores = element_scores_from_velocities(vt)
        dominant = max(scores, key=scores.get)
        s_pos = sheng_position(scores)
        sheng_history.append(s_pos)
        s_flow = sheng_flow_score(sheng_history)
        quality = movement_quality_from_scores(scores)
        herbs = resonant_herbs(quality)
        fvecs = element_flow_vectors(scores)
        frames.append(FiveElementFrame(
            frame_idx=t,
            element_scores=scores,
            dominant_element=dominant,
            sheng_position=s_pos,
            sheng_flow_score=s_flow,
            movement_quality=quality,
            resonant_herbs=herbs,
            flow_vectors=fvecs,
        ))
    return frames


def five_element_frame_to_json(fe: FiveElementFrame) -> dict:
    """Serialize FiveElementFrame to JSON-ready dict for BaguaViewer Layer 6."""
    return {
        "frame":       fe.frame_idx,
        "scores":      {k: round(v, 4) for k, v in fe.element_scores.items()},
        "dominant":    fe.dominant_element,
        "sheng_pos":   round(fe.sheng_position, 4),
        "sheng_flow":  round(fe.sheng_flow_score, 4),
        "quality":     fe.movement_quality,
        "herbs":       [[h[0], h[1], h[2]] for h in fe.resonant_herbs],
        "flow_vecs":   [[round(float(v),4) for v in row] for row in fe.flow_vectors],
        "colors":      {e: ELEMENTS[e]["color"] for e in SHENG_CYCLE},
    }


# ---------------------------------------------------------------------------
# Sheng cycle tracking
# ---------------------------------------------------------------------------

def sheng_position(scores: Dict[str, float]) -> float:
    """
    Compute continuous position in Sheng cycle [0, 5).
    Interpolates between element peaks based on score distribution.
    """
    vals = np.array([scores.get(e, 0.0) for e in SHENG_CYCLE])
    vals = vals / (vals.sum() + 1e-9)
    # Circular weighted mean position
    angles = np.array([i * 2 * np.pi / 5 for i in range(5)])
    sin_mean = np.sum(vals * np.sin(angles))
    cos_mean = np.sum(vals * np.cos(angles))
    angle = np.arctan2(sin_mean, cos_mean) % (2 * np.pi)
    return float(angle * 5 / (2 * np.pi))


def sheng_flow_score(
    positions: List[float],
    window: int = 30,
) -> float:
    """
    Score smoothness of Sheng cycle progression over recent window.
    Ideal: position advances steadily 0->5 (modular).
    Returns [0,1] -- 1.0 = perfect smooth Sheng progression.
    """
    if len(positions) < 2:
        return 0.5
    recent = positions[-window:]
    diffs = np.diff(recent)
    # Wrap diffs for circular space
    diffs = ((diffs + 2.5) % 5.0) - 2.5
    positive_flow = float(np.mean(diffs > 0))   # fraction advancing
    smoothness = float(1.0 / (1.0 + np.std(diffs)))
    return float((positive_flow + smoothness) / 2.0)


def element_flow_vectors(scores: Dict[str, float]) -> np.ndarray:
    """
    Generate canonical flow vector per element weighted by score.
    Returns: (5, 3) -- one vector per element in Sheng order
    Vectors are body-frame directional archetypes.
    """
    archetypes = np.array([
        [ 0.4,  0.8,  0.0],   # Wood:  up-outward
        [ 0.9,  0.1,  0.0],   # Fire:  radial horizontal
        [ 0.0, -0.1,  0.0],   # Earth: centering slight down
        [-0.3, -0.7,  0.0],   # Metal: descending inward
        [ 0.0, -1.0,  0.0],   # Water: straight down
    ])
    weights = np.array([scores.get(e, 0.0) for e in SHENG_CYCLE])
    return archetypes * weights[:, None]


# ---------------------------------------------------------------------------
# Main pipeline
# ---------------------------------------------------------------------------

def compute_five_element_stream(
    positions: np.ndarray,
    dt: float = 1.0 / 120.0,
) -> List[FiveElementFrame]:
    """
    Classify five-element character of movement over full session.
    positions: (T, J, 3) skeleton landmark positions
    Returns: list of FiveElementFrame per timestep
    """
    T, J, _ = positions.shape
    # Compute joint velocities
    vel = np.gradient(positions, dt, axis=0)        # (T, J, 3)
    sheng_history = []
    frames = []
    for t in range(T):
        vt = vel[t]                                 # (J, 3)
        scores = element_scores_from_velocities(vt)
        dominant = max(scores, key=scores.get)
        s_pos = sheng_position(scores)
        sheng_history.append(s_pos)
        s_flow = sheng_flow_score(sheng_history)
        quality = movement_quality_from_scores(scores)
        herbs = resonant_herbs(quality)
        fvecs = element_flow_vectors(scores)
        frames.append(FiveElementFrame(
            frame_idx=t,
            element_scores=scores,
            dominant_element=dominant,
            sheng_position=s_pos,
            sheng_flow_score=s_flow,
            movement_quality=quality,
            resonant_herbs=herbs,
            flow_vectors=fvecs,
        ))
    return frames


def five_element_frame_to_json(fe: FiveElementFrame) -> dict:
    """Serialize FiveElementFrame to JSON-ready dict for BaguaViewer Layer 6."""
    return {
        "frame":       fe.frame_idx,
        "scores":      {k: round(v, 4) for k, v in fe.element_scores.items()},
        "dominant":    fe.dominant_element,
        "sheng_pos":   round(fe.sheng_position, 4),
        "sheng_flow":  round(fe.sheng_flow_score, 4),
        "quality":     fe.movement_quality,
        "herbs":       [[h[0], h[1], h[2]] for h in fe.resonant_herbs],
        "flow_vecs":   [[round(float(v),4) for v in row] for row in fe.flow_vectors],
        "colors":      {e: ELEMENTS[e]["color"] for e in SHENG_CYCLE},
    }

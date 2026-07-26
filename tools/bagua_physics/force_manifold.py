"""
force_manifold.py
Guardian Interface -- Bagua Physics Engine

Computes the force manifold: the 3D surface enclosing all force vectors
a practitioner can apply -- the structural sphere of influence.

Pipeline:
  1. Icosphere direction sampling (162 directions, uniform coverage)
  2. Per-direction force capacity from sensor stream or inverse dynamics
  3. Convex hull surface in force space
  4. Volume, asymmetry, gap detection metrics
  5. Real-time manifold update from arm FT sensor events
  6. Five-element sector coloring

Input:  wrench stream (T, 6) [Fx Fy Fz Mx My Mz] or per-arm sensor data
Output: ForceManifold dataclass, JSON for BaguaViewer Layer 5
"""

import numpy as np
from dataclasses import dataclass, field
from typing import List, Tuple, Optional


@dataclass
class ForceManifold:
    """Force manifold surface for one measurement window."""
    directions: np.ndarray      # icosphere directions  (D, 3)
    magnitudes: np.ndarray      # force capacity per direction (D,)
    vertices: np.ndarray        # manifold surface points (D, 3)
    colors: np.ndarray          # per-vertex RGB (D, 3)
    volume: float               # enclosed volume (N*m)^3 proxy
    asymmetry_lr: float         # left-right asymmetry ratio
    gap_dirs: np.ndarray        # directions with low capacity (K, 3)
    element_sectors: dict       # five-element sector mean magnitudes


# ---------------------------------------------------------------------------
# Icosphere direction sampling
# ---------------------------------------------------------------------------

def icosphere_directions(subdivisions: int = 2) -> np.ndarray:
    """
    Generate uniformly distributed directions on unit sphere
    via icosahedron subdivision.
    subdivisions=1 -> 42 dirs, subdivisions=2 -> 162 dirs
    Returns: (D, 3) unit vectors
    """
    # Golden ratio icosahedron vertices
    phi = (1.0 + np.sqrt(5.0)) / 2.0
    verts = []
    for s1 in [-1, 1]:
        for s2 in [-1, 1]:
            verts += [[0, s1, s2*phi], [s1, s2*phi, 0], [s2*phi, 0, s1]]
    verts = np.array(verts, dtype=float)
    verts /= np.linalg.norm(verts, axis=1, keepdims=True)
    # Faces of icosahedron
    faces = [
        [0,11,5],[0,5,1],[0,1,7],[0,7,10],[0,10,11],
        [1,5,9],[5,11,4],[11,10,2],[10,7,6],[7,1,8],
        [3,9,4],[3,4,2],[3,2,6],[3,6,8],[3,8,9],
        [4,9,5],[2,4,11],[6,2,10],[8,6,7],[9,8,1],
    ]
    faces = np.array(faces)
    for _ in range(subdivisions):
        new_faces = []
        mid_cache = {}
        def midpoint(i, j):
            key = (min(i,j), max(i,j))
            if key not in mid_cache:
                m = (verts[i] + verts[j]) / 2.0
                m /= np.linalg.norm(m)
                mid_cache[key] = len(verts)
                verts_list.append(m)
            return mid_cache[key]
        verts_list = list(verts)
        for f3 in faces:
            a,b,c = f3
            ab = midpoint(a,b)
            bc = midpoint(b,c)
            ca = midpoint(c,a)
            new_faces += [[a,ab,ca],[b,bc,ab],[c,ca,bc],[ab,bc,ca]]
        verts = np.array(verts_list)
        faces = np.array(new_faces)
    # Return unique directions
    verts = np.array(verts)
    verts /= np.linalg.norm(verts, axis=1, keepdims=True)
    return verts


# ---------------------------------------------------------------------------
# Five-element direction sectors
# ---------------------------------------------------------------------------

# Five element directional physics -- body-relative frame
# Wood:  upward-outward   (+Y dominant, +XZ spread)
# Fire:  radially outward (all horizontal directions, +XZ plane)
# Earth: downward-inward  (-Y dominant, converging)
# Metal: descending-in    (-Y, inward)
# Water: deep downward    (-Y maximum, straight down)

FIVE_ELEMENT_COLORS = {
    "Wood":  np.array([0.18, 0.55, 0.20]),  # green
    "Fire":  np.array([0.85, 0.15, 0.10]),  # red
    "Earth": np.array([0.80, 0.65, 0.10]),  # yellow
    "Metal": np.array([0.75, 0.78, 0.82]),  # white-silver
    "Water": np.array([0.05, 0.12, 0.55]),  # dark blue
}


def classify_direction_element(dirs: np.ndarray) -> List[str]:
    """
    Classify each direction vector into a five-element sector.
    dirs: (D, 3) unit vectors -- Y is up in body frame
    Returns: list of element names length D
    """
    labels = []
    for d in dirs:
        y = d[1]       # vertical component (+up)
        xz = np.sqrt(d[0]**2 + d[2]**2)   # horizontal spread
        if y > 0.5 and xz > 0.3:
            labels.append("Wood")    # upward-outward
        elif y > -0.2 and xz > 0.7:
            labels.append("Fire")    # radially outward horizontal
        elif y < -0.6:
            labels.append("Water")   # straight down
        elif y < -0.2 and xz < 0.5:
            labels.append("Metal")   # descending inward
        else:
            labels.append("Earth")   # centering
    return labels


def element_sector_colors(
    dirs: np.ndarray,
    magnitudes: np.ndarray,
) -> np.ndarray:
    """
    Color each direction by five-element sector, modulated by magnitude.
    dirs: (D, 3), magnitudes: (D,)
    Returns: (D, 3) RGB
    """
    labels = classify_direction_element(dirs)
    m_max = max(float(np.max(magnitudes)), 1e-6)
    colors = np.zeros((len(dirs), 3))
    for i, lbl in enumerate(labels):
        base = FIVE_ELEMENT_COLORS[lbl]
        brightness = 0.3 + 0.7 * float(magnitudes[i]) / m_max
        colors[i] = base * brightness
    return colors


# ---------------------------------------------------------------------------
# Manifold metrics
# ---------------------------------------------------------------------------

def manifold_volume(magnitudes: np.ndarray, dirs: np.ndarray) -> float:
    """
    Approximate enclosed volume of force manifold.
    Uses mean solid-angle weighting: V ~ (4/3)*pi*mean(r)^3
    magnitudes: (D,) radii in force space
    """
    r_mean = float(np.mean(magnitudes))
    return (4.0 / 3.0) * np.pi * r_mean**3


def lr_asymmetry(magnitudes: np.ndarray, dirs: np.ndarray) -> float:
    """
    Left-right asymmetry: ratio of mean force capacity
    in +X half vs -X half of direction sphere.
    Returns value near 1.0 for symmetric, >1 for right-dominant.
    """
    right_mask = dirs[:, 0] > 0
    left_mask  = dirs[:, 0] < 0
    r_mean = float(np.mean(magnitudes[right_mask])) if right_mask.any() else 0.0
    l_mean = float(np.mean(magnitudes[left_mask]))  if left_mask.any()  else 1e-6
    return r_mean / max(l_mean, 1e-6)


def detect_gaps(
    magnitudes: np.ndarray,
    dirs: np.ndarray,
    threshold: float = 0.4,
) -> np.ndarray:
    """
    Find directions with force capacity below threshold fraction of max.
    Returns: (K, 3) gap direction vectors
    """
    m_max = max(float(np.max(magnitudes)), 1e-6)
    gap_mask = magnitudes < threshold * m_max
    return dirs[gap_mask]


# ---------------------------------------------------------------------------
# Force capacity from wrench stream
# ---------------------------------------------------------------------------

def capacity_from_wrench_stream(
    wrenches: np.ndarray,
    dirs: np.ndarray,
    decay: float = 0.95,
) -> np.ndarray:
    """
    Estimate force capacity per direction from observed wrench stream.
    wrenches: (T, 6) [Fx Fy Fz Mx My Mz]
    dirs:     (D, 3) icosphere directions
    decay:    exponential decay for running maximum
    Returns:  (D,) force capacity per direction
    """
    forces = wrenches[:, :3]                        # (T, 3) force only
    D = len(dirs)
    capacity = np.zeros(D)
    for t in range(len(forces)):
        f_t = forces[t]
        f_mag = np.linalg.norm(f_t)
        if f_mag < 1e-6:
            continue
        f_dir = f_t / f_mag
        # Project onto each icosphere direction
        dots = dirs @ f_dir                         # (D,) cosine similarity
        # Update running max with exponential decay
        contribution = np.maximum(dots, 0.0) * f_mag
        capacity = np.maximum(capacity * decay, contribution)
    return capacity


# ---------------------------------------------------------------------------
# Main pipeline
# ---------------------------------------------------------------------------

# Pre-compute icosphere directions once at module load
_DIRS = icosphere_directions(subdivisions=2)


def compute_force_manifold(
    wrenches: np.ndarray,
    dirs: np.ndarray = None,
    decay: float = 0.95,
    gap_threshold: float = 0.4,
) -> ForceManifold:
    """
    Build force manifold from wrench stream.
    wrenches: (T, 6) sensor data or inverse-dynamics output
    Returns: ForceManifold
    """
    if dirs is None:
        dirs = _DIRS
    magnitudes = capacity_from_wrench_stream(wrenches, dirs, decay)
    # Surface vertices: direction * magnitude
    vertices = dirs * magnitudes[:, None]
    colors = element_sector_colors(dirs, magnitudes)
    vol = manifold_volume(magnitudes, dirs)
    asym = lr_asymmetry(magnitudes, dirs)
    gaps = detect_gaps(magnitudes, dirs, gap_threshold)
    # Five-element sector means
    labels = classify_direction_element(dirs)
    sector_means = {}
    for elem in ["Wood", "Fire", "Earth", "Metal", "Water"]:
        idxs = [i for i, l in enumerate(labels) if l == elem]
        sector_means[elem] = float(np.mean(magnitudes[idxs])) if idxs else 0.0
    return ForceManifold(
        directions=dirs,
        magnitudes=magnitudes,
        vertices=vertices,
        colors=colors,
        volume=vol,
        asymmetry_lr=asym,
        gap_dirs=gaps,
        element_sectors=sector_means,
    )


def manifold_to_json(m: ForceManifold) -> dict:
    """Serialize ForceManifold to JSON-ready dict for BaguaViewer Layer 5."""
    return {
        "vertices":  [[round(float(v), 4) for v in row] for row in m.vertices],
        "colors":    [[round(float(c), 4) for c in row] for row in m.colors],
        "volume":    round(m.volume, 4),
        "asym_lr":   round(m.asymmetry_lr, 4),
        "gap_count": len(m.gap_dirs),
        "elements":  {k: round(v, 4) for k, v in m.element_sectors.items()},
    }

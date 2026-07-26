"""
icr_solver.py
Guardian Interface -- Bagua Physics Engine

Closed-form instantaneous center of rotation solver and leverage mathematics.

Pipeline:
  1. Velocity and angular velocity from skeleton landmark derivatives
  2. Closed-form ICR per joint pair each frame
  3. Lever arm vectors from ICR to effort and load points
  4. Leverage ratio lambda(t) time series per joint
  5. ICR trajectory stability index
  6. Palm change singularity detection

Input:  positions (T, J, 3), joint pair definitions
Output: ICRFrame per timestep
"""

import numpy as np
from dataclasses import dataclass, field
from typing import List, Tuple, Optional


# ---------------------------------------------------------------------------
# Joint pair definitions for ICR computation
# Each tuple: (proximal_idx, distal_idx, name)
# Using MediaPipe Holistic indices
# ---------------------------------------------------------------------------

JOINT_PAIRS = [
    (11, 13, "left_elbow"),
    (13, 15, "left_wrist"),
    (12, 14, "right_elbow"),
    (14, 16, "right_wrist"),
    (11, 12, "shoulder_girdle"),
    (11, 23, "left_hip"),
    (12, 24, "right_hip"),
    (23, 25, "left_knee"),
    (24, 26, "right_knee"),
    (25, 27, "left_ankle"),
    (26, 28, "right_ankle"),
    (23, 24, "pelvis"),
]


@dataclass
class JointICR:
    """ICR result for one joint pair at one timestep."""
    name: str
    icr_pos: np.ndarray        # ICR world position (3,)
    icr_valid: bool            # False if omega near zero (translation phase)
    lambda_val: float          # leverage ratio
    effort_arm: float          # |r_effort| meters
    load_arm: float            # |r_load| meters
    omega_mag: float           # relative angular velocity magnitude


@dataclass
class ICRFrame:
    """All joint ICR results for one timestep."""
    frame_idx: int
    joints: List[JointICR]
    palm_change: bool          # singularity detected this frame
    mean_lambda: float         # mean leverage ratio all valid joints
    stability_index: float     # ICR position variance (lower = better)


# ---------------------------------------------------------------------------
# Numerical derivatives
# ---------------------------------------------------------------------------

def central_diff(x: np.ndarray, dt: float) -> np.ndarray:
    """2nd-order central difference derivative. x: (N,...), returns (N,...) """
    d = np.zeros_like(x)
    d[1:-1] = (x[2:] - x[:-2]) / (2.0 * dt)
    d[0]    = (x[1] - x[0]) / dt
    d[-1]   = (x[-1] - x[-2]) / dt
    return d


def segment_angular_velocity(
    pos_prox: np.ndarray,
    pos_dist: np.ndarray,
    dt: float,
) -> np.ndarray:
    """
    Estimate angular velocity of segment from proximal and distal endpoints.
    pos_prox, pos_dist: (N, 3)
    Returns: (N, 3) angular velocity vector
    Method: omega = (r x r_dot) / |r|^2  where r = dist - prox
    """
    r = pos_dist - pos_prox                         # (N, 3) segment vector
    r_dot = central_diff(r, dt)                     # (N, 3) segment velocity
    r_norm_sq = np.sum(r**2, axis=1, keepdims=True) # (N, 1)
    r_norm_sq = np.where(r_norm_sq < 1e-12, 1e-12, r_norm_sq)
    omega = np.cross(r, r_dot) / r_norm_sq          # (N, 3)
    return omega


# ---------------------------------------------------------------------------
# Closed-form ICR
# ---------------------------------------------------------------------------

def compute_icr(
    pos_joint: np.ndarray,
    vel_joint: np.ndarray,
    omega: np.ndarray,
    omega_thresh: float = 0.01,
) -> Tuple[np.ndarray, np.ndarray]:
    """
    Closed-form ICR from joint position, velocity, and angular velocity.
    pos_joint:  (N, 3)
    vel_joint:  (N, 3)
    omega:      (N, 3) angular velocity of segment
    Returns: icr_pos (N, 3), valid_mask (N,) bool
    Formula: x_ICR = x_joint + (omega x v) / |omega|^2
    """
    omega_mag_sq = np.sum(omega**2, axis=1)         # (N,)
    valid = omega_mag_sq > omega_thresh**2
    omega_cross_v = np.cross(omega, vel_joint)      # (N, 3)
    denom = np.where(omega_mag_sq < 1e-12, 1e-12, omega_mag_sq)
    icr_offset = omega_cross_v / denom[:, None]     # (N, 3)
    icr_pos = pos_joint + icr_offset
    return icr_pos, valid


# ---------------------------------------------------------------------------
# Leverage ratio
# ---------------------------------------------------------------------------

def leverage_ratio(
    icr_pos: np.ndarray,
    effort_pos: np.ndarray,
    load_pos: np.ndarray,
    effort_force: Optional[np.ndarray] = None,
    load_force: Optional[np.ndarray] = None,
) -> Tuple[np.ndarray, np.ndarray, np.ndarray]:
    """
    Compute leverage ratio lambda = |r_e x F_e| / |r_l x F_l|.
    If forces not provided, uses geometric ratio |r_e| / |r_l|.
    All inputs: (N, 3)
    Returns: lambda (N,), effort_arm (N,), load_arm (N,)
    """
    r_e = effort_pos - icr_pos                      # (N, 3)
    r_l = load_pos   - icr_pos                      # (N, 3)
    effort_arm = np.linalg.norm(r_e, axis=1)        # (N,)
    load_arm   = np.linalg.norm(r_l, axis=1)        # (N,)
    if effort_force is not None and load_force is not None:
        M_e = np.linalg.norm(np.cross(r_e, effort_force), axis=1)
        M_l = np.linalg.norm(np.cross(r_l, load_force),   axis=1)
        denom = np.where(M_l < 1e-9, 1e-9, M_l)
        lam = M_e / denom
    else:
        denom = np.where(load_arm < 1e-9, 1e-9, load_arm)
        lam = effort_arm / denom
    return lam, effort_arm, load_arm


# ---------------------------------------------------------------------------
# Palm change singularity detection
# ---------------------------------------------------------------------------

def detect_palm_change(
    pelvis_xy: np.ndarray,
    dt: float,
    window: int = 10,
    omega_thresh: float = 0.5,
) -> np.ndarray:
    """
    Detect palm change events: reversal of orbital direction.
    pelvis_xy: (N, 2)
    Returns: (N,) bool array -- True at palm change frames
    """
    N = len(pelvis_xy)
    events = np.zeros(N, dtype=bool)
    vel = central_diff(pelvis_xy, dt)               # (N, 2)
    # Approximate orbital angular velocity: sign of cross product with radius
    # Use center of mass of trajectory as approximate circle center
    center = np.mean(pelvis_xy, axis=0)
    rel = pelvis_xy - center                        # (N, 2)
    # 2D cross: rel x vel = rel_x*vel_y - rel_y*vel_x
    cross_2d = rel[:, 0]*vel[:, 1] - rel[:, 1]*vel[:, 0]
    sign = np.sign(cross_2d)
    # Sign change = direction reversal = palm change
    for i in range(window, N - window):
        before = sign[i-window:i]
        after  = sign[i:i+window]
        if (np.mean(before) > 0.3 and np.mean(after) < -0.3) or \
           (np.mean(before) < -0.3 and np.mean(after) > 0.3):
            events[i] = True
    return events


# ---------------------------------------------------------------------------
# Main pipeline
# ---------------------------------------------------------------------------

def compute_icr_stream(
    positions: np.ndarray,
    dt: float = 1.0 / 120.0,
    joint_pairs: List[Tuple] = None,
) -> List[ICRFrame]:
    """
    Compute ICR and leverage ratio for all joint pairs over full trajectory.
    positions: (T, J, 3)
    Returns: list of ICRFrame per timestep
    """
    if joint_pairs is None:
        joint_pairs = JOINT_PAIRS
    T = positions.shape[0]
    # Palm change detection from pelvis midpoint
    pelvis_xy = ((positions[:, 23, :2] + positions[:, 24, :2]) / 2.0
                 if positions.shape[1] > 24
                 else positions[:, 0, :2])
    palm_events = detect_palm_change(pelvis_xy, dt)
    # Pre-compute velocities for all joints
    velocities = central_diff(positions, dt)        # (T, J, 3)
    # Per-pair computation
    pair_results = []
    for p_idx, d_idx, name in joint_pairs:
        if p_idx >= positions.shape[1] or d_idx >= positions.shape[1]:
            continue
        pos_p = positions[:, p_idx, :]              # (T, 3)
        pos_d = positions[:, d_idx, :]              # (T, 3)
        vel_p = velocities[:, p_idx, :]             # (T, 3)
        omega = segment_angular_velocity(pos_p, pos_d, dt)  # (T, 3)
        icr_pos, valid = compute_icr(pos_p, vel_p, omega)   # (T,3), (T,)
        # Leverage: effort=distal end, load=proximal CoM
        lam, e_arm, l_arm = leverage_ratio(icr_pos, pos_d, pos_p)
        omega_mag = np.linalg.norm(omega, axis=1)
        pair_results.append((name, icr_pos, valid, lam, e_arm, l_arm, omega_mag))
    # Build per-frame output
    frames = []
    for t in range(T):
        joints = []
        lambdas = []
        for name, icr_pos, valid, lam, e_arm, l_arm, omega_mag in pair_results:
            j = JointICR(
                name=name,
                icr_pos=icr_pos[t],
                icr_valid=bool(valid[t]),
                lambda_val=float(np.clip(lam[t], 0, 20)),
                effort_arm=float(e_arm[t]),
                load_arm=float(l_arm[t]),
                omega_mag=float(omega_mag[t]),
            )
            joints.append(j)
            if j.icr_valid:
                lambdas.append(j.lambda_val)
        mean_lam = float(np.mean(lambdas)) if lambdas else 0.0
        # Stability: std of ICR positions this frame across valid joints
        valid_icrs = [j.icr_pos for j in joints if j.icr_valid]
        stab = float(np.std(np.stack(valid_icrs))) if len(valid_icrs) > 1 else 0.0
        frames.append(ICRFrame(
            frame_idx=t,
            joints=joints,
            palm_change=bool(palm_events[t]),
            mean_lambda=mean_lam,
            stability_index=stab,
        ))
    return frames


def icr_frame_to_json(f: ICRFrame) -> dict:
    """Serialize ICRFrame to JSON-ready dict for WebSocket transport."""
    return {
        "frame":       f.frame_idx,
        "palm_change": f.palm_change,
        "mean_lambda": round(f.mean_lambda, 4),
        "stability":   round(f.stability_index, 4),
        "joints": [
            {
                "name":       j.name,
                "valid":      j.icr_valid,
                "icr":        [round(float(v), 4) for v in j.icr_pos],
                "lambda":     round(j.lambda_val, 4),
                "effort_arm": round(j.effort_arm, 4),
                "load_arm":   round(j.load_arm, 4),
            }
            for j in f.joints
        ],
    }

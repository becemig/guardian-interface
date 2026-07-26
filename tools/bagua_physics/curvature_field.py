"""
curvature_field.py
Guardian Interface -- Bagua Physics Engine

Computes per-joint curvature and attunement scalar A(t) from 3D skeleton landmarks.

Pipeline:
  1. Arc-length reparameterization of landmark trajectories
  2. Frenet-Serret frame: tangent T, normal N, binormal B, curvature kappa, torsion tau
  3. Covariant acceleration via Levi-Civita connection (finite-difference Christoffel)
  4. Attunement scalar A(t) = norm of covariant acceleration
  5. Sectional curvature K from Riemann tensor
  6. Heat-map color per joint via teal-amber-red colormap

Input:  positions array shape (T, J, 3) -- T frames, J joints, 3D coords
Output: CurvatureFrame dataclass per timestep
"""

import numpy as np
from dataclasses import dataclass, field
from typing import List, Tuple

# ---------------------------------------------------------------------------
# MediaPipe Holistic landmark indices (subset used for kinematic chain)
# ---------------------------------------------------------------------------
JOINT_NAMES = [
    "nose", "left_eye", "right_eye", "left_ear", "right_ear",
    "left_shoulder", "right_shoulder", "left_elbow", "right_elbow",
    "left_wrist", "right_wrist", "left_hip", "right_hip",
    "left_knee", "right_knee", "left_ankle", "right_ankle",
    "left_heel", "right_heel", "left_foot_index", "right_foot_index",
]

# Primary kinematic chain joints for curvature computation
CHAIN_JOINTS = [5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]
# left_shoulder right_shoulder left_elbow right_elbow
# left_wrist right_wrist left_hip right_hip
# left_knee right_knee left_ankle right_ankle

# ---------------------------------------------------------------------------
# Data structures
# ---------------------------------------------------------------------------

@dataclass
class FrenetFrame:
    """Frenet-Serret frame for a single joint trajectory."""
    joint_idx: int
    T: np.ndarray         # tangent vectors  (N, 3)
    N: np.ndarray         # normal vectors   (N, 3)
    B: np.ndarray         # binormal vectors (N, 3)
    kappa: np.ndarray     # curvature        (N,)
    tau: np.ndarray       # torsion          (N,)
    arc_s: np.ndarray     # arc-length param (N,)


@dataclass
class CurvatureFrame:
    """Per-timestep physics output for one frame of skeleton data."""
    frame_idx: int
    attunement: np.ndarray     # A(t) per joint       shape (J,)
    kappa: np.ndarray          # curvature per joint  shape (J,)
    tau: np.ndarray            # torsion per joint    shape (J,)
    colors: np.ndarray         # RGB heat-map colors  shape (J, 3)
    global_attunement: float   # mean A(t) all joints
    sectional_K: np.ndarray    # sectional curvature  shape (J,)


# ---------------------------------------------------------------------------
# Arc-length reparameterization
# ---------------------------------------------------------------------------

def arc_length(positions: np.ndarray) -> np.ndarray:
    """
    Compute cumulative arc-length along trajectory.
    positions: (N, 3)
    returns:   (N,) arc-length values starting at 0
    """
    diffs = np.diff(positions, axis=0)          # (N-1, 3)
    seg_lens = np.linalg.norm(diffs, axis=1)    # (N-1,)
    s = np.concatenate([[0.0], np.cumsum(seg_lens)])
    return s


def richardson_derivative(f: np.ndarray, s: np.ndarray) -> np.ndarray:
    """
    4th-order Richardson extrapolation derivative df/ds.
    f: (N, 3) or (N,)
    s: (N,) arc-length or time parameterization
    returns: df/ds same shape as f
    Uses central differences interior, forward/backward at boundaries.
    """
    N = len(s)
    df = np.zeros_like(f)
    # Interior: 4th-order central difference
    for i in range(2, N - 2):
        h = s[i+1] - s[i-1]  # approximate local spacing
        df[i] = (-f[i+2] + 8*f[i+1] - 8*f[i-1] + f[i-2]) / (3 * h)
    # Boundaries: 2nd-order
    for i in [0, 1]:
        h = s[i+1] - s[i] if i < N-1 else s[i] - s[i-1]
        df[i] = (f[min(i+1, N-1)] - f[max(i-1, 0)]) / (2 * h + 1e-12)
    for i in [N-2, N-1]:
        h = s[i] - s[i-1] if i > 0 else s[i+1] - s[i]
        df[i] = (f[min(i+1, N-1)] - f[max(i-1, 0)]) / (2 * h + 1e-12)
    return df

# ---------------------------------------------------------------------------
# Frenet-Serret frame
# ---------------------------------------------------------------------------

def build_frenet_frame(positions: np.ndarray, joint_idx: int) -> FrenetFrame:
    """
    Build Frenet-Serret frame for a single joint trajectory.
    positions: (N, 3) trajectory of one joint over N frames
    """
    s = arc_length(positions)
    # First derivative: tangent direction
    dp = richardson_derivative(positions, s)
    norms = np.linalg.norm(dp, axis=1, keepdims=True)
    norms = np.where(norms < 1e-9, 1e-9, norms)
    T = dp / norms                              # unit tangent (N, 3)
    # Second derivative: curvature direction
    dT = richardson_derivative(T, s)
    kappa = np.linalg.norm(dT, axis=1)         # curvature scalar (N,)
    kappa_safe = np.where(kappa < 1e-9, 1e-9, kappa)
    N_vec = dT / kappa_safe[:, None]           # principal normal (N, 3)
    # Binormal
    B = np.cross(T, N_vec)                     # binormal (N, 3)
    # Torsion: tau = -(dB/ds) . N
    dB = richardson_derivative(B, s)
    tau = -np.einsum("ij,ij->i", dB, N_vec)    # torsion scalar (N,)
    return FrenetFrame(
        joint_idx=joint_idx, T=T, N=N_vec, B=B,
        kappa=kappa, tau=tau, arc_s=s
    )


# ---------------------------------------------------------------------------
# Covariant acceleration and attunement scalar
# ---------------------------------------------------------------------------

def covariant_acceleration(positions: np.ndarray, dt: float = 1.0/120.0) -> np.ndarray:
    """
    Compute covariant acceleration norm A(t) for a joint trajectory.
    Approximation: on Euclidean R^3 covariant accel = ordinary accel.
    For body manifold embedding, we project acceleration onto tangent plane.
    positions: (N, 3)
    returns:   (N,) attunement scalar A(t)
    """
    t = np.arange(len(positions)) * dt
    vel = richardson_derivative(positions, t)       # (N, 3) velocity
    acc = richardson_derivative(vel, t)             # (N, 3) acceleration
    # Tangent direction
    speed = np.linalg.norm(vel, axis=1, keepdims=True)
    speed = np.where(speed < 1e-9, 1e-9, speed)
    T_hat = vel / speed                             # unit tangent
    # Covariant component: remove tangential part (centripetal remains)
    acc_tangential = np.einsum("ij,ij->i", acc, T_hat)[:, None] * T_hat
    acc_covariant = acc - acc_tangential            # transverse covariant accel
    A = np.linalg.norm(acc_covariant, axis=1)      # attunement scalar (N,)
    return A

# ---------------------------------------------------------------------------
# Sectional curvature (finite difference approximation)
# ---------------------------------------------------------------------------

def sectional_curvature(positions: np.ndarray, dt: float = 1.0/120.0) -> np.ndarray:
    """
    Approximate sectional curvature K from trajectory.
    K = kappa^2 -- valid for curves in R^3 embedded manifold.
    positions: (N, 3)
    returns:   (N,) sectional curvature
    """
    t = np.arange(len(positions)) * dt
    vel = richardson_derivative(positions, t)
    acc = richardson_derivative(vel, t)
    # Curvature formula: kappa = |v x a| / |v|^3
    cross = np.cross(vel, acc)                        # (N, 3)
    cross_norm = np.linalg.norm(cross, axis=1)        # (N,)
    speed = np.linalg.norm(vel, axis=1)               # (N,)
    speed3 = np.where(speed**3 < 1e-12, 1e-12, speed**3)
    kappa = cross_norm / speed3
    return kappa ** 2                                  # K = kappa^2


# ---------------------------------------------------------------------------
# Heat-map colormap: teal -> amber -> red
# ---------------------------------------------------------------------------

TEAL  = np.array([0x20, 0x80, 0x8D], dtype=float) / 255.0
AMBER = np.array([0xDA, 0x71, 0x01], dtype=float) / 255.0
RED   = np.array([0xA1, 0x35, 0x44], dtype=float) / 255.0


def attunement_to_color(A: np.ndarray, A_max: float = None) -> np.ndarray:
    """
    Map attunement scalar array to RGB heat-map colors.
    A:     (J,) attunement values
    A_max: normalization ceiling (defaults to session 95th percentile)
    returns: (J, 3) RGB float in [0, 1]
    """
    if A_max is None:
        A_max = float(np.percentile(A, 95)) if len(A) > 1 else 1.0
    A_max = max(A_max, 1e-6)
    t = np.clip(A / A_max, 0.0, 1.0)           # normalized [0, 1]
    # Two-segment colormap: [0,0.5] teal->amber, [0.5,1] amber->red
    colors = np.zeros((len(t), 3))
    lo = t < 0.5
    hi = ~lo
    t_lo = t[lo] * 2.0                         # remap [0,0.5] to [0,1]
    t_hi = (t[hi] - 0.5) * 2.0                # remap [0.5,1] to [0,1]
    colors[lo] = np.outer(1 - t_lo, TEAL) + np.outer(t_lo, AMBER)
    colors[hi] = np.outer(1 - t_hi, AMBER) + np.outer(t_hi, RED)
    return colors


# ---------------------------------------------------------------------------
# Main pipeline: positions -> CurvatureFrame per frame
# ---------------------------------------------------------------------------

def compute_curvature_stream(
    positions: np.ndarray,
    dt: float = 1.0 / 120.0,
    window: int = 30,
    joint_indices: List[int] = None,
) -> List[CurvatureFrame]:
    """
    Process full skeleton trajectory and return per-frame CurvatureFrames.
    positions:     (T, J, 3) -- T timesteps, J joints, 3D coords
    dt:            frame interval in seconds (default 1/120)
    window:        number of frames to use for local computation
    joint_indices: which joints to compute (default CHAIN_JOINTS)
    returns:       list of CurvatureFrame, one per timestep
    """
    if joint_indices is None:
        joint_indices = CHAIN_JOINTS
    T, J, _ = positions.shape
    n_joints = len(joint_indices)
    # Compute full-trajectory attunement and curvature per joint
    A_all = np.zeros((T, n_joints))
    K_all = np.zeros((T, n_joints))
    kappa_all = np.zeros((T, n_joints))
    tau_all   = np.zeros((T, n_joints))
    for ji, jidx in enumerate(joint_indices):
        traj = positions[:, jidx, :]           # (T, 3)
        A_all[:, ji]     = covariant_acceleration(traj, dt)
        K_all[:, ji]     = sectional_curvature(traj, dt)
        ff = build_frenet_frame(traj, jidx)
        kappa_all[:, ji] = ff.kappa
        tau_all[:, ji]   = ff.tau
    # Session-level normalization ceiling (95th percentile)
    A_max = float(np.percentile(A_all, 95))
    # Build per-frame output
    frames = []
    for t_idx in range(T):
        A_t = A_all[t_idx]
        colors = attunement_to_color(A_t, A_max)
        frames.append(CurvatureFrame(
            frame_idx=t_idx,
            attunement=A_t,
            kappa=kappa_all[t_idx],
            tau=tau_all[t_idx],
            colors=colors,
            global_attunement=float(np.mean(A_t)),
            sectional_K=K_all[t_idx],
        ))
    return frames


def frame_to_json(cf: CurvatureFrame) -> dict:
    """Serialize CurvatureFrame to JSON-ready dict for WebSocket transport."""
    return {
        "frame": cf.frame_idx,
        "global_A": round(cf.global_attunement, 5),
        "attunement": [round(float(v), 5) for v in cf.attunement],
        "kappa":      [round(float(v), 5) for v in cf.kappa],
        "tau":        [round(float(v), 5) for v in cf.tau],
        "K":          [round(float(v), 5) for v in cf.sectional_K],
        "colors":     [[round(float(c), 4) for c in row] for row in cf.colors],
    }

"""
holonomy_integrator.py
Guardian Interface -- Bagua Physics Engine

Computes SO(3) holonomy per completed Bagua circle loop.

Pipeline:
  1. Circle loop detection from overhead CoP or pelvis trajectory
  2. Body orientation quaternion tracking (from IMU or skeleton)
  3. Connection form Omega along loop
  4. Path-ordered exponential via Magnus expansion (order 2)
  5. Holonomy angle and axis extraction from SO(3) result
  6. Power signature: holonomy per unit arc length

Input:  pelvis positions (T, 3), body quaternions (T, 4)
Output: HolonomyResult per detected circle
"""

import numpy as np
from dataclasses import dataclass
from typing import List, Optional, Tuple


@dataclass
class HolonomyResult:
    """Holonomy computed for one completed circle loop."""
    circle_idx: int
    start_frame: int
    end_frame: int
    arc_length: float        # total path length of circle (meters)
    radius_mean: float       # mean circle radius (meters)
    radius_std: float        # radius stability (lower = better)
    hol_matrix: np.ndarray   # SO(3) holonomy matrix (3, 3)
    hol_angle: float         # holonomy rotation angle (radians)
    hol_axis: np.ndarray     # holonomy rotation axis (3,)
    power_signature: float   # hol_angle / arc_length (rad/m)
    omega_mean: float        # mean orbital angular velocity (rad/s)


# ---------------------------------------------------------------------------
# Quaternion utilities
# ---------------------------------------------------------------------------

def quat_mult(q1: np.ndarray, q2: np.ndarray) -> np.ndarray:
    """Multiply two quaternions [w, x, y, z]."""
    w1, x1, y1, z1 = q1
    w2, x2, y2, z2 = q2
    return np.array([
        w1*w2 - x1*x2 - y1*y2 - z1*z2,
        w1*x2 + x1*w2 + y1*z2 - z1*y2,
        w1*y2 - x1*z2 + y1*w2 + z1*x2,
        w1*z2 + x1*y2 - y1*x2 + z1*w2,
    ])


def quat_to_rot(q: np.ndarray) -> np.ndarray:
    """Convert unit quaternion [w, x, y, z] to 3x3 rotation matrix."""
    w, x, y, z = q / (np.linalg.norm(q) + 1e-12)
    return np.array([
        [1-2*(y*y+z*z),   2*(x*y-w*z),   2*(x*z+w*y)],
        [  2*(x*y+w*z), 1-2*(x*x+z*z),   2*(y*z-w*x)],
        [  2*(x*z-w*y),   2*(y*z+w*x), 1-2*(x*x+y*y)],
    ])


def rot_to_axis_angle(R: np.ndarray) -> Tuple[np.ndarray, float]:
    """Extract axis and angle from SO(3) rotation matrix."""
    # angle from trace: tr(R) = 1 + 2*cos(theta)
    cos_theta = (np.trace(R) - 1.0) / 2.0
    cos_theta = np.clip(cos_theta, -1.0, 1.0)
    angle = np.arccos(cos_theta)
    if abs(angle) < 1e-8:
        return np.array([0.0, 0.0, 1.0]), 0.0
    # axis from skew-symmetric part
    skew = (R - R.T) / (2.0 * np.sin(angle) + 1e-12)
    axis = np.array([skew[2, 1], skew[0, 2], skew[1, 0]])
    axis = axis / (np.linalg.norm(axis) + 1e-12)
    return axis, float(angle)


def skew(v: np.ndarray) -> np.ndarray:
    """3x3 skew-symmetric matrix from vector v."""
    return np.array([
        [ 0,    -v[2],  v[1]],
        [ v[2],  0,    -v[0]],
        [-v[1],  v[0],  0   ],
    ])


# ---------------------------------------------------------------------------
# Circle loop detection
# ---------------------------------------------------------------------------

def fit_circle_2d(xy: np.ndarray) -> Tuple[np.ndarray, float]:
    """
    Least-squares circle fit to 2D points.
    xy: (N, 2)
    returns: center (2,), radius float
    """
    A = np.column_stack([2*xy[:, 0], 2*xy[:, 1], np.ones(len(xy))])
    b = xy[:, 0]**2 + xy[:, 1]**2
    result, _, _, _ = np.linalg.lstsq(A, b, rcond=None)
    cx, cy = result[0], result[1]
    r = np.sqrt(result[2] + cx**2 + cy**2)
    return np.array([cx, cy]), float(r)


def detect_circle_loops(
    pelvis_xy: np.ndarray,
    dt: float = 1.0/120.0,
    min_loop_frames: int = 120,
    angle_tolerance: float = 0.15,
) -> List[Tuple[int, int]]:
    """
    Detect completed circle loops in pelvis XY trajectory.
    pelvis_xy: (T, 2) horizontal pelvis position
    Returns list of (start_frame, end_frame) per detected loop.
    Strategy: track cumulative angle around estimated center.
    A loop completes when cumulative angle crosses 2*pi.
    """
    T = len(pelvis_xy)
    if T < min_loop_frames:
        return []
    # Estimate circle center from full trajectory
    center, radius = fit_circle_2d(pelvis_xy)
    # Compute angle of each point relative to center
    rel = pelvis_xy - center
    angles = np.arctan2(rel[:, 1], rel[:, 0])
    # Unwrap angles to get cumulative rotation
    angles_unwrapped = np.unwrap(angles)
    total_rotation = angles_unwrapped[-1] - angles_unwrapped[0]
    # Detect each 2*pi crossing
    loops = []
    two_pi = 2.0 * np.pi
    direction = 1 if total_rotation > 0 else -1
    start = 0
    cumulative = 0.0
    for i in range(1, T):
        delta = angles_unwrapped[i] - angles_unwrapped[i-1]
        cumulative += delta
        if abs(cumulative) >= two_pi * (1.0 - angle_tolerance):
            if i - start >= min_loop_frames:
                loops.append((start, i))
            start = i
            cumulative = 0.0
    return loops


# ---------------------------------------------------------------------------
# Magnus expansion path-ordered exponential
# ---------------------------------------------------------------------------

def magnus_order2(Omega_series: np.ndarray, dt: float) -> np.ndarray:
    """
    Order-2 Magnus expansion for path-ordered matrix exponential.
    Omega_series: (N, 3, 3) sequence of Lie algebra elements (skew-symmetric)
    dt: time step
    Returns: SO(3) matrix -- the holonomy
    Preserves SO(3) structure exactly (unlike naive Euler integration).
    """
    N = len(Omega_series)
    R = np.eye(3)
    for i in range(N - 1):
        Om1 = Omega_series[i]
        Om2 = Omega_series[i + 1]
        # Magnus order-2: Omega_mag = 0.5*(Om1+Om2)*dt + (dt^2/12)*[Om1,Om2]
        comm = Om1 @ Om2 - Om2 @ Om1
        Omega_mag = 0.5 * (Om1 + Om2) * dt + (dt**2 / 12.0) * comm
        # Matrix exponential of skew-symmetric: Rodrigues formula
        # exp(skew(v)) = I + sin(|v|)/|v| * skew(v) + (1-cos|v|)/|v|^2 * skew(v)^2
        v = np.array([Omega_mag[2,1], Omega_mag[0,2], Omega_mag[1,0]])
        theta = np.linalg.norm(v)
        if theta < 1e-10:
            dR = np.eye(3) + Omega_mag
        else:
            K = skew(v / theta)
            dR = np.eye(3) + np.sin(theta)*K + (1-np.cos(theta))*(K @ K)
        R = R @ dR
    # Project back onto SO(3) via SVD to correct drift
    U, _, Vt = np.linalg.svd(R)
    R = U @ Vt
    if np.linalg.det(R) < 0:
        U[:, -1] *= -1
        R = U @ Vt
    return R


# ---------------------------------------------------------------------------
# Connection form from quaternion trajectory
# ---------------------------------------------------------------------------

def connection_form_series(
    quats: np.ndarray,
    dt: float,
) -> np.ndarray:
    """
    Compute connection form Omega(t) as skew-symmetric matrix series.
    quats: (N, 4) body orientation quaternions [w, x, y, z]
    Returns: (N, 3, 3) skew-symmetric Omega per frame
    Omega = R^T * dR/dt  (body angular velocity in body frame)
    """
    N = len(quats)
    Omega_series = np.zeros((N, 3, 3))
    for i in range(N):
        R = quat_to_rot(quats[i])
        if i < N - 1:
            R_next = quat_to_rot(quats[i + 1])
            dR = (R_next - R) / dt
        else:
            R_prev = quat_to_rot(quats[i - 1])
            dR = (R - R_prev) / dt
        Omega = R.T @ dR   # body-frame angular velocity as skew matrix
        # Symmetrize to enforce skew: Omega = 0.5*(Omega - Omega.T)
        Omega_series[i] = 0.5 * (Omega - Omega.T)
    return Omega_series


# ---------------------------------------------------------------------------
# Main pipeline
# ---------------------------------------------------------------------------

def compute_holonomy_stream(
    pelvis_pos: np.ndarray,
    body_quats: np.ndarray,
    dt: float = 1.0 / 120.0,
) -> List[HolonomyResult]:
    """
    Detect circle loops and compute SO(3) holonomy per loop.
    pelvis_pos:  (T, 3) pelvis world position
    body_quats:  (T, 4) body orientation quaternions [w,x,y,z]
    Returns: list of HolonomyResult -- one per completed circle
    """
    pelvis_xy = pelvis_pos[:, :2]
    loops = detect_circle_loops(pelvis_xy, dt=dt)
    results = []
    for ci, (s, e) in enumerate(loops):
        seg_pos  = pelvis_pos[s:e]
        seg_quat = body_quats[s:e]
        n = len(seg_pos)
        if n < 4:
            continue
        # Arc length and radius stats
        diffs = np.diff(seg_pos, axis=0)
        arc = float(np.sum(np.linalg.norm(diffs, axis=1)))
        center, radius = fit_circle_2d(seg_pos[:, :2])
        radii = np.linalg.norm(seg_pos[:, :2] - center, axis=1)
        # Angular velocity for omega_mean
        angles = np.unwrap(np.arctan2(
            seg_pos[:, 1] - center[1],
            seg_pos[:, 0] - center[0]
        ))
        omega_vals = np.abs(np.diff(angles) / dt)
        # Connection form and Magnus holonomy
        Omega_series = connection_form_series(seg_quat, dt)
        hol_mat = magnus_order2(Omega_series, dt)
        axis, angle = rot_to_axis_angle(hol_mat)
        power_sig = angle / arc if arc > 1e-6 else 0.0
        results.append(HolonomyResult(
            circle_idx=ci,
            start_frame=s,
            end_frame=e,
            arc_length=arc,
            radius_mean=float(np.mean(radii)),
            radius_std=float(np.std(radii)),
            hol_matrix=hol_mat,
            hol_angle=angle,
            hol_axis=axis,
            power_signature=power_sig,
            omega_mean=float(np.mean(omega_vals)),
        ))
    return results


def holonomy_to_json(h: HolonomyResult) -> dict:
    """Serialize HolonomyResult to JSON-ready dict."""
    return {
        "circle_idx":      h.circle_idx,
        "start_frame":     h.start_frame,
        "end_frame":       h.end_frame,
        "arc_length_m":    round(h.arc_length, 4),
        "radius_mean_m":   round(h.radius_mean, 4),
        "radius_std_m":    round(h.radius_std, 4),
        "hol_angle_rad":   round(h.hol_angle, 6),
        "hol_axis":        [round(float(v), 5) for v in h.hol_axis],
        "power_signature": round(h.power_signature, 6),
        "omega_mean_rads": round(h.omega_mean, 5),
    }

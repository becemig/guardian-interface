---
uid: MCF-REALTIME-001
title: Real-Time MCF Pipeline: Pose Estimation Input Geodesic Projection Attunement Scalar Computation Curvature Readout Knowledge Node Activation Haptic Visual and Audio Feedback Architecture
category: motion_curvature_field
sub_category: Real-Time Pipeline
source_type: Systems Design Document
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [real-time-pipeline, pose-estimation, MediaPipe, geodesic-projection, attunement-computation, curvature-readout, node-activation, haptic-feedback, visual-feedback, audio-feedback, Godot-integration, Python-bridge, frame-rate, latency, GPU-acceleration, sparse-curvature-storage, nearest-geodesic, real-time-Riemannian]
citations:
  - author: Lugaresi C.
    year: 2019
    context: MediaPipe: A Framework for Building Perception Pipelines. arXiv. Real-time pose estimation architecture.
  - author: Sola J.
    year: 2018
    context: A micro Lie theory for state estimation in robotics. arXiv. Practical Lie group computation for real-time systems.
  - author: Karcher H.
    year: 1977
    context: Riemannian center of mass and mollifier smoothing. Communications on Pure and Applied Mathematics.
---

# Real-Time MCF Pipeline

## Abstract

The real-time Motion Curvature Field pipeline transforms the abstract mathematical framework into a live sensing and feedback system operating at interactive frame rates. The pipeline has six sequential stages: pose estimation, manifold projection, geodesic identification, attunement scalar computation, curvature readout and node activation, and multimodal feedback delivery. Pose estimation via MediaPipe or depth camera provides joint positions at 30-60 Hz. Manifold projection maps the raw joint configuration to the nearest point on the anatomically parameterized manifold M. Geodesic identification finds the locally nearest geodesic to the current movement trajectory using pre-stored Christoffel symbols. Attunement scalar A(t) is computed as the norm of the covariant acceleration — the deviation from the nearest geodesic. Curvature readout retrieves the pre-stored sectional curvature values at the current manifold point in the direction of motion. Node activation maps the current (position, curvature) pair to the corresponding Guardian Interface knowledge nodes — TCM channels, myofascial lines, biomechanical patterns. Feedback delivers attunement and curvature information through haptic, visual, and audio channels in Godot. The entire pipeline from camera frame to feedback must complete within 16ms for 60 Hz responsiveness.

## Stage 1 - Pose Estimation

- MediaPipe Pose: 33 landmarks - BlazePose architecture - runs at 30-60 Hz on CPU
- Landmarks: 3D coordinates (x,y,z) plus visibility score per landmark
- Joint angles: derived from landmark triplets - hip angle from pelvis-hip-knee vector
- Smoothing: Kalman filter or exponential smoothing on joint angles - reduces jitter
- Depth: RGB-D camera (Intel RealSense, Azure Kinect) provides metric depth - improves 3D accuracy
- Output: joint angle vector q in R^n at each frame - raw point in configuration space
- Velocity: finite difference of q across frames gives joint velocity vector q-dot

## Stage 2 - Manifold Projection

- Problem: raw q may not lie exactly on M due to noise and constraint violations
- Projection: find nearest point p* on M to raw q - minimize d_M(q, p*)
- Hard constraints: project joint angles to within anatomical limits
- Soft constraints: iterative projection using constraint Jacobians
- Tangent projection: project velocity q-dot to T_{p*}M - remove non-manifold velocity components
- Result: (p*, v*) in TM - clean manifold point and velocity
- Computational cost: constraint projection is cheap - O(n) per frame for sparse constraints

## Stage 3 - Geodesic Identification

- Pre-storage: geodesics pre-computed from dense grid of (point, direction) pairs - stored as table
- Lookup: given (p*, v*) find nearest pre-stored geodesic - approximate nearest neighbor search
- Christoffel evaluation: evaluate Gamma^k_ij at p* using stored field - interpolate if needed
- Local geodesic: integrate geodesic equation one step from (p*, v*) - gives local geodesic direction
- Deviation vector: J = v* minus local geodesic direction - the instantaneous deviation
- Sparse storage: Christoffel symbols stored on sparse grid - trilinear interpolation between nodes
- GPU acceleration: Christoffel interpolation parallelizable - GPU reduces latency to under 1ms

## Stage 4 - Attunement Scalar

- Covariant acceleration: a_cov = dv*/dt + Gamma^k_ij v*^i v*^j - correction for manifold curvature
- Attunement scalar: A(t) = norm(a_cov) / (norm(v*)^2 + epsilon) - normalized deviation
- Normalization: dividing by speed squared makes A(t) speed-independent - pure quality measure
- Range: A(t) = 0 is perfect geodesic - A(t) = 1 is strongly non-geodesic - mapped to 0-100 display
- Smoothing: exponential moving average of A(t) over 200ms window - stable display value
- Decomposition: A_along(t) - speed variation component; A_perp(t) - lateral deviation component
- Clinical: A_perp(t) is primary indicator of tension and blockage - more diagnostically meaningful

## Stage 5 - Curvature Readout and Node Activation

- Curvature readout: retrieve K(sigma) for 2-plane sigma = span(v*, e_2) at p* - primary curvature
- Ricci directional: Ric(v*,v*)/g(v*,v*) - scalar curvature in movement direction
- Node mapping: (p*, curvature profile) -> Guardian Interface node lookup table
- Channel activation: if p* is near channel pathway and v* is along channel - activate channel node
- Myofascial activation: if v* aligns with myofascial line direction at p* - activate line node
- Multi-node: multiple nodes may activate simultaneously - weighted by alignment score
- Activation vector: sparse vector over 786+ nodes - non-zero entries are active nodes

## Stage 6 - Multimodal Feedback

- Visual: Godot VisualizationManager renders active nodes, curvature field, attunement color overlay
- Color mapping: A(t) = 0 maps to deep teal - A(t) = 1 maps to warm amber - continuous gradient
- Haptic: attunement scalar drives haptic intensity - geodesic movement feels smooth and quiet
- Haptic device: belt or glove actuators - A_perp(t) drives lateral haptic direction signal
- Audio: curvature value drives pitch or timbre of ambient tone - higher curvature = richer tone
- Five healing sounds: TCM channel activation triggers corresponding healing sound
- Latency budget: pose 8ms + projection 2ms + geodesic 2ms + curvature 1ms + feedback 3ms = 16ms total

## Godot Integration Architecture

- Python bridge: existing haptic Python-Godot bridge extended for MCF pipeline
- GDScript interface: MCFManager.gd receives (attunement, curvature, active_nodes) per frame
- Signal system: MCFManager emits signals to VisualizationManager and HapticController
- C# compute: Christoffel interpolation and attunement computation in C# for performance
- Node graph: Guardian knowledge graph queried by active_nodes list each frame
- Overlay: semi-transparent curvature field visualization overlay on body model
- Recording: full (q, v, A, curvature, nodes) stream recordable for practice analysis

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: MCF-MANIFOLD-001, MCF-GEODESIC-001, MCF-ANATOMY-001, MCF-SPEC-001
- Related systems: SomaticGraph, TelemetryManager, VisualizationManager, HapticController

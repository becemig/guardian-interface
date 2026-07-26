---
uid: BAGUA-CURV-001
title: Full Curvature Tensor and Holonomy Computation Pipeline: Frenet-Serret Frame Construction Riemann Curvature Tensor from Finite Differences Covariant Acceleration Attunement Scalar Parallel Transport Integration SO(3) Holonomy Per Circle and the Curvature Heat Map Shader for Guardian Interface BaguaViewer
category: bagua_capture
sub_category: Curvature Pipeline
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [curvature-tensor, holonomy-computation, Frenet-Serret-frame, Riemann-curvature, covariant-acceleration, attunement-scalar, parallel-transport, SO3-holonomy, curvature-heat-map, WebGL-shader, Levi-Civita-connection, geodesic-deviation, sectional-curvature, MCF-pipeline, Bagua-curvature, Wu-Wei-geodesic-flow, curvature-visualization, path-ordered-exponential]
citations:
  - author: do Carmo M.P.
    year: 1992
    context: Riemannian Geometry. Birkhauser. Curvature tensor and geodesic theory.
  - author: Bloch A.M.
    year: 2003
    context: Nonholonomic Mechanics and Control. Springer. Sub-Riemannian curvature.
  - author: Murray R.M.
    year: 1994
    context: A Mathematical Introduction to Robotic Manipulation. CRC Press. SO(3) and SE(3).
---

# Full Curvature Tensor and Holonomy Computation Pipeline

## Abstract

The curvature computation pipeline is the mathematical engine at the center of the MCF framework -- it converts raw 3D skeleton landmark trajectories into the geometric quantities that reveal the deep structure of movement quality. Curvature in the Riemannian sense measures how much a trajectory deviates from a geodesic -- the straightest possible path consistent with the constraints. For human movement on the body configuration manifold, a geodesic corresponds to movement that uses the minimum possible muscular effort for the given task -- the Wu Wei state of classical Daoism rendered as a precise differential geometric quantity. The attunement scalar A(t) is the norm of the covariant acceleration: it is exactly zero for geodesic movement and positive for any deviation, with larger values indicating greater departure from optimal. The pipeline computes A(t) for every joint trajectory simultaneously and renders the result as a curvature heat map on the skeleton -- the primary Layer 1 visualization of the BaguaViewer. Beyond pointwise curvature the pipeline integrates the connection form around each complete circle using path-ordered matrix exponential to compute the holonomy -- the net SO(3) rotation accumulated per circle traversal. This holonomy is the rotational power signature of the circle: it encodes how much net rotational work the practitioner extracted from the geometry of the circle itself, distinct from muscular effort. Expert practitioners show consistent holonomy signatures that become more refined with practice -- the geometry of their movement becomes more precise, accumulating holonomy more efficiently per unit of muscular effort.

## Frenet-Serret Frame Construction

- Input: landmark trajectory gamma(t) in R^3 at 120fps
- Arc length: s(t) = integral_0^t |gamma_dot(tau)| dtau -- reparameterize by arc length
- Unit tangent: T(s) = d(gamma)/ds -- direction of travel along trajectory
- Curvature: kappa(s) = |dT/ds| -- rate of change of tangent direction
- Principal normal: N(s) = (dT/ds) / kappa -- points toward center of curvature
- Binormal: B(s) = T(s) cross N(s) -- perpendicular to osculating plane
- Torsion: tau(s) = -(dB/ds) . N -- rate of rotation of osculating plane
- Frame: {T, N, B} forms orthonormal moving frame -- Frenet-Serret frame
- Frenet equations: dT/ds = kappa*N, dN/ds = -kappa*T + tau*B, dB/ds = -tau*N

## Covariant Acceleration and Attunement Scalar

- Covariant derivative: D/dt -- derivative that accounts for manifold geometry
- Covariant acceleration: D(gamma_dot)/dt -- acceleration projected onto manifold
- Levi-Civita connection: torsion-free metric-compatible connection on body manifold
- Christoffel symbols: Gamma^k_ij -- encode how basis vectors change across manifold
- Practical computation: D(v)/dt = dv/dt + Gamma(q) * v -- add connection correction
- Attunement scalar: A(t) = |D(gamma_dot)/dt|_g -- norm in metric g
- Geodesic condition: A(t) = 0 everywhere -- Wu Wei movement
- Heat map: A(t) per joint mapped to color via shader -- teal=0 amber=mid red=high
- Global A: mean A(t) across all joints -- single movement quality scalar per frame

## Riemann Curvature Tensor

- Riemann tensor: R(X,Y)Z = nabla_X nabla_Y Z - nabla_Y nabla_X Z - nabla_{[X,Y]} Z
- Measures: failure of covariant derivatives to commute -- intrinsic curvature of manifold
- Sectional curvature: K(X,Y) = g(R(X,Y)Y, X) / (|X|^2|Y|^2 - g(X,Y)^2)
- Positive K: manifold curves like sphere -- trajectories converge
- Negative K: manifold curves like saddle -- trajectories diverge -- chaotic
- Finite difference: compute R numerically from Christoffel symbols on sampled manifold
- Body manifold: configuration space of human body -- R encodes biomechanical coupling
- Geodesic deviation: Jacobi equation -- R determines how nearby geodesics spread apart

## Parallel Transport and Connection Form

- Parallel transport: moving a vector along a curve while keeping it parallel
- Levi-Civita: parallel transport preserves inner product -- metric compatible
- Transport equation: dV/dt + Gamma(gamma_dot) V = 0 -- linear ODE along curve
- Connection form: Omega = Gamma_mu dx^mu -- matrix-valued 1-form
- Integration: Omega along curve gives accumulated rotation of parallel transported frame
- Circle loop: integrate Omega around one complete Bagua circle
- Result: SO(3) matrix encoding net frame rotation -- the holonomy
- Sub-Riemannian: connection form restricted to horizontal distribution D

## SO(3) Holonomy Per Circle

- Holonomy group: set of all SO(3) elements achievable by parallel transport loops
- Path-ordered exponential: Hol(gamma) = P exp(integral_gamma Omega)
- Numerical integration: Magnus expansion -- preserves SO(3) structure exactly
- Magnus order 2: Hol approx exp(integral Omega + 0.5*integral[Omega, Omega])
- Per-circle computation: triggered when overhead camera detects loop completion
- Holonomy angle: axis-angle decomposition of Hol -- angle is holonomy magnitude
- Power signature: holonomy angle per unit arc length -- normalized rotational power
- Expert signature: consistent holonomy angle across circles -- refined geometry

## Curvature Heat Map Shader

- Input: A(t) per joint at 60fps -- normalized to [0,1] range per session
- Color map: GLSL shader function -- teal #20808D at 0 -- amber #DA7101 at 0.5 -- red #A13544 at 1
- Interpolation: smooth GLSL mix() between color stops -- no banding
- Bone mesh: capsule geometry per segment -- vertex color from joint A(t) interpolated
- Joint sphere: sphere at each joint colored by local A(t)
- Animation: A(t) updates each frame -- skeleton breathes with movement quality
- Pulse: global A mean drives skeleton-wide brightness pulse -- quality rhythm visible
- History trail: each joint draws 2-second fading trajectory -- path curvature visible

## Pipeline Integration

- Input: 33 x 3D positions at 120fps from multi-view skeleton reconstruction
- Stage 1: Frenet-Serret frame per landmark -- kappa tau per joint
- Stage 2: covariant acceleration per joint -- A(t) per joint
- Stage 3: Riemann tensor from Christoffel symbols -- sectional curvature K
- Stage 4: connection form integration -- per-step holonomy accumulation
- Stage 5: circle completion detection -- per-circle holonomy SO(3) matrix
- Output: A(t) array, K array, Hol(gamma) per circle, kappa per joint
- Transport: FastAPI WebSocket -- JSON physics frame to BaguaViewer at 60fps
- Python module: curvature_field.py + holonomy_integrator.py

## Wu Wei Geodesic Interpretation

- Wu Wei: action without forcing -- Daoist principle of effortless movement
- Geodesic: mathematically effortless -- zero covariant acceleration -- A(t) = 0
- Correspondence: Wu Wei IS geodesic flow on the body configuration manifold
- Practice feedback: curvature heat map shows practitioner exactly where forcing occurs
- Training: reduce A(t) at flagged joints -- learn to release the forcing
- IDA-PBC connection: practice reshapes the effective metric -- changes what geodesic means
- Mastery: A(t) near zero everywhere -- movement has become genuinely effortless
- Holonomy: expert extracts maximum holonomy per unit A(t) -- efficient geometry

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: BAGUA-FOUND-001, BAGUA-ICR-001, BAGUA-CAMERA-001
- Related nodes: MCF-MANIFOLD-001, SUBRIEM-FOUND-001, PORT-WAVE-001
- Related systems: BaguaViewer Layer 1, MCFSystem, SomaticGraph
- Python modules: curvature_field.py holonomy_integrator.py
- Central: curvature heat map is the primary real-time feedback layer of BaguaViewer

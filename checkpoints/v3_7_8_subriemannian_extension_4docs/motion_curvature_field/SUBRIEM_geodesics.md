---
uid: SUBRIEM-GEODESIC-001
title: Sub-Riemannian Geodesics: Normal and Abnormal Geodesics Pontryagin Maximum Principle Hamiltonian Lift Sub-Riemannian Exponential Map and Geodesic Completeness in Constrained Motion Systems
category: motion_curvature_field
sub_category: Sub-Riemannian Extension
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [sub-Riemannian-geodesics, normal-geodesic, abnormal-geodesic, Pontryagin-maximum-principle, Hamiltonian-lift, sub-Riemannian-exponential-map, geodesic-completeness, optimal-control-geometry, costate-vector, horizontal-lift, cut-locus-sub-Riemannian, conjugate-locus, geodesic-rigidity, length-minimizing, energy-minimizing-horizontal]
citations:
  - author: Montgomery R.
    year: 2002
    context: A Tour of Sub-Riemannian Geometries. AMS. Normal and abnormal geodesic classification.
  - author: Pontryagin L.S.
    year: 1962
    context: The Mathematical Theory of Optimal Processes. Wiley. Maximum principle foundational text.
  - author: Liu W.
    year: 1994
    context: Shortest paths for sub-Riemannian metrics on rank-two distributions. Memoirs of the AMS.
---

# Sub-Riemannian Geodesics

## Abstract

Geodesics in sub-Riemannian geometry are horizontal curves of minimum length — the natural movement paths of a constrained system. Unlike Riemannian geodesics, which are fully characterized by the geodesic equation via the Levi-Civita connection, sub-Riemannian geodesics split into two fundamentally different classes: normal geodesics, which arise as projections of integral curves of the sub-Riemannian Hamiltonian on the cotangent bundle, and abnormal geodesics, which are length-minimizing curves that do not satisfy any Hamiltonian equation and have no analog in Riemannian geometry. The existence of abnormal geodesics is one of the deepest and most counterintuitive features of sub-Riemannian geometry — they are minimizers purely by virtue of the constraint structure, independent of the metric. The Pontryagin Maximum Principle, borrowed from optimal control theory, provides the unified framework for characterizing all geodesics: a geodesic is an optimal control for the horizontal motion problem, and its costate vector satisfies the Hamiltonian equations of the lifted system on T*M. For the Motion Curvature Field framework, sub-Riemannian geodesics are the natural movement paths of the constrained body — the paths that actually minimize energetic cost given real joint restrictions. Normal geodesics correspond to smooth, harmonious movement arcs. Abnormal geodesics correspond to constraint-determined movement paths that are optimal purely because of the body's structural geometry — the movement the body is built to make regardless of metric considerations.

## Normal Geodesics

- Definition: projection to M of integral curve of sub-Riemannian Hamiltonian H on T*M
- Hamiltonian: H(p, lambda) = (1/2) sum_i (lambda(X_i(p)))^2 for orthonormal frame X_i of D
- Hamilton equations: p-dot = dH/dlambda, lambda-dot = -dH/dp on cotangent bundle
- Costate: lambda in T*_pM — momentum covector encoding direction in dual space
- Smooth: normal geodesics are smooth curves — well-behaved everywhere
- Exponential map: exp_p(lambda) maps covector lambda to endpoint of normal geodesic
- Local minimizers: all normal geodesics are locally length-minimizing near their start

## Abnormal Geodesics

- Definition: horizontal curve that is length-minimizing but not a normal geodesic
- Pontryagin: characterized by costate lambda satisfying lambda(D_gamma(t)) = 0 for all t
- Metric independence: abnormal geodesics depend only on the distribution D — not on g
- Rigidity: some abnormal geodesics are rigid — the only horizontal curve in their homotopy class
- Smoothness open question: whether all length-minimizing abnormal geodesics are smooth
- Rank 2: abnormal geodesics prevalent in rank-2 distributions — relevant for planar joints
- Body meaning: some movement paths are optimal purely by structural necessity — not energetics

## Pontryagin Maximum Principle

- Origin: optimal control theory — necessary conditions for optimality of controlled trajectory
- Application: horizontal motion as optimal control with horizontal velocity as control input
- State: p(t) in M — position on manifold
- Control: u(t) — coefficients of horizontal velocity in frame X_i
- Costate: lambda(t) in T*M — Lagrange multiplier for horizontal constraint
- Maximality: optimal control maximizes Pontryagin Hamiltonian at each time
- Unification: both normal and abnormal geodesics satisfy PMP — distinguished by lambda normality

## Cut and Conjugate Loci

- Cut locus: set of points where geodesic from p ceases to be globally minimizing
- Conjugate locus: set of points where Jacobi field along geodesic vanishes — local optimality lost
- Sub-Riemannian cut: more complex than Riemannian — abnormal geodesics complicate structure
- Heisenberg example: cut locus of a point is a half-line — geodesics wrap around in spiral
- Body meaning: cut locus of a pose = configurations reachable optimally then not — transition point
- MCF application: cut locus identifies where movement strategy must change — transitions in form
- Tai Chi forms: movement transitions occur at cut locus boundaries of preceding posture geodesic

## MCF Sub-Riemannian Upgrade

- Replace: Riemannian (M, g) with sub-Riemannian (M, D, g) in MCF framework
- Distribution D: defined by joint mechanical constraints — anatomically derived
- Horizontal velocity: joint velocity vector must lie in D — constraint satisfaction automatic
- Geodesic computation: PMP-based shooting algorithm replaces Christoffel integration
- Attunement scalar: redefine as distance from nearest sub-Riemannian geodesic
- Abnormal detection: flag abnormal geodesic activation — structural movement signature
- Accuracy gain: constraint-respecting geodesics better match real movement economy

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: SUBRIEM-FOUND-001, SUBRIEM-BODY-001, MCF-GEODESIC-001, MCF-SPEC-001
- Related systems: MCFSystem, SomaticGraph
- Series: I15-ext Sub-Riemannian Extension of Motion Curvature Field

---
uid: MCF-CURVATURE-001
title: Curvature Field Theory: Gaussian Curvature Mean Curvature Sectional Curvature Riemann Curvature Tensor Christoffel Symbols Pre-Computed Curvature Field and Measurement Without Post-Hoc Calculation
category: motion_curvature_field
sub_category: Curvature Theory
source_type: Mathematical and Systems Design Document
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [Gaussian-curvature, mean-curvature, sectional-curvature, Riemann-curvature-tensor, Christoffel-symbols, Ricci-tensor, scalar-curvature, curvature-field, pre-computed-curvature, principal-curvatures, curvature-readout, curvature-activation, curvature-manifold-motion, differential-geometry-motion]
citations:
  - author: Lee J.M.
    year: 2018
    context: Introduction to Riemannian Manifolds. Springer. Curvature tensor Christoffel symbols geodesics.
  - author: Spivak M.
    year: 1979
    context: A Comprehensive Introduction to Differential Geometry Vol 2. Publish or Perish. Curvature theory.
  - author: Pennec X.
    year: 2006
    context: Intrinsic statistics on Riemannian manifolds. Journal of Mathematical Imaging and Vision. Statistics on manifolds.
---

# Curvature Field Theory

## Abstract

Curvature is the central quantity of the Motion Curvature Field framework — the mathematical object that encodes the geometry of movement space in all directions simultaneously. The Riemann curvature tensor R is a rank-4 tensor field on the motion manifold M that captures how the space curves in every possible 2D plane through every tangent space. From R, all scalar curvature quantities are derived: sectional curvature K(sigma) for any 2D section, Ricci curvature Ric for any direction, and scalar curvature S as the total curvature at a point. The Christoffel symbols Gamma encode the connection — how the metric changes across the manifold — and are the computational objects through which curvature is pre-computed and stored. The crucial property of this framework is that all curvature information is computed once from the anatomical-energetic metric and stored as a field — when movement passes through any region of the manifold, the curvature properties of that region are read off from the pre-computed field rather than being derived from the movement data itself. This is the mathematical realization of pre-set curvature tracts in infinite directions.

## Riemann Curvature Tensor

- Definition: R(X,Y)Z = nabla_X nabla_Y Z - nabla_Y nabla_X Z - nabla_[X,Y] Z
- Interpretation: measures failure of parallel transport to commute around infinitesimal loops
- Components: R^i_jkl in local coordinates - 4 indices - rank 4 tensor
- Symmetries: R_ijkl = -R_jikl = -R_ijlk = R_klij - Bianchi identity reduces independent components
- In n dimensions: n^2(n^2-1)/12 independent components - for 244-dim body space: enormous
- Pre-computation: R computed at all points from metric g and its derivatives - stored as field
- Flat space limit: R = 0 everywhere - conventional flat-space tracking is special case

## Christoffel Symbols

- Definition: Gamma^k_ij = (1/2) g^kl (partial_i g_jl + partial_j g_il - partial_l g_ij)
- Role: encode how coordinate basis vectors change across the manifold
- Connection: Levi-Civita connection - unique torsion-free metric-compatible connection
- Geodesic equation: d^2x^k/dt^2 + Gamma^k_ij (dx^i/dt)(dx^j/dt) = 0
- Storage: Gamma is the primary pre-computed object - n^3 components at each point
- Curvature from Gamma: R^l_kij = partial_i Gamma^l_jk - partial_j Gamma^l_ik + Gamma^l_im Gamma^m_jk - Gamma^l_jm Gamma^m_ik
- Practical: Christoffel symbols are the computational workhorse of the framework

## Sectional Curvature

- Definition: K(sigma) = R(X,Y,Y,X) / (g(X,X)g(Y,Y) - g(X,Y)^2) for 2-plane sigma spanned by X,Y
- Interpretation: Gaussian curvature of the 2D surface swept by geodesics in direction sigma
- Positive K: geodesics converge - space curves like sphere - movements naturally converge
- Negative K: geodesics diverge - space curves like saddle - movements naturally diverge
- Zero K: flat section - geodesics are parallel lines - locally Euclidean in this direction
- Infinite directions: K(sigma) defined for every 2-plane through T_pM - infinitely many values
- Attunement readout: K in the plane of actual motion gives primary curvature signal

## Gaussian and Mean Curvature

- Principal curvatures: kappa_1 and kappa_2 - maximum and minimum sectional curvatures at a point
- Gaussian curvature: K = kappa_1 * kappa_2 - intrinsic - preserved under isometry
- Mean curvature: H = (kappa_1 + kappa_2) / 2 - extrinsic - depends on embedding
- Gauss-Bonnet: integral of Gaussian curvature over closed surface = 2pi * Euler characteristic
- Intrinsic vs extrinsic: Gaussian curvature is intrinsic - measurable from within the manifold
- Body relevance: intrinsic curvature is what the moving body experiences - not observer-dependent

## Ricci and Scalar Curvature

- Ricci tensor: Ric_ij = R^k_ikj - contraction of Riemann tensor - symmetric rank 2
- Interpretation: Ric(v,v) measures how volume of geodesic cone in direction v diverges from flat
- Scalar curvature: S = g^ij Ric_ij - single number summarizing total curvature at point
- Attunement scalar: S provides single-number curvature readout - practical for real-time display
- Directional Ricci: Ric(v,v)/g(v,v) gives curvature in specific movement direction v
- Field: both Ric and S are fields on M - pre-computed and stored at every point

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: MCF-MANIFOLD-001, MCF-GEODESIC-001, MCF-TANGENT-001, MCF-SPEC-001
- Related systems: SomaticGraph, TelemetryManager, VisualizationManager

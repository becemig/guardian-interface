---
uid: SUBRIEM-FOUND-001
title: Foundations of Sub-Riemannian Geometry: Horizontal Distributions Bracket Generation Chow-Rashevskii Theorem Carnot-Caratheodory Distance and the Geometry of Constrained Movement Systems
category: motion_curvature_field
sub_category: Sub-Riemannian Extension
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [sub-Riemannian-geometry, horizontal-distribution, bracket-generation, Chow-Rashevskii, Carnot-Caratheodory-distance, Heisenberg-group, Carnot-group, sub-Riemannian-geodesic, constrained-geometry, contact-geometry, distribution-rank, accessibility, non-holonomic-systems, Hormander-condition, sub-Riemannian-manifold]
citations:
  - author: Montgomery R.
    year: 2002
    context: A Tour of Sub-Riemannian Geometries, Their Geodesics and Applications. AMS Mathematical Surveys.
  - author: Agrachev A.
    year: 2019
    context: A Comprehensive Introduction to Sub-Riemannian Geometry. Cambridge University Press.
  - author: Bellaiche A.
    year: 1996
    context: The tangent space in sub-Riemannian geometry. Sub-Riemannian Geometry, Birkhauser.
---

# Foundations of Sub-Riemannian Geometry

## Abstract

Sub-Riemannian geometry is the geometry of constrained movement — a generalization of Riemannian geometry in which motion is restricted to a subset of available directions at each point in the manifold. Where Riemannian geometry allows movement in any direction from any point, sub-Riemannian geometry specifies a distribution D — a smoothly varying subspace of the tangent space T_pM — and permits only horizontal motion: movement whose velocity vector lies within D at every instant. The metric is defined only on horizontal vectors, leaving transverse directions metrically invisible. The foundational theorem of sub-Riemannian geometry — the Chow-Rashevskii theorem — guarantees that despite this restriction, any two points can still be connected by a horizontal path, provided the distribution satisfies the bracket-generating condition: the iterated Lie brackets of horizontal vector fields eventually span the full tangent space. This seemingly paradoxical result — that a constrained system can still reach everywhere — is the mathematical foundation for understanding how the body's joints, though constrained in their individual degrees of freedom, can collectively produce the full richness of human movement. The Carnot-Caratheodory distance measures path length using only horizontal motion — the true energetic cost of constrained movement. Sub-Riemannian geometry is likely more accurate than Riemannian geometry for the Motion Curvature Field framework, because real joints are not freely movable in all directions: they are constrained systems whose constraints are not mere boundary conditions but structural features of the geometry itself.

## Core Definitions

- Manifold M: smooth n-dimensional manifold — the full configuration space
- Distribution D: smooth assignment p to D_p, a subspace of T_pM of rank k less than n
- Horizontal vector: v in T_pM is horizontal if v is in D_p
- Horizontal curve: gamma(t) is horizontal if gamma-dot(t) is in D_gamma(t) for all t
- Sub-Riemannian metric: inner product g defined only on D — transverse directions undefined
- CC distance: d_cc(p,q) = infimum of lengths of horizontal curves connecting p to q
- Horizontal length: length of horizontal curve = integral of sqrt(g(gamma-dot, gamma-dot)) dt

## Bracket Generation and Chow-Rashevskii

- Lie bracket: [X,Y] measures failure of X and Y flows to commute — new direction generated
- Bracket of horizontal fields: [X,Y] may point outside D — generates new directions
- Hormander condition: iterated brackets of D-sections span T_pM at every p
- Bracket-generating: distribution satisfying Hormander condition — also called completely nonholonomic
- Chow-Rashevskii theorem: if D is bracket-generating then any two points are CC-connected
- Implication: constrained movement can still reach any configuration — via indirect horizontal paths
- Body meaning: despite joint constraints, the body can access any posture via chained movements

## Carnot Groups and Model Spaces

- Heisenberg group: simplest non-trivial sub-Riemannian space — rank 2 distribution in R^3
- Heisenberg geodesics: helical paths — spiral structure emerges naturally from constraints
- Carnot group: stratified nilpotent Lie group — local model for sub-Riemannian manifolds
- Tangent cone: sub-Riemannian manifold looks like Carnot group at infinitesimal scale
- Dilations: Carnot groups have natural scaling — sub-Riemannian balls scale anisotropically
- Hausdorff dimension: sub-Riemannian space has Hausdorff dimension greater than topological
- Chan Si link: Heisenberg geodesic helices correspond to silk-reeling spiral movement paths

## Comparison with Riemannian Geometry

- Riemannian: metric on all of TM — any direction permitted — geodesics globally smooth
- Sub-Riemannian: metric only on D — constrained directions — geodesics may be non-smooth
- Abnormal geodesics: sub-Riemannian has geodesics not satisfying Hamiltonian equation — no analog in Riemannian
- Curvature: sub-Riemannian curvature theory more complex — no Levi-Civita connection on full TM
- Balls: sub-Riemannian metric balls are not smooth — box-shaped at small scales
- MCF upgrade: replace Riemannian M with sub-Riemannian (M, D, g) for joint-constrained body
- Accuracy: sub-Riemannian more accurately models real biomechanical constraint structure

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: MCF-MANIFOLD-001, MCF-GEODESIC-001, SUBRIEM-GEODESIC-001, SUBRIEM-BODY-001
- Related systems: MCFSystem, SomaticGraph
- Series: I15-ext Sub-Riemannian Extension of Motion Curvature Field

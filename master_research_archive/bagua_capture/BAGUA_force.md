---
uid: BAGUA-FORCE-001
title: Force Plate Array and Ground Reaction Force Analysis: Circular Plate Configuration Friction Cone Geometry Twist Moment Decomposition Center of Pressure Trajectory Port-Hamiltonian GRF Formulation and the Ground Vector Field Visualization for Bagua Circle Walking
category: bagua_capture
sub_category: Force Plate Array
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [force-plate-array, GRF-analysis, friction-cone-geometry, twist-moment-decomposition, center-of-pressure, CoP-trajectory, port-Hamiltonian-GRF, ground-vector-field, piezoelectric-force-plate, circular-array-configuration, Bagua-ground-contact, slip-detection, wrench-decomposition, torsion-moment-bagua, foot-strike-bagua, six-axis-force, kinetic-chain-ground, Earth-element-physics]
citations:
  - author: Winter D.A.
    year: 2009
    context: Biomechanics and Motor Control of Human Movement. 4th Edition. Wiley.
  - author: Kistler Group
    year: 2018
    context: Force Measurement Technology for Biomechanics. Kistler Technical Documentation.
  - author: Zatsiorsky V.M.
    year: 2002
    context: Kinetics of Human Motion. Human Kinetics. GRF decomposition and CoP analysis.
---

# Force Plate Array and Ground Reaction Force Analysis

## Abstract

Ground reaction force measurement is the most direct physical window into how a practitioner structures their relationship with the Earth element in TCM biomechanical terms. The GRF is the sum of all forces the ground exerts on the body — it is the counterpart to every force the body exerts on the ground, and it fully determines the net external force driving the whole-body center of mass. For Bagua circle walking, the GRF has a characteristic three-dimensional signature: a vertical component that oscillates between single and double support loading, a centripetal horizontal component directed toward the circle center that maintains the circular orbit, and a tangential horizontal component that drives forward progression along the arc. The twist moment about the vertical axis — the free moment or torque about the foot contact point — encodes the rotational demand placed on the support limb and is particularly sensitive to the Bagua-specific simultaneous orbital and axial rotation. A circular array of eight six-axis force plates embedded in the floor, with plate centers positioned at the eight trigram directions, captures the complete force history of every footfall with trigram-indexed spatial context. The center of pressure trajectory within and across plates traces the load transfer path through the foot and between feet, providing a ground-level signature of the kinetic chain structure. In the port-Hamiltonian framework the GRF is a port variable — the effort-flow pair at the ground contact port — and its power product F.v gives the instantaneous mechanical power input from the ground into the body system. The Guardian Interface visualization renders this as a ground vector field: animated arrows at each plate and interpolated across the floor surface showing force direction, magnitude, and center of pressure trajectory as a glowing trail.

## Circular Plate Configuration

- Plate count: 8 plates minimum — 4m circle radius — one per trigram direction
- Plate geometry: square 600mm x 600mm — covers full foot for any step angle
- Center positions: at 0 45 90 135 180 225 270 315 degrees on 3m radius circle
- Trigram indexing: Qian 0 Xun 45 Zhen 90 Li 135 Kun 180 Gen 225 Kan 270 Dui 315
- Gap bridging: interpolated force field between plates using thin-plate spline
- Embedding: flush with floor surface — no trip hazard — sealed edges
- Synchronization: hardware trigger — all 8 plates simultaneous — sub-0.5ms
- Sample rate: 2000 Hz per plate — captures full transient of each foot strike

## GRF Decomposition

- Six-axis output: Fx Fy Fz Mx My Mz per plate per sample
- Vertical: Fz — body weight support — oscillates 0.8 to 1.2 BW in Bagua walking
- Centripetal horizontal: F_centripetal = m * omega^2 * r toward circle center
- Tangential horizontal: F_tangential — forward propulsion along arc
- Free moment: Mz — twist moment about vertical axis — rotational demand on foot
- CoP: center of pressure xp = -My/Fz yp = Mx/Fz — load centroid on plate
- Wrench: W = (F, tau) in se(3)* — complete ground contact descriptor
- Power: P_ground = F.v_CoP + tau.omega_foot — ground port power input

## Friction Cone Geometry

- Friction cone: set of all feasible friction forces F_f with ||F_f|| <= mu_s * Fz
- Static friction coefficient: mu_s approx 0.8 for rubber sole on hardwood floor
- Cone apex: at CoP on floor surface — cone opens upward in force space
- Slip condition: actual friction force exits cone — foot slides
- Bagua constraint: non-holonomic no-slip constraint requires F_f inside cone always
- Margin: safety margin = (mu_s * Fz - |F_f|) / (mu_s * Fz) — how close to slip
- Visualization: cone rendered as transparent wireframe at each plate in Layer 5
- Expert: expert practitioners show large friction cone margin — never near slip

## Twist Moment Analysis

- Free moment Mz: torque the body exerts on ground around vertical axis at CoP
- Bagua source: simultaneous orbital and axial rotation demands twist from support foot
- Peak moments: highest Mz at palm change — body reverses orbital direction
- Torsional load: Mz transmitted through foot ankle knee hip to spine
- TCM: Mz corresponds to rotational demand on Kidney channel — root stability
- Five element: Earth element receives and grounds the twist — Spleen Stomach channels
- Pathological: excessive Mz with low Fz indicates torsional overloading
- Guardian: Mz time series feeds Ba Gang deficiency-excess meter in real time

## Center of Pressure Trajectory

- CoP path: trace of load centroid across plate surface during single support
- Heel to toe: normal progression from heel strike through metatarsal push-off
- Bagua pattern: toe-in stance shifts CoP medially — inner foot loading
- Butterfly pattern: CoP traces figure-8 in double support transfer — Bagua signature
- Stability index: CoP deviation from foot center line — structural integrity measure
- Transfer path: inter-plate CoP transfer shows weight shift timing and smoothness
- Visualization: glowing CoP trail rendered on floor in Layer 5 — fades over time
- Expert: smooth continuous CoP trajectory with characteristic Bagua medial loading

## Port-Hamiltonian GRF Formulation

- Ground port: external port of body PH system — where environment inputs power
- Effort: GRF wrench W = (F_GRF, tau_GRF) in se(3)*
- Flow: foot velocity twist V = (v_CoP, omega_foot) in se(3)
- Port power: P_ground = <W, V> = F.v + tau.omega
- Energy source: P_ground > 0 means ground inputs energy to body — propulsion phase
- Energy sink: P_ground < 0 means body inputs energy to ground — braking phase
- Passivity: net energy from ground over full cycle = 0 for steady state walking
- TCM: P_ground is the physical measure of Earth element Qi input per step

## Ground Vector Field Visualization

- Floor arrows: GRF vector rendered at each plate center — length = magnitude
- Color: teal for propulsive (P > 0) — amber for braking (P < 0) — gray neutral
- Interpolation: thin-plate spline from 8 plates to full floor vector field
- CoP trail: glowing line on floor surface — last 2 seconds — fades to transparent
- Friction cones: transparent cone at each active plate — red rim when near slip
- Environmental sphere: force manifold volume rendered around practitioner body
- Power heatmap: floor surface colored by instantaneous ground power magnitude
- Ba Gang: ground power feed to Interior-Exterior meter — deep vs surface loading

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: BAGUA-FOUND-001, BAGUA-IMU-001, BAGUA-APPARATUS-001
- Related nodes: BIOM-GAIT-001, PORT-BODY-001, PORT-THERMO-001
- Related systems: BaguaViewer Layer 5, SomaticGraph, MCFSystem
- TCM: Earth element ground port — Spleen Stomach channel loading signature

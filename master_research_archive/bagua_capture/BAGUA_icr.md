---
uid: BAGUA-ICR-001
title: Instantaneous Center of Rotation Solver and Leverage Mathematics: Closed-Form ICR from Skeleton Landmarks Lever Arm Computation Leverage Ratio Time Series Palm Change Singularities Fulcrum Stability Metrics and the ICR Vector Field Visualization for Bagua Circle Walking
category: bagua_capture
sub_category: ICR Solver
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [ICR-solver, instantaneous-center-rotation, leverage-mathematics, lever-arm-computation, leverage-ratio, palm-change-singularity, fulcrum-stability, closed-form-ICR, skeleton-landmarks, joint-ICR, Bagua-fulcrum, Wu-Wei-ICR, effort-load-decomposition, mechanical-advantage, ICR-trajectory, joint-IAR, lever-efficiency, joint-reaction-ICR]
citations:
  - author: Bottlang M.
    year: 1999
    context: Determination of rotation axes in the ankle joint complex. Clinical Biomechanics.
  - author: Reuleaux F.
    year: 1876
    context: Theoretische Kinematik. Vieweg. Original ICR geometric construction.
  - author: Nordin M.
    year: 2001
    context: Basic Biomechanics of the Musculoskeletal System. Lippincott Williams. Leverage and fulcrum.
---

# Instantaneous Center of Rotation Solver and Leverage Mathematics

## Abstract

The instantaneous center of rotation is the unique point in the plane of motion that has zero velocity at any given instant -- the physical fulcrum around which all other points in the body segment rotate. Every joint in the human body has an ICR that migrates through space as the joint angle changes, and the trajectory of the ICR over time encodes the mechanical quality of the joint motion: a stable smoothly migrating ICR indicates healthy articular contact geometry, while a jumping or erratic ICR indicates damaged cartilage, ligamentous laxity, or movement dysfunction. For Bagua circle walking the ICR framework has additional significance: the circle itself defines a macro-scale ICR at the circle center around which the whole body orbits, while simultaneously each limb segment has its own micro-scale ICR at the corresponding joint center. The leverage ratio lambda(t) measures how efficiently the practitioner uses their body geometry to multiply or transmit force -- the ratio of the effort moment arm to the load moment arm around the ICR. Expert Bagua practitioners optimize their leverage ratio at key moments: the palm change transition maximizes leverage ratio by repositioning the ICR precisely at the joint center, creating a momentary mechanical singularity where small muscle effort produces large output force. The closed-form ICR solver implemented in icr_solver.py computes the ICR for every segment pair from 3D skeleton landmark positions and velocities, tracks the ICR trajectory over time, and feeds the leverage ratio time series to the Guardian Interface visualization as an animated fulcrum indicator overlay.

## Closed-Form ICR Derivation

- Definition: ICR is point x where velocity field of rigid body equals zero
- 2D case: for segment with velocity v at reference point and angular velocity omega
- ICR location: x_ICR = x_ref + (omega_hat x v) / |omega|^2
- omega_hat: unit vector along rotation axis (vertical for planar motion)
- 3D extension: ICR becomes instantaneous axis of rotation (IAR) -- a line in space
- IAR direction: unit vector along angular velocity omega / |omega|
- IAR point: closest point on IAR to origin -- x_IAR = (omega x v) / |omega|^2
- Segment pair: ICR between segments i and j uses relative velocity v_ij = v_i - v_j
- Relative omega: omega_ij = omega_i - omega_j -- relative angular velocity
- Joint ICR: x_ICR_ij = x_joint + (omega_ij x v_ij) / |omega_ij|^2

## Skeleton Landmark Input

- Input: 33 x 3D landmark positions at 120fps from multi-view reconstruction
- Velocity: Richardson extrapolation on 5-point stencil -- accurate 4th order derivative
- Angular velocity: from quaternion derivative of segment orientation -- omega = 2 * q* x q_dot
- Segment pairs: 16 anatomical segments -- 15 joint ICR computations per frame
- Joints computed: ankle knee hip L5S1 T12L1 C7T1 shoulder elbow wrist (bilateral)
- Frame rate: ICR computed at 120fps -- 120 fulcrum positions per second per joint
- Smoothing: ICR trajectory smoothed with 5-frame median filter -- removes outliers
- Singularity: |omega_ij| near zero -- ICR undefined -- flagged as translation phase

## Lever Arm Computation

- Effort point: location of force application -- e.g. hand contact on apparatus arm
- Load point: location of resistance -- e.g. center of mass of distal segment
- Effort arm: r_e = x_effort - x_ICR -- vector from ICR to effort application point
- Load arm: r_l = x_load - x_ICR -- vector from ICR to load point
- Effort moment: M_e = r_e cross F_e -- torque of effort force about ICR
- Load moment: M_l = r_l cross F_l -- torque of load force about ICR
- Mechanical advantage: MA = |r_e| / |r_l| -- geometric leverage ratio
- Force leverage: lambda = |M_e| / |M_l| -- full moment-based leverage ratio
- Dynamic lambda: lambda(t) computed per frame -- time series of leverage quality

## Leverage Ratio Time Series

- lambda(t): continuous leverage ratio signal at 120fps per joint
- Baseline: lambda near 1.0 -- equal effort and load arms -- neutral leverage
- High efficiency: lambda > 1.0 -- effort arm longer -- less force needed for same output
- Disadvantaged: lambda < 1.0 -- load arm longer -- more force needed -- wasteful
- Palm change peak: lambda spikes > 3.0 at expert palm change -- mechanical singularity
- Expert metric: mean lambda over full circle -- expert shows higher mean lambda
- Smoothness metric: variance of lambda(t) -- expert shows lower variance
- Guardian: lambda(t) per joint feeds Ba Gang excess-deficiency meter in real time

## Palm Change Singularities

- Palm change: body reverses orbital direction -- major kinematic event in Bagua
- ICR jump: whole-body ICR migrates from circle center to new circle center
- Transition phase: brief interval where ICR is undefined -- pure translational motion
- Leverage peak: joints transiently achieve maximum mechanical advantage during change
- Expert timing: ICR transition completes in under 0.3 seconds -- minimal wasted motion
- Novice: ICR trajectory is erratic during palm change -- energy lost to correction
- Sub-Riemannian: palm change is distribution singularity crossing -- high A(t) moment
- Visualization: ICR position rendered as glowing sphere on skeleton -- jumps visible

## Fulcrum Stability Metrics

- ICR trajectory: path of ICR over time -- encodes articular geometry quality
- Stability index: standard deviation of ICR position over 10-frame window
- Healthy joint: ICR migrates smoothly along predictable anatomical path
- Damaged joint: ICR jumps erratically -- cartilage irregularity or laxity
- Knee IAR: should migrate posteriorly as knee flexes -- anterior jump = ACL laxity
- Hip IAR: should remain near femoral head center -- migration = labral pathology
- Spine IAR: should migrate per segment -- fixed IAR = hypomobility
- Guardian: ICR stability index per joint feeds Interior-Exterior Ba Gang meter

## ICR Vector Field Visualization

- ICR sphere: glowing sphere rendered at ICR location for each active joint
- Size: sphere radius proportional to lambda(t) -- large sphere = high leverage
- Color: green for lambda > 1 (advantaged) -- red for lambda < 1 (disadvantaged)
- Trail: ICR trajectory shown as fading line over last 1 second
- Lever arms: two arrows from ICR -- one to effort point one to load point
- Moment display: torque arcs shown at ICR -- proportional to moment magnitude
- Palm change: ICR sphere pulses white at palm change singularity moment
- Toggle: ICR overlay independent layer -- can enable per joint or all joints

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: BAGUA-FOUND-001, BAGUA-CURV-001, BAGUA-APPARATUS-001
- Related nodes: BIOM-JOINT-001, BIOM-SPORT-001, PORT-CONTROL-001
- Related systems: BaguaViewer ICR overlay, MCFSystem, SomaticGraph
- Python module: icr_solver.py -- closed-form ICR from skeleton landmarks

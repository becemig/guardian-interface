---
uid: BAGUA-APPARATUS-001
title: Gyro Training Apparatus Design and Force Manifold Mapping: Eight-Arm Gimbal Hub Architecture Six-Axis Force-Torque Sensor Array Gyroscopic Flywheel Resistance Adjustable Magnetic Brake System Force Manifold Surface Computation Two-Person Recording Mode and the Environmental Force Sphere Visualization for Bagua Circle Walking Research
category: bagua_capture
sub_category: Gyro Apparatus
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [gyro-apparatus, force-manifold, eight-arm-gimbal, six-axis-FT-sensor, gyroscopic-flywheel, magnetic-brake, force-torque-capture, two-person-recording, environmental-force-sphere, wrench-capture, Peng-Jin-apparatus, port-Hamiltonian-apparatus, force-manifold-surface, structural-sphere-influence, gimbal-3DOF, apparatus-calibration, dual-practitioner-stream, bagua-apparatus-design]
citations:
  - author: Siciliano B.
    year: 2009
    context: Robotics: Modelling Planning and Control. Springer. Force-torque sensing and wrench.
  - author: Hacksel P.J.
    year: 1994
    context: Estimation of environment forces and rigid-body velocities using observers. IEEE ICRA.
  - author: Khatib O.
    year: 1987
    context: A unified approach for motion and force control of robot manipulators. IEEE Robotics.
---

# Gyro Training Apparatus Design and Force Manifold Mapping

## Abstract

The gyro training apparatus is the active force measurement and resistance component of the Bagua capture system -- the device that makes the invisible structural forces of internal martial arts physically measurable and visually renderable. Classical Bagua Zhang training uses a partner or a tree to provide resistance against which Peng Jin structural force is developed and refined. The apparatus replaces this with an instrumented eight-arm radial structure whose arms correspond to the eight Bagua trigram directions, each arm equipped with a six-axis force-torque sensor, a three-DOF gimbal that allows free deflection in any direction, an optical rotary encoder measuring deflection angle, an adjustable magnetic brake providing variable resistance from zero to fifty Newtons, and a gyroscopic flywheel providing realistic dynamic inertial resistance that responds to the rotational demands of internal force application in ways that simple spring resistance cannot replicate. The complete wrench -- force vector plus torque vector -- at each arm tip is measured at one kilohertz, providing a high-fidelity record of exactly what force the practitioner applies, in what direction, with what rotational character, at every moment of contact. From the eight-arm wrench data the force manifold is computed: the three-dimensional surface in force-direction space that encloses all force vectors the practitioner can apply -- the structural sphere of influence that the apparatus makes visible. This force manifold is the physical correlate of the TCM concept of structural sphere of Qi influence: the volume of space within which the practitioner can project force efficiently. Expert practitioners show large smooth convex force manifolds. Novices show small irregular manifolds with gaps in directions where structural force is unavailable. The two-person mode records independent wrench streams from two practitioners simultaneously, enabling analysis of force interaction patterns between partners.

## Eight-Arm Gimbal Hub Architecture

- Central hub: precision steel gimbal mount 80cm diameter -- floor or ceiling mount
- Eight arms: extend radially at 45-degree intervals -- Bagua trigram directions
- Arm material: carbon fiber tube 30mm diameter 1.2m length -- rigid and lightweight
- Gimbal per arm: 3-DOF bearing assembly at hub junction -- free deflection any direction
- Deflection range: plus minus 30 degrees in all directions -- full contact range
- Arm tips: padded contact bar 40cm width with rotating sleeve -- body and hand contact
- Height adjustment: arms on vertical rail -- set to practitioner height before session
- Trigram map: arm 0 = Qian north -- arm 1 = Xun northeast -- clockwise to arm 7 = Dui

## Six-Axis Force-Torque Sensor Array

- Sensor type: six-axis strain gauge -- Fx Fy Fz Mx My Mz per sensor
- Placement: between gimbal mount and arm tube -- all contact force passes through sensor
- Force range: 500N linear -- covers full Peng Jin structural force output
- Torque range: 50 Nm rotational -- covers full twist and rotational force demands
- Sample rate: 1000 Hz -- resolves all relevant force transients in Bagua contact
- Overload protection: mechanical stop at 150 percent rated load -- sensor protected
- Calibration: six-point static calibration per sensor -- gravity compensation applied
- Wrench output: W(t) = (F(t), tau(t)) in se(3)* -- complete contact descriptor

## Gyroscopic Flywheel Resistance

- Flywheel: 2kg steel disk 20cm diameter per arm -- moment of inertia 0.01 kg m^2
- Speed: 3000 rpm nominal -- gyroscopic stiffness proportional to spin speed
- Gyroscopic torque: tau_gyro = I * omega_spin cross omega_precession
- Precession resistance: deflecting the arm causes precession -- gyro resists realistically
- Dynamic character: gyro resistance is velocity-dependent -- models partner inertia
- Spring comparison: simple spring gives force proportional to deflection only
- Gyro advantage: resists both force and torque -- more realistic than spring
- Speed control: brushless motor maintains 3000 rpm -- variable speed option for training

## Adjustable Magnetic Brake System

- Brake type: eddy-current magnetic brake -- no contact -- no wear -- smooth resistance
- Range: 0 to 50N resistance at arm tip -- continuously adjustable per arm
- Control: PWM signal to electromagnet -- computer controlled per arm independently
- Response: less than 10ms to new set point -- can follow realtime force profiles
- Training modes: constant resistance -- progressive resistance -- reactive resistance
- Reactive mode: brake resistance proportional to practitioner force output -- mirrors Ting Jin
- Direction sensitivity: brake applies only along arm axis -- free lateral motion preserved
- Preset profiles: five-element profiles -- Wood increasing -- Water decreasing resistance

## Force Manifold Surface Computation

- Definition: F_manifold(q, d) = max force in direction d from configuration q
- Measurement: sweep arm through all directions -- record maximum force per direction
- Sampling: 162 directions from icosphere subdivision -- uniform directional coverage
- Surface: convex hull of force vectors in force space -- structural sphere of influence
- Volume: manifold volume = total structural force capacity -- expert metric
- Asymmetry: left-right manifold ratio -- structural balance assessment
- Gap detection: directions with manifold indentation -- structural weakness identified
- Visualization: translucent colored sphere around practitioner -- Layer 5 in BaguaViewer

## Two-Person Recording Mode

- Configuration: practitioner A on arms 0 1 2 3 -- practitioner B on arms 4 5 6 7
- Independent streams: wrench recorded separately per practitioner -- no mixing
- Interaction analysis: cross-correlation of force streams -- Ting Jin responsiveness
- Lead-follow: which practitioner initiates -- which responds -- timing analysis
- Force echo: practitioner B force magnitude as function of practitioner A force
- Ting Jin metric: response latency and force fidelity of following practitioner
- TCM: two-person mode captures Ting Jin listening force -- sensitivity to partner Qi
- Visualization: two colored force spheres side by side -- overlap region highlighted

## Environmental Force Sphere Visualization

- Sphere mesh: icosphere 162 vertices centered on practitioner pelvis
- Vertex color: force manifold magnitude in that direction -- teal high red low
- Radius: scaled to force magnitude -- sphere breathes with structural force output
- Rotation: sphere orientation tracks body orientation -- always body-relative
- Real-time: force manifold updated each arm contact event -- 1kHz input
- History: manifold morphs smoothly between contact events -- interpolated
- Gaps: low-force directions rendered as concave depressions -- weakness visible
- Five element: sphere sectors colored by five-element direction map -- Wood Fire Earth Metal Water

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: BAGUA-FOUND-001, BAGUA-FORCE-001, BAGUA-TCM-001
- Related nodes: PORT-CONTROL-001, PORT-BODY-001, BIOM-FASCIAL-001
- Related systems: BaguaViewer Layer 5, SomaticGraph, MCFSystem
- Python module: force_manifold.py -- manifold surface from sensor or inverse dynamics
- TCM: force manifold IS the physical structural Qi sphere -- measurable and trainable

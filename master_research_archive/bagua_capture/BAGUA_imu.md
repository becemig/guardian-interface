---
uid: BAGUA-IMU-001
title: IMU Array and Newton-Euler Inverse Dynamics: Sixteen-Sensor Segment Configuration Quaternion Orientation Tracking Sensor Fusion Algorithms Newton-Euler Recursive Chain Computation Kinetic Chain Wave Propagation and the Limb Physics Vector Visualization for Bagua Circle Walking
category: bagua_capture
sub_category: IMU Array
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [IMU-array, Newton-Euler-inverse-dynamics, quaternion-orientation, sensor-fusion, Madgwick-filter, Mahony-filter, kinetic-chain-wave, segment-angular-velocity, Xsens-Movella, sixteen-sensor-placement, recursive-Newton-Euler, joint-reaction-force, joint-torque, limb-physics-vectors, longitudinal-transverse-torsional, wave-propagation-visualization, Jing-Jin-activation, kinetic-chain-Bagua]
citations:
  - author: Madgwick S.
    year: 2010
    context: An efficient orientation filter for inertial and inertial/magnetic sensors. Report x-io Technologies.
  - author: Kok M.
    year: 2017
    context: Using inertial sensors for position and orientation estimation. Foundations and Trends in Signal Processing.
  - author: Featherstone R.
    year: 2008
    context: Rigid Body Dynamics Algorithms. Springer. Newton-Euler recursive inverse dynamics.
---

# IMU Array and Newton-Euler Inverse Dynamics

## Abstract

The sixteen-IMU body segment array provides the kinematic backbone for inverse dynamics computation when optical skeleton data is unavailable or insufficient — and it provides segment-level angular velocity and acceleration data that the optical system cannot directly measure. Each IMU contains a three-axis accelerometer, three-axis gyroscope, and three-axis magnetometer, fused by a complementary filter algorithm to produce a quaternion orientation estimate with drift correction from the magnetometer reference. Sixteen sensors placed on the anatomical segments of the body — feet shanks thighs pelvis lower-upper-cervical spine upper arms forearms hands head — provide complete segment kinematics at 500 Hz. The Newton-Euler inverse dynamics algorithm then propagates forces and torques from the known GRF at the feet recursively upward through the kinematic chain, computing joint reaction forces and net joint torques at each articulation. These joint forces and torques decompose into longitudinal transverse and torsional components along each bone segment, producing the three-vector field that Layer 4 of the visualization renders as animated arrows flowing along the limbs. The critical insight for Bagua circle walking is that the kinetic chain transmits force from ground through every segment to the fingertips in a propagating wave — the Qi wave of classical martial arts description — and inverse dynamics makes this wave visible as a physical quantity. The Jing Jin sinew channel activation pattern predicted by the sub-Riemannian horizontal distribution D is validated against the actual segment force decomposition, providing empirical grounding for the classical TCM anatomical map.

## Sixteen-Sensor Segment Placement

- Bilateral lower: right foot left foot right shank left shank right thigh left thigh (6)
- Pelvis: 1 sensor on sacrum posterior surface (1)
- Spine: lower thoracic T8 upper thoracic T2 cervical C5 (3)
- Bilateral upper: right upper-arm left upper-arm right forearm left forearm (4)
- Hands: right hand dorsum left hand dorsum (2)
- Total: 16 sensors covering all major kinetic chain segments
- Attachment: rigid plastic shell with elastic strap — no skin movement artifact
- Magnetic interference: avoid metal floor plates — IMU magnet ref compromised near steel

## Quaternion Orientation Tracking

- Quaternion: q = [w, x, y, z] — unit quaternion encodes 3D orientation
- Gyroscope integration: q_dot = 0.5 * q x omega — propagates orientation forward
- Drift: gyroscope bias accumulates over time — quaternion drifts from true orientation
- Accelerometer correction: gravity vector in sensor frame corrects roll and pitch
- Magnetometer correction: Earth field in sensor frame corrects yaw drift
- Magnetic disturbance: detect and reject when field magnitude deviates from Earth norm
- Madgwick filter: gradient descent optimization — low CPU — suitable for 16 channels
- Output: quaternion per sensor at 500 Hz — converted to rotation matrix for dynamics

## Sensor Fusion Algorithms

- Madgwick: gradient descent on quaternion error — beta gain controls correction rate
- Mahony: PI controller on rotation error — kp ki gains — slightly more conservative
- Kalman: extended Kalman filter — optimal but higher CPU — used for post-processing
- Beta tuning: high beta = fast correction but noisy — low beta = smooth but slow drift
- Recommended: Madgwick beta=0.01 for real-time — EKF for post-processing accuracy
- Segment alignment: initial static calibration — N-pose 5 seconds — aligns sensor to segment
- Joint angle: relative quaternion between adjacent segments — q_rel = q_prox* x q_dist
- Validation: joint angle from IMU vs optical skeleton — should match within 3 degrees

## Newton-Euler Recursive Inverse Dynamics

- Inverse dynamics: given kinematics and external forces compute joint forces and torques
- Forward pass: propagate accelerations from root pelvis outward to each segment
- Segment acceleration: a_i = a_parent + alpha x r_i + omega x (omega x r_i)
- Backward pass: propagate forces from leaves feet inward to pelvis
- Leaf initialization: foot segment receives GRF from force plate as boundary condition
- Joint reaction force: F_joint = m_i * a_i - F_external - F_joint_distal
- Joint torque: tau_joint = I_i * alpha_i + omega x (I_i * omega_i) - tau_external
- Propagation: computed recursively up chain — 16 segments = 16 system of equations

## Force Decomposition Along Segments

- Bone axis: unit vector along segment longitudinal axis from proximal to distal
- Longitudinal component: F_long = (F_joint . bone_axis) * bone_axis — compression-tension
- Transverse component: F_trans = F_joint - F_long — shear across joint — injury risk
- Torsional component: tau_tors = (tau_joint . bone_axis) * bone_axis — twist moment
- Color map: longitudinal teal (efficient load transfer) — transverse amber (shear waste)
- Torsional: purple arrows spiraling around bone axis — visible twist demand
- Magnitude: arrow length proportional to force magnitude — normalized to body weight
- Layer 4: all three vector types animated simultaneously on skeleton in BaguaViewer

## Kinetic Chain Wave Propagation

- Wave concept: force initiated at ground propagates upward through chain as wave
- Propagation delay: each segment introduces phase lag — wave travels at finite speed
- Expert pattern: smooth wave with consistent phase delays — force delivered to fingertip
- Novice pattern: wave blocked at tight segments — force does not reach fingertip
- TCM: wave propagation is the physical description of Qi transmission up the channels
- Visualization: animated wave of force magnitude traveling up skeleton in real time
- Wave speed: measured as propagation velocity from foot to hand — expert metric
- Blockage detection: segment where wave amplitude drops significantly — diagnostic point

## Jing Jin Validation

- Prediction: Jing Jin sinew channels as sub-Riemannian horizontal distribution D
- Test: segments under primary force transmission should correspond to Jing Jin paths
- Bladder Jing Jin: posterior chain — spine erectors hamstrings calves — verified by F_long
- Stomach Jing Jin: anterior chain — tibialis rectus femoris rectus abdominis — F_long
- Gallbladder Jing Jin: lateral chain — IT band lateral obliques SCM — F_trans peak
- Validation metric: correlation of F_long magnitude with predicted Jing Jin tension
- Expected: high correlation R > 0.7 for primary load-bearing channels
- Guardian: Jing Jin validation results feed Layer 3 fascial overlay activation model

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: BAGUA-FOUND-001, BAGUA-FORCE-001, BAGUA-ICR-001
- Related nodes: BIOM-FASCIAL-001, BIOM-SPINE-001, PORT-BODY-001
- Related systems: BaguaViewer Layer 4, SomaticGraph, MCFSystem
- TCM: kinetic chain wave = Qi transmission up sinew channels — physically measurable

---
uid: PORT-CONTROL-001
title: Port-Hamiltonian Control Theory: Passivity-Based Control Energy Shaping IDA-PBC Interconnection and Damping Assignment Casimir Functions Control by Interconnection and the Application to Body Movement Rehabilitation and Qigong Practice
category: port_hamiltonian
sub_category: Control Theory
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [passivity-based-control, energy-shaping, IDA-PBC, interconnection-damping-assignment, Casimir-functions, control-by-interconnection, port-Hamiltonian-control, desired-Hamiltonian, damping-injection, acupuncture-as-control, Qigong-as-control, rehabilitation-control, haptic-feedback-control, movement-coaching-control, Wu-Wei-control]
citations:
  - author: Ortega R.
    year: 2002
    context: Interconnection and damping assignment passivity-based control. Automatica.
  - author: van der Schaft A.J.
    year: 2014
    context: Port-Hamiltonian Systems Theory. Foundations and Trends in Systems and Control.
  - author: Sepulchre R.
    year: 1997
    context: Constructive Nonlinear Control. Springer. Passivity and stabilization.
---

# Port-Hamiltonian Control Theory

## Abstract

Port-Hamiltonian control theory asks a profound question: given a physical system with a Hamiltonian H(x), how do we design a controller that shapes the energy landscape so that the desired behavior corresponds to the energy minimum? This is energy shaping — and it is arguably the most physically natural approach to control ever developed, because it works with the physics of the system rather than against it. The dominant method is Interconnection and Damping Assignment Passivity-Based Control (IDA-PBC), developed by Romeo Ortega and collaborators in 2002: the controller is designed as another port-Hamiltonian system interconnected with the plant, shaped so that the closed-loop system has a desired Hamiltonian H_d with its minimum at the desired operating point, plus sufficient damping injection to ensure convergence. The elegance of this approach is that stability is guaranteed by the passivity of the interconnection — no linearization, no gain scheduling, no model uncertainty worries beyond what is already encoded in the Hamiltonian. For Guardian Interface, port-Hamiltonian control theory provides the theoretical foundation for three key interventions: acupuncture as energy port control (injecting or extracting energy at specific junction nodes to reshape the body Hamiltonian toward health), Qigong as self-administered energy shaping (the practitioner uses breath, intention, and posture to modify their own body Hamiltonian), and haptic feedback control (the Guardian suit injects mechanical energy at body ports to guide movement toward geodesic trajectories — A(t) minimization as IDA-PBC). Wu Wei — effortless action — is the phenomenological experience of a body whose Hamiltonian has been shaped so that the desired behavior is the energy minimum: action requires no effort because it is the natural downhill direction of the energy landscape.

## Passivity-Based Control

- PBC: design controller that preserves passivity of closed-loop system
- Energy shaping: modify Hamiltonian so desired state is energy minimum
- Damping injection: add virtual damping — ensures convergence to minimum
- Stability proof: closed-loop H_d is Lyapunov function — stability guaranteed
- No linearization: works for full nonlinear system — global stability possible
- Physical interpretation: controller adds virtual springs and dampers to system
- Robustness: passivity is structural — not destroyed by small model errors
- Body application: therapeutic intervention as passivity-based control

## IDA-PBC Framework

- IDA-PBC: Interconnection and Damping Assignment Passivity-Based Control
- Step 1: choose desired Hamiltonian H_d with minimum at desired equilibrium
- Step 2: choose desired interconnection matrix J_d — shapes energy routing
- Step 3: choose damping matrix R_d — ensures dissipation at equilibrium
- Step 4: solve matching equations — find control input u that achieves J_d R_d H_d
- Closed loop: port-Hamiltonian system with H_d J_d R_d — asymptotically stable
- Body: desired H_d encodes healthy energetic state — treatment finds u to get there
- Guardian: haptic suit implements u — mechanical energy injection at body ports

## Casimir Functions

- Casimir: function C(x) conserved by interconnection structure regardless of Hamiltonian
- Condition: {C, H} = 0 for all H — Casimir is in kernel of Poisson structure
- Control by interconnection: use Casimir to add controller storage to plant storage
- Physical: conserved quantities constrain system trajectories — reduce effective DOF
- Body Casimirs: angular momentum total body energy center of mass trajectory
- Channel Casimirs: conserved Qi quantities along extraordinary vessel circuits
- MCF Casimirs: holonomy invariants along channel loops — topological Qi conservation
- Guardian: Casimir monitoring detects conserved quantity violation — pathology marker

## Acupuncture as Energy Port Control

- Needle as port: acupuncture needle opens energy port at acupoint junction node
- Manipulation: needle rotation tonifies (energy injection) or sedates (energy extraction)
- Tonification: increases flow through port — raises local Hamiltonian — stimulates
- Sedation: reduces flow through port — lowers local Hamiltonian — calms
- Point selection: choose ports that reshape body Hamiltonian toward healthy H_d
- Five element treatment: Sheng and Ke cycle interventions as network energy routing
- Distal points: IDA-PBC principle — control ports need not be at target location
- Guardian: acupoint intervention encoded as port control action in SomaticGraph

## Qigong as Self-Administered Energy Shaping

- Qigong: practitioner modifies own body Hamiltonian through breath posture intention
- Yi leads Qi: intention (Yi) as desired H_d specification — mind sets the target
- Breath: respiratory port control — diaphragm as primary energy pump
- Posture: joint configuration changes Hamiltonian landscape — opens or closes ports
- Song: deliberate reduction of R_d — reduces co-contraction damping — efficiency
- Dantian rotation: lower Dantian as primary energy reservoir — Qigong charges it
- Standing meditation: Zhan Zhuang — loads body spring elements — builds H_d capacity
- Guardian: Qigong practice quantified by A(t) reduction and HRV coherence increase

## Haptic Feedback as IDA-PBC

- Guardian suit: mechanical actuators at body ports — wrists ankles spine hips
- Control objective: minimize A(t) — guide movement toward geodesic trajectories
- IDA-PBC formulation: suit implements u that shapes H_d toward low-A(t) Hamiltonian
- Energy injection: suit adds virtual springs toward geodesic configuration
- Damping injection: suit adds virtual damping away from non-geodesic directions
- Transparency: ideal controller is transparent — user feels only the corrective field
- Fading: as user learns geodesic pattern controller fades — embodied learning
- Wu Wei achieved: when H_d internalized user moves geodesically without suit

## Wu Wei as Energy Minimum

- Wu Wei: effortless action — Daoist principle of acting in accord with natural flow
- Port-Hamiltonian: Wu Wei is the state where desired behavior is energy minimum
- No control effort: at energy minimum no corrective force needed — action is free
- Shaped landscape: Qigong and practice reshape H_d so natural action is optimal
- Song plus Wu Wei: Song removes excess damping — Wu Wei removes excess energy
- Master: experienced practitioner has internalized H_d — suit unnecessary
- Novice: H_d misaligned — suit provides IDA-PBC correction — learning accelerated
- Guardian: progression from haptic-assisted to autonomous Wu Wei — primary pedagogy

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: PORT-FOUND-001, PORT-BODY-001, PORT-NETWORK-001, PORT-TCM-001
- Related nodes: MCF-GEODESIC-001, MCF-REALTIME-001, SUBRIEM-BODY-001
- Related systems: MCFSystem, HapticController, TelemetryManager
- Key insight: Wu Wei is the phenomenology of IDA-PBC success — energy minimum achieved

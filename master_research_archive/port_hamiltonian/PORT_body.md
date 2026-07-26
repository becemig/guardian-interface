---
uid: PORT-BODY-001
title: The Body as Port-Hamiltonian System: Joints as Power Ports Muscles as Actuators Organs as Energy Storage and Dissipation Elements Qi as Power Flow Channel Network as Bond Graph and the Unified Energetic Architecture of the Living Body
category: port_hamiltonian
sub_category: Body as System
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [body-port-Hamiltonian, joints-as-ports, muscles-as-actuators, organs-energy-storage, Qi-power-flow, channel-bond-graph, body-energy-architecture, multibody-Hamiltonian, muscle-port-model, organ-capacitance, Jing-energy-storage, Shen-information-port, fascia-transmission-line, dantian-energy-reservoir, TCM-port-topology]
citations:
  - author: van der Schaft A.J.
    year: 2014
    context: Port-Hamiltonian Systems Theory. Foundations and Trends in Systems and Control.
  - author: Duindam V.
    year: 2009
    context: Modeling and Control of Complex Physical Systems. Springer. Port-Hamiltonian approach.
  - author: Unschuld P.U.
    year: 2003
    context: Huang Di Nei Jing Su Wen. University of California Press. TCM energetic foundations.
---

# The Body as Port-Hamiltonian System

## Abstract

The human body is, from the perspective of port-Hamiltonian systems theory, the most sophisticated energy processing network in the known universe. Every physiological process — muscle contraction, neural signaling, metabolic transformation, hormonal regulation, immune response — is fundamentally an energy transaction: energy is stored, released, routed, transformed, and dissipated through a hierarchically organized network of ports, junctions, storage elements, and dissipation elements. The genius of Traditional Chinese Medicine is that it developed, through millennia of systematic clinical observation, a functional map of precisely this network — not in the language of physics and mathematics, but in the language of Qi, Blood, channels, and organs. Qi is power flow: the product of effort and flow variables at each port in the body network. The twelve primary channels are transmission lines routing power between organ systems. The acupoints are junction structures where power can be injected, extracted, or redirected. The five Zang organs are energy storage elements — each storing a different form of body energy: Kidney stores Jing (constitutional potential energy), Heart stores Shen (information-theoretic energy), Liver stores Blood (hydraulic potential energy), Lung stores Wei Qi (surface kinetic energy), Spleen stores Gu Qi (metabolic chemical energy). The three Dantian are capacitive reservoirs — the lower Dantian storing the largest energy reserve in the body network, the middle Dantian routing cardiac and respiratory energy, the upper Dantian integrating neural and sensory energy. This framework does not reduce TCM to physics — it reveals that TCM and physics are describing the same energetic reality at different levels of resolution and with different conceptual vocabularies.

## Multibody Hamiltonian Structure

- Body as multibody system: N rigid segments connected by joints
- Each segment: I element — inertia tensor — stores kinetic energy H_k = 0.5 p^T M^-1 p
- Each joint: 1-junction — common velocity — sums torques from all connected elements
- Gravitational potential: C element at each segment center of mass
- Total Hamiltonian: H = sum of all segment kinetic plus all gravitational potential
- Equations of motion: canonical Hamiltonian equations on joint configuration space
- Constraints: holonomic constraints reduce configuration space — sub-Riemannian if non-holonomic
- MCF link: body Hamiltonian H is the energy function on sub-Riemannian manifold M

## Joints as Power Ports

- Joint port: (tau, omega) — torque effort, angular velocity flow — P = tau times omega
- Effort: torque from all muscles ligaments and external forces at that joint
- Flow: angular velocity of joint — kinematic output
- Hinge joint: one-dimensional port — single torque-velocity pair
- Ball and socket: three-dimensional port — three torque-velocity pairs
- Passive joint: stores elastic energy in ligaments and capsule — C element in parallel
- Acupoint at joint: junction structure — port where Qi can be accessed clinically
- MCF: joint port curvature equals sub-Riemannian curvature at that configuration

## Muscles as Actuators

- Muscle port: (F, v) — force effort, contraction velocity flow — P = F times v
- Active element: SE source — generates effort from metabolic energy input
- Elastic element: tendon as C element — nonlinear spring — toe region plus linear
- Viscous element: R element — velocity-dependent damping — energy dissipation
- Hill model: SE source plus C element plus R element in series — three-element muscle
- Metabolic input: chemical energy port — ATP as effort source — metabolic Qi
- Eccentric: muscle absorbs power — negative power flow — energy storage in tendon
- Concentric: muscle delivers power — positive power flow — Qi activation

## Organs as Energy Storage Elements

- Kidney Zang: Jing storage — constitutional potential energy — deepest C element
- Kidney port: (essence pressure, Jing flow) — slow discharge — lifetime energy reserve
- Heart Zang: Shen storage — information-theoretic energy — neural integration
- Heart port: (blood pressure, cardiac output) — P = MAP times CO — cardiac power
- Liver Zang: Blood storage — hydraulic reservoir — smooth muscle tension regulator
- Liver port: (hepatic pressure, portal flow) — metabolic gateway port
- Lung Zang: Wei Qi generation — respiratory energy — ventilatory power
- Lung port: (alveolar pressure, respiratory flow) — P = pressure times tidal volume rate
- Spleen Zang: Gu Qi transformation — metabolic chemical energy conversion

## Dantian as Capacitive Reservoirs

- Lower Dantian: primary energy reservoir — largest C element in body network
- Location: 3 cun below navel — center of gravity — mechanical energy centroid
- Lower function: stores prenatal Jing potential — releases during exertion
- Middle Dantian: cardiac respiratory energy junction — thoracic power hub
- Middle function: routes Heart and Lung Qi — emotional energy regulation
- Upper Dantian: neural sensory integration — information energy reservoir
- Upper function: Shen residence — integrates all sensory and cognitive energy flows
- Bond graph: three Dantian as three primary C elements — body energy hierarchy

## Qi as Power Flow

- Classical Qi: vital energy animating all body functions — flows through channels
- Port-Hamiltonian: Qi = instantaneous power flow P = e times f at each port
- Sufficient Qi: adequate power flow — organ functions normally — health
- Deficient Qi: insufficient power flow — organ underperforms — deficiency pattern
- Stagnant Qi: blocked power flow — port impedance mismatch — stagnation pattern
- Rebellious Qi: reversed power flow — negative effort or flow — counterflow pattern
- Excess Qi: overflow power — storage element saturated — excess pattern
- Guardian: telemetry channels measure power flow proxies — HRV as cardiac Qi metric

## Channel Network as Bond Graph

- Twelve primary channels: transmission lines — route power between organ ports
- Channel as line: effort propagates along channel — Qi wave is pressure wave
- Acupoint as junction: 0-junction or 1-junction — effort or flow summation node
- Source points: SE source character — maximum Qi generation from organ
- He-sea points: deep confluence — large C element — energy reservoir access
- Luo points: connecting channels — lateral power transfer between paired channels
- Xi-cleft points: acute accumulation — sharp C element — emergency energy storage
- Guardian: channel graph as bond graph — encodable in SomaticGraph network structure

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: PORT-FOUND-001, PORT-NETWORK-001, PORT-TCM-001
- Related nodes: MCF-TCM-001, SUBRIEM-TCM-001, BIOM-TCM-001
- Related systems: MCFSystem, SomaticGraph, TelemetryManager
- Central claim: Qi IS power flow — channels ARE transmission lines — organs ARE storage elements

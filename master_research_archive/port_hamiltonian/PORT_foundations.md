---
uid: PORT-FOUND-001
title: Port-Hamiltonian Systems Foundations: Hamiltonian Mechanics Energy Ports Storage Elements Dissipation Elements Junction Structure Bond Graphs Dirac Structures and the Unified Energy-Based Modeling Framework
category: port_hamiltonian
sub_category: Foundations
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [port-Hamiltonian, Hamiltonian-mechanics, energy-ports, storage-elements, dissipation-elements, junction-structure, bond-graphs, Dirac-structure, passivity, energy-based-modeling, van-der-Schaft, Maschke, symplectic-geometry, Legendre-transform, generalized-coordinates, conjugate-variables, power-flow, effort-flow]
citations:
  - author: van der Schaft A.J.
    year: 2014
    context: Port-Hamiltonian Systems Theory. Foundations and Trends in Systems and Control.
  - author: Maschke B.M.
    year: 1992
    context: Port-controlled Hamiltonian systems: modelling origins and systemtheoretic properties.
  - author: Paynter H.M.
    year: 1961
    context: Analysis and Design of Engineering Systems. MIT Press. Bond graph foundations.
---

# Port-Hamiltonian Systems Foundations

## Abstract

Port-Hamiltonian systems theory is the most powerful unified framework for modeling physical systems across energy domains — mechanical, electrical, hydraulic, thermal, chemical — by focusing on the universal currency that all physical processes share: energy. Developed by Arjan van der Schaft and Bernhard Maschke in the early 1990s, building on Henry Paynter's bond graph formalism and the classical Hamiltonian mechanics of Hamilton and Jacobi, port-Hamiltonian theory represents a physical system as a network of energy storage elements, energy dissipation elements, and energy ports through which the system exchanges energy with its environment. The central insight is that all physical interaction is energy exchange — every interface between a system and its environment, or between subsystems, is a port through which power flows as the product of an effort variable and a flow variable. The mathematical structure that encodes how energy flows through the network without loss is the Dirac structure — a generalization of the symplectic structure of classical Hamiltonian mechanics to systems with ports and dissipation. Port-Hamiltonian systems are inherently passive: they cannot generate energy, only store, dissipate, and route it. This passivity property is not merely mathematical convenience — it is the physical law of energy conservation expressed in the most general possible form. For the Guardian Interface project, port-Hamiltonian theory provides the energy-theoretic foundation that sub-Riemannian geometry provides the kinematic foundation: while MCF describes the geometry of body movement, port-Hamiltonian theory describes the energy flows that drive and sustain it. Qi, in this framework, is not metaphor — it is literally the power flow through the body's port-Hamiltonian network, with channels as transmission lines, acupoints as junction structures, and organs as energy storage and dissipation elements.

## Hamiltonian Mechanics Review

- Lagrangian: L = T - V — kinetic minus potential energy — equations of motion
- Legendre transform: converts Lagrangian to Hamiltonian — generalized momenta
- Hamiltonian: H = T + V — total energy — sum of kinetic and potential
- Canonical equations: dq/dt = partial H / partial p — dp/dt = minus partial H / partial q
- Symplectic structure: phase space geometry — area-preserving flow — Liouville theorem
- Conservation: dH/dt = 0 for autonomous system — energy conserved along trajectories
- Poisson bracket: {f,g} — fundamental operation of Hamiltonian mechanics
- Generalization: port-Hamiltonian extends to open systems — energy exchange with environment

## Bond Graphs and Power Variables

- Bond graph: graphical representation of power flow in physical network — Paynter 1961
- Power bond: directed line representing energy exchange between elements
- Effort variable e: force voltage pressure temperature — generalized force
- Flow variable f: velocity current flow rate entropy rate — generalized velocity
- Power: P = e times f — product of effort and flow — universal across all domains
- Mechanical: effort = force, flow = velocity, P = F times v
- Electrical: effort = voltage, flow = current, P = V times I
- Hydraulic: effort = pressure, flow = volumetric flow rate
- Thermal: effort = temperature, flow = entropy rate

## Port-Hamiltonian Structure

- State: x in X — energy variables — generalized positions and momenta
- Hamiltonian: H(x) — total stored energy — smooth function on state space
- Dirac structure: D — encodes power-preserving interconnection — generalized symplectic
- Storage port: (f_S, e_S) — energy storage — capacitors inductors springs masses
- Dissipation port: (f_R, e_R) — energy dissipation — resistors dampers
- External port: (f_P, e_P) — energy exchange with environment — inputs and outputs
- Power balance: dH/dt = e_P times f_P minus dissipation — power in minus losses
- Passivity: dH/dt less than or equal to e_P times f_P — cannot generate energy

## Dirac Structures

- Dirac structure: generalization of symplectic and Poisson structures to port systems
- Definition: subspace D of (F times E) such that power is zero on D
- Telematrix: represents Dirac structure as matrix equation — J and B matrices
- Skew-symmetry: J = minus J^T — encodes energy-conserving internal routing
- Junctions: 0-junction (common effort) and 1-junction (common flow) — bond graph
- Composition: Dirac structures compose — subsystem interconnection preserves structure
- Geometric: Dirac structure on manifold — extends to field theories and continua
- MCF link: Dirac structure on body manifold M — symplectic complement of sub-Riemannian D

## Energy Storage and Dissipation Elements

- C element: capacitor spring — stores energy as function of displacement — e = partial H / partial q
- I element: inductor mass — stores energy as function of momentum — f = partial H / partial p
- R element: resistor damper — dissipates energy — e_R = R times f_R
- SE source: effort source — voltage source force source — external energy input
- SF source: flow source — current source velocity source
- Multibody: each rigid body segment is I element — joints are 1-junctions
- Muscle: combined C element (elastic) plus R element (viscous) plus SE source (active)
- Tendon: C element with nonlinear stiffness — toe region plus linear region

## Passivity and Stability

- Passivity: system cannot generate energy from nothing — fundamental physical constraint
- Supply rate: w(e,f) = e times f — power supplied to system
- Storage function: H(x) — energy stored — Lyapunov function for stability
- Dissipation inequality: H(x(T)) minus H(x(0)) less than or equal to integral of w dt
- Stability: passive systems with dissipation are asymptotically stable around minima
- Song: Wu Wei as energetically passive state — minimum energy geodesic — natural stability
- MCF: attunement scalar A(t) near zero equals near-passive motion — energy efficient

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: PORT-BODY-001, PORT-NETWORK-001, PORT-CONTROL-001
- Related nodes: MCF-MANIFOLD-001, SUBRIEM-FOUND-001, BIOM-FOUND-001
- Related systems: MCFSystem, SomaticGraph
- Key insight: Qi IS power flow in body port-Hamiltonian network — effort times flow

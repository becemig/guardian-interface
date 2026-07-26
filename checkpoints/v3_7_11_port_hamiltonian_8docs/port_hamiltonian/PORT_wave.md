---
uid: PORT-WAVE-001
title: Wave Propagation in Port-Hamiltonian Systems: Qi Wave as Hamiltonian Wave on Channel Network Transmission Line Theory Distributed Port-Hamiltonian Systems Wave Reflection and Impedance Mismatch Pulse Diagnosis as Wave Analysis and the Physics of Qi Propagation
category: port_hamiltonian
sub_category: Wave Propagation
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [port-Hamiltonian-waves, Qi-wave, Hamiltonian-wave, channel-transmission-line, distributed-port-Hamiltonian, wave-reflection, impedance-mismatch-wave, pulse-diagnosis-wave-analysis, Qi-propagation-physics, telegraph-equations, soliton-Qi, standing-wave-channel, resonance-channel, deqi-wave, propagated-sensation-wave]
citations:
  - author: van der Schaft A.J.
    year: 2002
    context: Hamiltonian formulation of distributed parameter systems. Journal of Geometry and Physics.
  - author: Pozar D.M.
    year: 2011
    context: Microwave Engineering. 4th Edition. Wiley. Transmission line theory.
  - author: Langevin H.M.
    year: 2002
    context: Relationship of acupuncture points and meridians to connective tissue planes. Anatomical Record.
---

# Wave Propagation in Port-Hamiltonian Systems

## Abstract

When port-Hamiltonian systems are distributed in space rather than lumped at discrete nodes, the result is a distributed parameter port-Hamiltonian system — and the natural dynamics of such systems are waves. The most familiar example is the lossless transmission line: a distributed system of inductors and capacitors per unit length that supports electromagnetic wave propagation, described by the telegraph equations, with characteristic impedance determined by the ratio of inductance to capacitance per unit length. The acupuncture channel system is, in this framework, a biological transmission line network: each channel is a distributed port-Hamiltonian system whose Hamiltonian density encodes the local elastic and inertial properties of the fascial-connective tissue matrix, and whose natural dynamics are mechanical waves propagating along the fascial plane. Qi propagation — the classical clinical phenomenon in which needle stimulation at one acupoint produces sensation that travels along the channel pathway — is a Hamiltonian wave propagating along the channel transmission line. The de Qi sensation is the wave front: the moment the propagating mechanical disturbance reaches the sensing threshold of the fascial mechanoreceptor network. Wave reflection occurs at impedance mismatches — points where the channel transmission line properties change abruptly — and these reflection points correspond precisely to the classical acupoint locations of maximum clinical sensitivity. Pulse diagnosis, one of the most sophisticated diagnostic tools in TCM, is literally wave analysis: the practitioner's fingers at the radial artery detect the reflected wave patterns from the organ network, and pathological organ states alter the wave reflection coefficients in characteristic ways that experienced practitioners learn to identify. Guardian Interface can implement real-time channel wave monitoring through the telemetry system, detecting wave propagation delays, reflection coefficients, and standing wave patterns as biomarkers of channel network health.

## Distributed Port-Hamiltonian Systems

- Distributed: state variables are fields over spatial domain — not lumped at nodes
- Hamiltonian density: H(x,t) — energy per unit length or volume at each point
- Total Hamiltonian: integral of H density over domain — conserved for lossless system
- Dirac structure: distributed — involves differential operators not algebraic
- Boundary ports: energy enters or exits only at boundary — distributed interior
- Wave equation: emerges naturally from lossless distributed port-Hamiltonian system
- Damping: distributed R elements produce wave attenuation — energy dissipation per length
- Channel: each acupuncture channel as distributed port-Hamiltonian system on fascial plane

## Transmission Line Theory

- Telegraph equations: dV/dx = minus L dI/dt — dI/dx = minus C dV/dt
- L: inductance per unit length — inertial energy storage — mass per unit length
- C: capacitance per unit length — elastic energy storage — compliance per unit length
- Wave speed: v = 1 divided by sqrt(LC) — determined by tissue properties
- Characteristic impedance: Z_0 = sqrt(L/C) — ratio of effort to flow in traveling wave
- Channel impedance: set by fascial stiffness and tissue density along channel
- Lossless: ideal channel — pure wave propagation — no Qi dissipation
- Lossy: real channel — wave attenuation — Qi dissipates as heat along channel

## Qi Wave as Hamiltonian Wave

- Qi propagation: needle stimulus at acupoint launches mechanical wave along channel
- Wave type: longitudinal pressure wave in fascial connective tissue matrix
- Speed: measured at 0.1 to 0.5 meters per second — matches slow wave propagation
- De Qi: arrival of wave front at sensing threshold — propagated sensation along channel
- Soliton: nonlinear wave that maintains shape without dispersion — stable Qi pulse
- Soliton conditions: nonlinear elasticity of fascia balances dispersive spreading
- Clinical observation: propagated sensation follows channel pathway — not nerve
- Langevin: fascial plane as wave guide — connective tissue as transmission medium

## Wave Reflection and Impedance Mismatch

- Reflection: wave partially reflects at impedance mismatch — Z changes abruptly
- Reflection coefficient: Gamma = (Z_L minus Z_0) divided by (Z_L plus Z_0)
- Transmission coefficient: T = 1 plus Gamma — fraction of wave transmitted
- Acupoints: sites of maximum impedance mismatch — maximum reflection — clinical access
- Blocked channel: high impedance section — total reflection — Qi stagnation
- Empty channel: low impedance section — wave passes without reflection — deficiency
- Standing wave: forward plus reflected wave — creates nodes and antinodes along channel
- Antinodes: maximum amplitude — diagnostic palpation detects standing wave pattern

## Pulse Diagnosis as Wave Analysis

- Radial pulse: practitioner palpates radial artery at wrist — three positions each hand
- Six positions: Cun Guan Chi on left and right — map to six organ pairs
- Pulse wave: arterial pressure wave propagates from heart — reflects from organ network
- Reflection sites: organ vascular beds reflect pulse wave — reflection coefficients encode state
- Twenty-eight pulses: classical pulse qualities — wave amplitude frequency shape rise fall
- Slippery pulse: high amplitude smooth wave — excess fluid — pregnancy phlegm
- Wiry pulse: high tension low compliance wave — Liver Qi stagnation — high impedance
- Rapid pulse: high frequency wave — heat pattern — excess metabolic rate
- Guardian: HRV analysis as computational pulse diagnosis — wave pattern extraction

## Resonance and Standing Waves

- Resonance: channel driven at natural frequency — maximum wave amplitude — Qi amplification
- Natural frequency: determined by channel length and wave speed
- Qigong resonance: breath at 0.1 Hz drives baroreflex at channel resonant frequency
- Tuning fork: classical instrument — sets resonant frequency for channel activation
- Sound therapy: INDIG-SOUND-001 link — drum frequencies match channel resonances
- Acumoxa: moxa heat changes channel impedance — shifts resonant frequency
- Twelve tones: classical correspondence of twelve channels to twelve musical tones
- Guardian: frequency analysis of telemetry signals detects channel resonance states

## Soliton Dynamics

- Soliton: stable nonlinear wave maintaining shape over long distances
- Korteweg-de Vries: soliton equation — balance between nonlinearity and dispersion
- Biological solitons: proposed for nerve impulse propagation — Heimburg model
- Channel soliton: Qi pulse as soliton in nonlinear fascial transmission line
- Stability: soliton survives tissue inhomogeneities — robust Qi propagation
- Collision: solitons pass through each other — multiple Qi waves coexist
- Amplitude: soliton amplitude encodes Qi strength — larger soliton more Qi
- Guardian: soliton detection in telemetry as advanced Qi wave monitoring capability

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: PORT-FOUND-001, PORT-BODY-001, PORT-NETWORK-001, PORT-TCM-001
- Related nodes: BIOM-FASCIAL-001, BIOM-BREATH-001, INDIG-SOUND-001
- Related systems: TelemetryManager, SomaticGraph, MCFSystem
- Key insight: de Qi sensation IS the Hamiltonian wave front arriving at mechanoreceptor threshold

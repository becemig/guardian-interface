---
uid: CPSY-FOUND-001
title: Foundations of Computational Psychiatry: Bayesian Brain Predictive Processing Computational Phenotyping Quantitative Models of Mental Illness and the Integration of Neuroscience Mathematics and Clinical Psychiatry
category: computational_psychiatry
sub_category: Foundations
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [computational-psychiatry, Bayesian-brain, predictive-processing, computational-phenotyping, quantitative-psychiatry, RDoC, precision-psychiatry, computational-biomarkers, transdiagnostic, Huys-computational-psychiatry, Montague-computational-psychiatry, mathematical-psychiatry, reinforcement-learning-psychiatry, generative-models-psychiatry]
citations:
  - author: Montague P.R.
    year: 2012
    context: Computational psychiatry. Trends in Cognitive Sciences. Founding framework quantitative models mental illness.
  - author: Huys Q.J.M.
    year: 2016
    context: Computational psychiatry as a bridge from neuroscience to clinical applications. Nature Neuroscience.
  - author: Insel T.R.
    year: 2010
    context: Research Domain Criteria (RDoC): toward a new classification framework for research on mental disorders. American Journal of Psychiatry.
---

# Foundations of Computational Psychiatry

## Abstract

Computational psychiatry is a discipline that applies mathematical models — drawn from Bayesian inference, reinforcement learning, dynamical systems, and information theory — to understand, predict, and treat mental illness. Rather than categorizing disorders by symptom clusters alone (as in DSM), computational psychiatry seeks the generative mechanisms underlying psychiatric conditions: the specific parameters of neural computation that are altered in depression, anxiety, psychosis, addiction, and trauma. The field was formally named by Read Montague in 2012, though its roots trace to earlier work on dopamine and reward prediction error by Schultz, Dayan, and Montague in the 1990s. The Research Domain Criteria (RDoC) framework, proposed by the NIMH, aligns with computational psychiatry by organizing mental disorders around quantifiable neural circuits and computational processes rather than syndromal categories. Computational phenotyping — using smartphone sensors, digital behavior, and wearable data to construct objective psychiatric biomarkers — extends the framework to real-world continuous monitoring. In TCM terms, computational psychiatry attempts to formalize the dynamics of Shen — the Heart-Mind — as a mathematical system that can be measured, modeled, and therapeutically influenced.

## The Computational Turn in Psychiatry

- Historical context: psychiatry long lacked quantitative biological markers - diagnosis by symptoms only
- Neuroscience gap: neuroimaging and genetics have not yet transformed clinical practice
- Computational bridge: mathematical models provide mechanistic link between brain and behavior
- Translational: same models apply from single neuron to whole-brain to clinical symptom level
- Precision psychiatry: individual computational parameters predict treatment response
- Transdiagnostic: computational mechanisms cut across DSM categories - shared parameters
- RDoC: NIMH framework - organize research around circuits and computations not diagnoses

## Core Mathematical Frameworks

- Bayesian inference: brain as probabilistic inference machine - beliefs as probability distributions
- Reinforcement learning: learning from reward and punishment - value functions and prediction error
- Dynamical systems: mental states as trajectories in high-dimensional state space
- Information theory: mutual information, entropy, complexity as neural and psychiatric metrics
- Active inference: unified framework combining perception, action, and learning under free energy
- Network models: brain as complex network - graph theory metrics predict psychiatric risk
- Signal detection theory: perceptual thresholds and bias - anxiety as altered detection criterion

## Computational Phenotyping

- Definition: using passive digital data to construct objective behavioral and psychiatric phenotypes
- Smartphone sensing: GPS mobility, call patterns, screen time, typing dynamics, accelerometer
- Wearables: HRV, sleep architecture, skin conductance, activity - continuous physiological stream
- Social media: linguistic analysis of posts predicts depression, bipolar, psychosis onset
- Speech: prosody, speech rate, pause patterns predict mood state and psychosis
- Passive vs active: passive sensing more ecologically valid than clinic-based assessment
- Guardian Interface: HRV, respiration, skin conductance channels already collecting phenotype data

## Key Models by Disorder

- Depression: reduced reward prediction error - blunted dopamine response to positive outcomes
- Anxiety: inflated prediction error for threat - prior belief that world is dangerous
- Schizophrenia: aberrant salience - prediction errors attached to irrelevant stimuli
- OCD: model-based vs model-free imbalance - habit system overrides goal-directed control
- PTSD: fear memory as overprecise prediction - extinction as Bayesian belief update failure
- Addiction: temporal discounting distortion - immediate reward hypervalued vs delayed
- Autism: reduced sensory prediction - over-reliance on bottom-up sensory input

## Clinical Applications

- Treatment selection: computational parameters predict who responds to which treatment
- Early warning: computational phenotyping detects prodromal symptoms before clinical threshold
- Mechanism-targeted therapy: treat the specific computational parameter that is altered
- Drug development: computational models guide target identification and trial design
- Closed-loop: real-time computational state estimation drives adaptive intervention
- Guardian Interface potential: real-time computational phenotyping via telemetry channels

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: CPSY-BAYES-001, CPSY-PHENO-001, CPSY-MOOD-001, CPSY-EEG-001, CPSY-TCM-001
- Related systems: SomaticGraph, TelemetryManager

---
uid: COMPNEURO-PREDCODE-001
title: Predictive Coding and Active Inference: Hierarchical Predictive Processing, Prediction Error Minimization, Active Inference Framework, Interoceptive Predictive Coding, and the Shen Anticipatory World Model Correspondence
category: Computational Neuroscience
sub_category: Predictive Coding
source_type: Peer Reviewed Research
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [Heart, Governing, Triple Burner, Kidney, Liver]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [predictive-coding-active-inference, hierarchical-predictive-processing, prediction-error-minimization, active-inference-framework, interoceptive-predictive-coding, Shen-anticipatory-world-model, Karl-Friston-active-inference, Andy-Clark-predictive-mind, precision-weighting-attention, allostasis-predictive-coding]
citations:
  - author: Friston K.
    year: 2010
    context: The free-energy principle a unified brain theory predictive coding active inference hierarchical predictive processing prediction error minimization active inference framework and interoceptive predictive coding.
  - author: Clark A.
    year: 2016
    context: Surfing Uncertainty Prediction Action and the Embodied Mind Shen anticipatory world model correspondence Karl Friston active inference Andy Clark predictive mind precision weighting attention and allostasis predictive coding.
---

# Predictive Coding and Active Inference

## Abstract

Predictive coding and active inference — the influential computational neuroscience framework proposing that the brain is fundamentally a prediction machine that continuously generates top-down predictions about incoming sensory signals and updates its internal models based on bottom-up prediction errors, with active inference extending this framework to encompass action as another means of minimizing prediction error by changing the sensory world to match predictions rather than updating predictions to match the world — provides a unified account of perception, action, attention, learning, and emotion within a single mathematical framework grounded in variational free energy minimization. Hierarchical predictive processing — the architectural implementation of predictive coding as a hierarchy of cortical areas in which higher areas send top-down predictions to lower areas and receive bottom-up prediction error signals, with the primary sensory cortices at the bottom of the hierarchy receiving the raw sensory input and passing prediction errors upward, the higher association areas maintaining longer timescale predictions about the causes of sensory streams, and the prefrontal cortex maintaining the highest-level predictions about the agent's context, goals, and identity. Prediction error minimization — the fundamental computational principle proposed by Helmholtz and formalized by Karl Friston in which the brain minimizes surprise or free energy by either updating its generative model to better explain sensory input through perceptual inference, or by taking actions that sample sensory data confirming prior predictions through active inference, with attention implemented as precision weighting that amplifies prediction errors from reliable sensory channels and suppresses those from unreliable channels. Interoceptive predictive coding — the extension of predictive coding to interoceptive signals from the body's internal milieu, with the insular cortex generating predictions about the body's physiological state that are updated by visceral afferent prediction errors, providing the computational basis for the subjective feeling of emotion as the brain's prediction of the body's current physiological state, with allostatic regulation understood as predictive control that acts to prevent predicted physiological deviations before they occur. The Shen anticipatory world model correspondence in TCM describes the classical understanding that the Heart governing Shen maintains the mental model of the world and the self that organizes perception and guides purposeful action — a correspondence with the predictive brain's generative model of the world maintained and updated by the highest cortical levels to guide adaptive behavior.

## Hierarchical Architecture

- Lower cortex: primary sensory areas — receive raw input send prediction errors up.
- Higher cortex: association areas — send predictions down receive errors up.
- Prefrontal: highest predictions — context goals identity self-model.
- Timescales: lower areas fast higher areas slow — nested temporal predictions.

## Prediction Error Signals

- Superficial pyramidal cells: send prediction errors upward — bottom-up.
- Deep pyramidal cells: send predictions downward — top-down.
- Dopamine: encodes reward prediction error — updates value predictions.
- Norepinephrine: encodes precision of prediction errors — modulates learning rate.

## Active Inference

- Perception: update model to match world — minimize prediction error by changing beliefs.
- Action: change world to match model — minimize prediction error by acting.
- Habits: strong priors on action outcomes — low-level reflexive active inference.
- Exploration: epistemic value of information — actively seek surprising informative states.

## TCM Correspondence

- Shen maintaining anticipatory world model corresponds to the brain's hierarchical generative model of the world and self.
- Heart governing Shen clarity corresponds to the accurate calibrated generative model minimizing prediction error.
- Liver governing smooth flow corresponds to the smooth updating of predictions by prediction errors without over or under-correction.
- Kidney governing deep constitutional priors corresponds to the deepest slowest-changing priors in the predictive hierarchy.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: COMPNEURO-FREEENERGY-001, COMPNEURO-BAYESIAN-001, COMPNEURO-COMPPSYCH-001, TCM-HEART-001, SOMAT-INTERO-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
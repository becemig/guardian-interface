---
uid: COMPNEURO-REINFORCE-001
title: Reinforcement Learning Neuroscience: Temporal Difference Learning, Dopamine as Prediction Error Signal, Model-Based vs Model-Free Learning, Habit and Goal-Directed Systems, and the Virtue Cultivation Reward Shaping Correspondence
category: Computational Neuroscience
sub_category: Reinforcement Learning
source_type: Peer Reviewed Research
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [Heart, Liver, Kidney, Triple Burner, Governing]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [reinforcement-learning-neuroscience, temporal-difference-learning, dopamine-prediction-error, model-based-model-free, habit-goal-directed, Virtue-Cultivation-reward-shaping, Wolfram-Schultz-dopamine, Q-learning-basal-ganglia, orbitofrontal-value, striatum-habit-formation]
citations:
  - author: Schultz W. et al.
    year: 1997
    context: A neural substrate of prediction and reward reinforcement learning neuroscience temporal difference learning dopamine as prediction error signal model-based vs model-free learning and habit and goal-directed systems.
  - author: Daw N.D. et al.
    year: 2011
    context: Model-based influences on humans choices and striatal prediction errors Virtue Cultivation reward shaping correspondence Wolfram Schultz dopamine Q-learning basal ganglia orbitofrontal value and striatum habit formation.
---

# Reinforcement Learning Neuroscience

## Abstract

Reinforcement learning neuroscience — the application of computational reinforcement learning theory to understanding how the brain learns to predict rewards and select actions that maximize long-term reward, with the landmark discovery by Wolfram Schultz that dopamine neurons in the ventral tegmental area and substantia nigra fire in precise correspondence with temporal difference prediction error signals — firing to unexpected rewards, ceasing to fire to predicted rewards, and showing depression responses to omission of predicted rewards — establishing dopamine as the neural currency of reward prediction error that drives learning throughout the basal ganglia and prefrontal cortex. Temporal difference learning — the computational framework in which the value of states and actions is learned through the discrepancy between predicted and actual rewards temporally integrated over future time, with the TD error signal at each time step providing the learning signal for updating value estimates, implemented neurally through the phasic dopamine signal of VTA and SNc neurons that is broadcast to the striatum and prefrontal cortex to update value representations encoded in synaptic weights. Model-based versus model-free reinforcement learning — the critical distinction between model-free learning that caches cached action values learned directly from reward experience without a world model, implemented through the dorsolateral striatum and habitual behavior, and model-based learning that uses an internal model of the world to plan and simulate future outcomes before selecting actions, implemented through the prefrontal cortex, hippocampus, and dorsomedial striatum and enabling flexible goal-directed behavior that transfers immediately to novel situations. Habit and goal-directed systems — the competition and cooperation between the stimulus-response habit system of the dorsolateral striatum that selects actions based on cached values regardless of current outcome value, and the goal-directed system of the prefrontal cortex and dorsomedial striatum that selects actions based on current outcome value and the causal action-outcome contingency, with stress, cognitive load, and extensive practice all shifting the balance toward habitual control. The Virtue Cultivation reward shaping correspondence in TCM describes the classical Confucian and Daoist understanding that virtue cultivation involves training the motivational system to find genuine reward in virtuous actions — a correspondence with the reinforcement learning understanding that the reward prediction error system can be shaped through practice and intention to find reward in the intrinsic qualities of ethical and cultivated action.

## Temporal Difference Learning

- TD error: actual reward plus discounted future value minus predicted value.
- Positive TD error: reward better than predicted — dopamine burst — strengthen action.
- Negative TD error: reward worse than predicted — dopamine dip — weaken action.
- Zero TD error: reward exactly as predicted — no dopamine change — no learning.

## Dopamine as TD Error

- VTA SNc: phasic dopamine encodes TD prediction error.
- Timing: dopamine shifts from reward to reward-predicting cue with learning.
- Omission: dopamine dip at expected reward time if reward omitted.
- Schultz experiment: classic demonstration in primate VTA neurons.

## Model-Based vs Model-Free

- Model-free: dorsolateral striatum — cached S-R values — fast inflexible habitual.
- Model-based: prefrontal orbitofrontal dorsomedial striatum — planned flexible goal-directed.
- Arbitration: prefrontal arbitrates between systems based on reliability and computational cost.
- Stress: shifts balance toward model-free habitual control.

## TCM Correspondence

- Virtue cultivation reshaping reward toward ethical action corresponds to reinforcement learning reward shaping through intention and practice.
- Heart governing the direction of Yi intention corresponds to the goal-directed system setting action selection objectives.
- Basal ganglia habit formation corresponds to the gradual automation of virtue through repeated practice — Yi to habit.
- Kidney governing constitutional motivational drive corresponds to the baseline dopaminergic tone setting the threshold for reward learning.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: COMPNEURO-COMPPSYCH-001, COMPNEURO-PREDCODE-001, MOVSCI-MOTORLEARN-001, TCM-HEART-001, NEURO-DOPAMINE-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
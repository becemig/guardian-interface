---
uid: CPSY-MOOD-001
title: Computational Models of Mood: Reinforcement Learning Reward Prediction Error Dopamine Signaling Temporal Discounting Anhedonia Depression as Computational Disorder and Bayesian Models of Affective State
category: computational_psychiatry
sub_category: Mood Models
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [computational-mood, reinforcement-learning-depression, reward-prediction-error, dopamine-computational, anhedonia-computational, temporal-discounting, mood-dynamics, affective-computing, Bayesian-mood, Q-learning-psychiatry, model-based-model-free, effort-cost-depression, negative-bias-depression, computational-antidepressant, mood-as-belief]
citations:
  - author: Huys Q.J.M.
    year: 2013
    context: Mapping anhedonia onto reinforcement learning: a behavioural meta-analysis. Biology of Mood and Anxiety Disorders.
  - author: Schultz W.
    year: 1997
    context: A neural substrate of prediction and reward. Science. Dopamine reward prediction error.
  - author: Dayan P.
    year: 2009
    context: Serotonin, inhibition, and negative mood. PLoS Computational Biology.
---

# Computational Models of Mood

## Abstract

Mood disorders are among the most prevalent psychiatric conditions globally, yet their biological mechanisms remain incompletely understood and their treatments insufficiently precise. Computational psychiatry offers a mechanistic framework: depression and related mood disorders as disturbances in reinforcement learning parameters — specifically in the computation and weighting of reward prediction error, the neural signal encoding the difference between expected and received reward. Wolfram Schultz, Peter Dayan, and Read Montague demonstrated in the 1990s that midbrain dopamine neurons fire in precise accordance with the Rescorla-Wagner prediction error rule, establishing reinforcement learning as the brain's core reward-learning algorithm. In depression, this system is altered: reward prediction errors are blunted (anhedonia), negative prediction errors are amplified (negative bias), and the effort cost of actions is overweighted (psychomotor retardation as computational phenomenon). Mood itself — the sustained affective background — can be modeled as a Bayesian belief: a slowly-updating prior over expected future reward that shapes perception, cognition, and motivation. Antidepressants and psychotherapy work, in part, by shifting the parameters of this computational system — increasing learning rates for positive outcomes, reducing the weight of negative prediction errors, decreasing effort discounting. In TCM, mood corresponds to the quality of Shen and the harmony of the five emotions with their organ systems — a computational translation maps Liver Qi stagnation to altered reward learning and excessive effort discounting.

## Reinforcement Learning Framework

- Temporal difference learning: V(s) updated by prediction error delta = r + gamma*V(s+1) - V(s)
- Reward prediction error: delta - positive when reward exceeds expectation - negative otherwise
- Dopamine: phasic dopamine bursts encode positive delta - pauses encode negative delta
- Learning rate alpha: how quickly predictions update from new experience
- Discount factor gamma: how much future reward is valued vs immediate reward
- Model-based: uses internal world model for planning - prefrontal cortex dependent
- Model-free: habit-based cached values - striatum dependent - fast but inflexible

## Depression as Computational Disorder

- Anhedonia: blunted positive prediction error - reduced dopamine response to reward
- Negative bias: amplified negative prediction error - losses loom larger than gains
- Reduced learning rate: slower updating from positive experiences - pessimistic prior persists
- Effort discounting: effort cost steeply discounted - activities feel not worth doing
- Temporal discounting: future rewards heavily discounted - present bias and hopelessness
- Rumination: excessive model-based planning with negative world model - mental simulation of loss
- Serotonin: proposed to modulate patience and aversive prediction - 5-HT and punishment

## Mood as Bayesian Belief

- Mood as prior: current mood is a prior distribution over expected future reward
- Persistence: mood updates slowly - integrates over many reward and punishment signals
- Perceptual effect: negative mood shifts perceptual bias - ambiguous stimuli read as negative
- Cognitive effect: negative mood narrows attention - reduces cognitive flexibility
- Motivational effect: negative mood reduces expected reward - reduces action initiation
- Updating: positive experiences provide likelihood to update mood prior - if attended to
- Intervention: behavioral activation targets mood prior directly via forced positive experience

## Effort and Motivation

- Effort-based decision making: net value = reward - effort cost
- Depression: effort cost parameter elevated - same reward requires more effort than it is worth
- Physical and cognitive: effort discounting applies to both physical and mental tasks
- ACC: anterior cingulate cortex computes effort cost - hypoactive in depression
- Dopamine and effort: DA in nucleus accumbens specifically encodes effort willingness
- Exercise: physical exercise directly targets effort discounting via dopamine and BDNF
- Qigong and Tai Chi: movement with low effort cost and intrinsic reward - optimal for depression

## Affective Computing in Guardian Interface

- HRV as mood proxy: low HRV correlates with negative mood state and blunted reward response
- Skin conductance: arousal dimension of mood - low arousal in depression high in anxiety
- Respiration: slowed in depression speeded in anxiety - breath as mood biomarker
- Movement quality: MCF attunement scalar as effort proxy - high A(t) indicates effort cost
- Somatic engagement: node activation breadth across knowledge graph tracks engagement quality
- Feedback loop: Guardian Interface as behavioral activation tool - movement as antidepressant

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: CPSY-FOUND-001, CPSY-BAYES-001, CPSY-TRAUMA-001, CPSY-EEG-001, CPSY-TCM-001
- Related systems: SomaticGraph, TelemetryManager, MCFSystem
- MCF link: A(t) attunement scalar as effort-cost proxy - geodesic motion as low-effort state

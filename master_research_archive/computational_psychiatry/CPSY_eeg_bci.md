---
uid: CPSY-EEG-001
title: EEG and Brain-Computer Interface in Computational Psychiatry: Neural Oscillations Frequency Band Biomarkers Affective Computing Real-Time Shen Monitoring Neurofeedback Closed-Loop BCI and Integration with Somatic Telemetry Streams
category: computational_psychiatry
sub_category: EEG and BCI
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [EEG-psychiatry, BCI-affective, neural-oscillations, alpha-EEG, theta-EEG, gamma-EEG, neurofeedback, affective-computing, real-time-BCI, closed-loop-neurofeedback, SSVEP, P300, ERN-depression, frontal-alpha-asymmetry, gamma-meditation, Shen-EEG, HRV-EEG-integration, multimodal-biosignal]
citations:
  - author: Luck S.J.
    year: 2014
    context: An Introduction to the Event-Related Potential Technique. MIT Press.
  - author: Davidson R.J.
    year: 1998
    context: Anterior electrophysiological asymmetries, emotion, and depression. Psychophysiology. Frontal alpha asymmetry.
  - author: Lutz A.
    year: 2004
    context: Long-term meditators self-induce high-amplitude gamma synchrony during mental practice. PNAS.
---

# EEG and Brain-Computer Interface in Computational Psychiatry

## Abstract

Electroencephalography provides millisecond-resolution access to the electrical dynamics of the brain — the only non-invasive modality capable of resolving neural oscillations in real time. In computational psychiatry, EEG serves as a direct readout of the prediction-error and precision-weighting processes that underlie mental health: gamma oscillations index precision weighting of sensory signals, alpha oscillations reflect top-down inhibitory control, theta oscillations track working memory and emotional processing, and frontal alpha asymmetry encodes motivational state and approach-withdrawal balance. Neurofeedback — operant conditioning of EEG signals — closes the loop between neural state and conscious intention, allowing practitioners to learn voluntary control over their own oscillatory dynamics. Affective computing extends this into real-world monitoring: wearable EEG headsets combined with multimodal physiological streams (HRV, skin conductance, respiration) enable continuous psychiatric state estimation outside the laboratory. The Guardian Interface project sits precisely at this intersection: as a somatic telemetry platform already capturing HRV, respiration, and skin conductance, the addition of an EEG channel would complete a multimodal biosignal stack capable of real-time computational phenotyping, Shen-state monitoring, and closed-loop neurofeedback guidance within the Godot environment. In TCM terms, EEG oscillations are measurable correlates of Shen activity — the Heart-Mind expressing its computational dynamics as patterned electrical rhythms across the scalp.

## Neural Oscillation Frequency Bands

- Delta 0.5-4 Hz: deep sleep slow waves - glymphatic clearance - NREM stage 3
- Theta 4-8 Hz: hippocampal memory encoding - emotional processing - meditative absorption
- Alpha 8-12 Hz: idle cortical rhythm - top-down inhibitory control - eyes-closed relaxation
- Beta 12-30 Hz: active cognition - motor preparation - anxious rumination
- Gamma 30-100 Hz: sensory binding - precision weighting - conscious perception - meditation
- High gamma 100+ Hz: local cortical processing - interictal epileptiform - surgical mapping
- Cross-frequency: theta-gamma coupling indexes working memory load and cognitive state

## Psychiatric EEG Biomarkers

- Frontal alpha asymmetry: left-right alpha power difference - approach vs withdrawal motivation
- Depression: right frontal alpha excess - withdrawal dominant - reduced left approach drive
- Anxiety: reduced alpha globally - hyperaroused cortex - excessive beta in frontal regions
- PTSD: reduced alpha - hypervigilant baseline - reduced P100 gating to startle
- Schizophrenia: reduced gamma power and synchrony - impaired sensory binding and precision
- ADHD: excess theta relative to beta - TBR ratio elevated - under-aroused frontal cortex
- ERN: error-related negativity - enlarged in OCD and anxiety - excessive error monitoring

## Meditation EEG Signatures

- Focused attention: increased frontal theta - sustained attention network engagement
- Open monitoring: increased parietal alpha - reduced default mode suppression
- Loving-kindness: increased gamma synchrony - Lutz 2004 long-term meditator finding
- Non-dual awareness: reduced alpha lateralization - equanimous baseline
- Qigong: theta increase and alpha coherence increase - similar to focused attention
- Tai Chi: alpha power increase - meditative movement distinct from aerobic exercise
- Guardian link: MCF attunement scalar near zero correlates with meditative movement EEG

## Neurofeedback

- Principle: real-time EEG feature extracted and fed back to subject as audio-visual signal
- Operant learning: subject learns to shift oscillatory state toward target through feedback
- Alpha-theta: deep relaxation protocol - theta rise with alpha support - trauma and addiction
- SMR training: sensorimotor rhythm 12-15 Hz - attention and impulse control - ADHD
- Gamma training: precision enhancement - cognitive enhancement protocols
- LORETA: source-localized neurofeedback - train specific brain regions not scalp signals
- Closed-loop Guardian: EEG neurofeedback within Godot environment - Shen training

## Affective Computing and Wearable BCI

- Wearable EEG: Muse, Emotiv, OpenBCI - dry electrode headsets for daily wear
- Emotion classification: valence and arousal from EEG features - SVM and deep learning
- Multimodal fusion: EEG plus HRV plus skin conductance - improved classification accuracy
- Real-time pipeline: feature extraction at 4 Hz update rate - compatible with 60 Hz Guardian
- Confounds: motion artifact, muscle artifact - ICA and adaptive filtering for removal
- Guardian integration: EEG as fifth telemetry channel alongside hrv, respiration, skin conductance
- Shen monitoring: frontal theta and alpha asymmetry as real-time Shen state readout

## Guardian Interface EEG Architecture

- Hardware: OpenBCI Cyton 8-channel or Muse 2 4-channel as entry point
- Python bridge: MNE-Python for EEG processing - brainflow for hardware abstraction
- Features: band power, frontal asymmetry, theta-gamma coupling, alpha coherence
- Godot signal: EEGFrame alongside MCFFrame and TelemetryFrame - unified somatic stack
- Node activation: EEG features trigger CPSY-EEG-001 and related computational nodes
- Visualization: brain topography overlay in Guardian Interface - real-time scalp map
- Neurofeedback: audio tone or haptic pulse when frontal theta exceeds threshold

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Proposed channel: eeg_frontal_asymmetry, eeg_theta_power, eeg_alpha_coherence
- Related nodes: CPSY-FOUND-001, CPSY-BAYES-001, CPSY-TCM-001, CONS-NEURAL-001
- Related systems: TelemetryManager, SomaticGraph, HapticController, MCFSystem

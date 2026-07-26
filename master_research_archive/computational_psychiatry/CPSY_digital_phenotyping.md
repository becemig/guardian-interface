---
uid: CPSY-PHENO-001
title: Digital Phenotyping and Computational Biomarkers: Passive Smartphone Sensing Wearable Physiological Streams Ecological Momentary Assessment Psychiatric Biomarker Discovery and Real-World Continuous Monitoring of Mental Health
category: computational_psychiatry
sub_category: Digital Phenotyping
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [digital-phenotyping, computational-biomarkers, smartphone-sensing, ecological-momentary-assessment, passive-sensing, wearables-psychiatry, HRV-psychiatry, speech-biomarkers, GPS-mobility, linguistic-biomarkers, just-in-time-adaptive-intervention, precision-psychiatry, mHealth, accelerometry-psychiatry, sleep-psychiatry-biomarker]
citations:
  - author: Onnela J.P.
    year: 2016
    context: Harnessing smartphone-based digital phenotyping to enhance behavioral and mental health research. Neuropsychopharmacology.
  - author: Torous J.
    year: 2018
    context: Smartphones, sensors, and machine learning to advance real-time prediction and interventions for suicide prevention. Crisis.
  - author: Insel T.R.
    year: 2017
    context: Digital phenotyping: technology for a new science of behavior. JAMA.
---

# Digital Phenotyping and Computational Biomarkers

## Abstract

Digital phenotyping — coined by Jukka-Pekka Onnela at Harvard — is the moment-by-moment quantification of the individual-level human phenotype using data from personal digital devices. It represents a fundamental shift in psychiatric assessment: away from retrospective self-report gathered in clinic visits, toward continuous objective behavioral data streams captured in the context of daily life. Smartphones passively record GPS mobility patterns, call and text frequency, screen time, typing dynamics, and accelerometer-based activity without requiring active user input. Wearable devices add physiological depth: HRV, respiration rate, skin conductance, sleep architecture, and movement quality. Speech and language biomarkers extracted from voice calls and transcripts carry diagnostic signal for depression, mania, psychosis, and cognitive decline. Together these streams constitute a digital behavioral fingerprint whose deviations from personal baseline predict psychiatric state with clinical-grade accuracy. Just-in-time adaptive interventions (JITAI) close the loop by triggering personalized therapeutic micro-interventions precisely when and where the person needs them. The Guardian Interface project already captures four of these streams — HRV, respiration rate, skin conductance, and somatic confidence — positioning it as a native digital phenotyping platform when connected to the computational psychiatry node layer.

## Passive Sensing Modalities

- GPS mobility: total distance, radius of gyration, number of unique locations, time at home
- GPS depression signal: reduced mobility, fewer locations, more time at home in depressive episodes
- Call and text patterns: social network size, communication frequency, response latency
- Screen time: total usage, nocturnal use, app category distribution
- Typing dynamics: keystroke speed, error rate, pause patterns - psychomotor slowing in depression
- Accelerometry: step count, gait regularity, postural sway, activity fragmentation
- Ambient audio: background noise classification - social context without content recording

## Physiological Biomarker Streams

- HRV: heart rate variability - primary autonomic nervous system biomarker
- HRV and depression: reduced HRV in major depression - blunted vagal tone
- HRV and anxiety: reduced HRV - sympathetic dominance - reduced regulatory flexibility
- Respiration rate: elevated in anxiety and panic - slowed in deep meditation states
- Skin conductance: electrodermal activity - sympathetic arousal marker - stress and emotional salience
- Sleep architecture: reduced slow-wave sleep in depression - REM disruption in PTSD and anxiety
- Actigraphy: wrist movement as sleep-wake proxy - circadian rhythm disruption in bipolar

## Speech and Language Biomarkers

- Prosody: pitch, rate, energy - slowed flat speech in depression, pressured in mania
- Pause patterns: increased pause duration in depression, psychomotor retardation
- Semantic coherence: reduced in psychosis - tangential associations detectable by NLP
- Sentiment: linguistic sentiment analysis of social media predicts depressive episodes
- Vocabulary diversity: type-token ratio reduced in depression and cognitive decline
- First-person singular: I-word usage elevated in depression - excessive self-focus
- Voice tremor: fine motor control degradation - biomarker for anxiety and Parkinson onset

## Just-in-Time Adaptive Interventions

- JITAI: deliver right intervention at right time and place in real world
- Trigger detection: real-time anomaly detection on digital phenotype streams
- Intervention library: micro-interventions matched to detected state - breathing, grounding, movement
- Personalization: person-specific baselines and thresholds - idiographic not nomothetic
- Closed-loop: phenotype stream drives intervention selection drives new phenotype measurement
- Guardian potential: Godot interface triggers somatic interventions from telemetry anomalies
- Privacy: all processing on-device - no raw behavioral data transmitted

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, skin_conductance, confidence
- Related nodes: CPSY-FOUND-001, CPSY-EEG-001, CPSY-MOOD-001, CPSY-TCM-001
- Related systems: TelemetryManager, SomaticGraph, HapticController

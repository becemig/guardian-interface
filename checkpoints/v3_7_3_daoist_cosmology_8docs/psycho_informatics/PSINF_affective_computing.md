---
uid: PSINF-AFFECT-001
title: Affective Computing: Emotion Recognition, Multimodal Affect Detection, Valence-Arousal Space, Sentiment Analysis, and Machine Empathy
category: Psycho-Informatics
sub_category: Affective Computing
source_type: Peer Reviewed Research
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [Heart, Liver, Triple Burner, Spleen, Lung]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [affective-computing, emotion-recognition, multimodal-affect-detection, valence-arousal-space, sentiment-analysis, machine-empathy, Rosalind-Picard, facial-action-coding, speech-emotion-recognition, physiological-emotion-detection]
citations:
  - author: Picard R.W.
    year: 1997
    context: Affective Computing affective computing emotion recognition multimodal affect detection valence arousal space and machine empathy.
  - author: Calvo R.A. and D-Mello S.
    year: 2010
    context: Affect detection an interdisciplinary review of models methods and their applications multimodal affect detection sentiment analysis and machine empathy.
---

# Affective Computing

## Abstract

Affective computing — coined by Rosalind Picard at MIT Media Lab in her 1997 book of the same name — is the field of computing that studies and develops systems that can recognize, interpret, process, and simulate human affects and emotions. The motivation is both scientific — understanding the role of emotion in cognition, decision-making, and health — and engineering — creating systems that interact with humans in emotionally intelligent ways, adapting their behavior to the emotional state of the user. Emotion recognition draws on multiple modalities: facial expression analysis using the Facial Action Coding System FACS which decomposes facial movement into action units that combine to produce recognizable emotional expressions; speech emotion recognition analyzing acoustic features including pitch, energy, speaking rate, and voice quality that carry emotional information independent of linguistic content; physiological emotion detection using EEG, galvanic skin response, heart rate variability, and respiration patterns as direct physiological correlates of emotional arousal and valence; and behavioral indicators including gesture, posture, gaze, and movement patterns. The dimensional model of emotion — organizing all emotional states in a two-dimensional valence-arousal space with valence ranging from negative to positive and arousal ranging from calm to excited — provides the most computationally tractable framework for emotion representation, capturing the primary dimensions of emotional variation while remaining computationally tractable. Sentiment analysis — the natural language processing task of detecting the emotional valence and intensity of text — extends affective computing into the domain of written language, enabling the analysis of social media, clinical notes, and patient-reported outcomes for emotional content at scale. The Guardian Interface confidence telemetry channel represents an implementation of affective computing principles — using HRV and respiration patterns as physiological indicators of the user's emotional-cognitive state to adapt the interface's behavior in real time.

## Multimodal Affect Detection

- Facial: action units from FACS combine to produce recognizable emotional expressions.
- Speech: pitch, energy, speaking rate, and voice quality carry emotional information.
- Physiological: EEG, GSR, HRV, and respiration are direct correlates of arousal and valence.
- Behavioral: gesture, posture, gaze, and movement patterns reflect emotional state.

## Valence-Arousal Space

- Valence axis: negative unpleasant to positive pleasant.
- Arousal axis: calm low arousal to excited high arousal.
- High arousal positive: joy, excitement, enthusiasm.
- High arousal negative: fear, anger, stress.
- Low arousal positive: contentment, calm, peace.
- Low arousal negative: sadness, depression, fatigue.

## Guardian Interface Affective Layer

- HRV as arousal indicator: high HRV correlates with calm positive affect and cognitive flexibility.
- Respiration rate as arousal indicator: slow deep respiration correlates with calm low-arousal states.
- Confidence telemetry as valence proxy: high confidence correlates with positive engaged affect.
- Real-time affective state detection enables adaptive interface response to user emotional state.

## TCM Correspondence

- Multimodal affect detection corresponds to the TCM diagnostic reading of the five emotional expressions through face, voice, and body.
- Valence-arousal space corresponds to the TCM seven emotions model mapping emotional states to organ networks.
- Machine empathy corresponds to the TCM practitioner's cultivation of resonance with the patient's Shen state.
- Affective computing physiological correlates correspond to TCM pulse qualities as physiological emotion indicators.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: PSINF-DIGPHEN-001, PSINF-HRV-001, NEURO-INTRO-001, PSY-CONSC-001, PSINF-EEG-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
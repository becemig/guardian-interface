---
uid: PSINF-HRV-001
title: HRV Biomarkers: Autonomic Nervous System Metrics, RMSSD SDNN LF HF Ratio, Vagal Tone, Emotion Regulation Capacity, and Polyvagal Biofeedback
category: Psycho-Informatics
sub_category: Physiological Biomarkers
source_type: Peer Reviewed Research
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [Heart, Lung, Kidney, Triple Burner, Spleen]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [HRV-biomarkers, autonomic-nervous-system-metrics, RMSSD, SDNN, LF-HF-ratio, vagal-tone, emotion-regulation-capacity, polyvagal-biofeedback, heart-rate-variability-training, RSA-respiratory-sinus-arrhythmia]
citations:
  - author: Task Force of the European Society of Cardiology
    year: 1996
    context: Heart rate variability standards of measurement physiological interpretation and clinical use HRV biomarkers RMSSD SDNN LF HF ratio and vagal tone.
  - author: Porges S.W.
    year: 2011
    context: The Polyvagal Theory Neurophysiological Foundations of Emotions Attachment Communication Self-Regulation polyvagal biofeedback emotion regulation capacity and HRV.
---

# HRV Biomarkers

## Abstract

Heart rate variability HRV — the beat-to-beat variation in the time interval between successive heartbeats — is the most widely validated non-invasive biomarker of autonomic nervous system function, providing a window into the dynamic balance between sympathetic and parasympathetic regulation of cardiac function that reflects the organism's capacity to flexibly respond to environmental demands. HRV is not noise in the cardiac signal but meaningful biological information: high HRV reflects a flexible, responsive autonomic nervous system capable of rapidly shifting between states as circumstances demand, while low HRV reflects a rigid, less adaptive system associated with poor health outcomes, reduced stress resilience, and diminished emotional regulation capacity. The primary time-domain HRV metrics are RMSSD — the root mean square of successive RR interval differences, reflecting primarily parasympathetic vagal influence on heart rate — and SDNN — the standard deviation of all RR intervals, reflecting overall autonomic variability including both sympathetic and parasympathetic contributions. The frequency-domain metrics divide HRV into low-frequency LF power 0.04-0.15 Hz reflecting mixed sympathetic-parasympathetic influence, high-frequency HF power 0.15-0.4 Hz reflecting primarily parasympathetic vagal influence synchronous with respiration as respiratory sinus arrhythmia RSA, and the LF/HF ratio often used as an index of sympathovagal balance though its interpretation is contested. Vagal tone — indexed by RMSSD and HF power — is the primary predictor of emotion regulation capacity in the neurovisceral integration model: individuals with higher vagal tone show greater flexibility in emotional response, faster recovery from stress, and more effective top-down prefrontal regulation of limbic reactivity. Polyvagal biofeedback — resonance frequency breathing at 0.1 Hz approximately 6 breaths per minute — maximally amplifies RSA and increases HRV through the synchronization of respiration with the natural oscillation frequency of the baroreflex system, providing a trainable intervention that increases vagal tone and emotion regulation capacity. The Guardian Interface HRV telemetry channel directly implements these biomarker principles, providing real-time autonomic state monitoring.

## Time Domain Metrics

- RMSSD: root mean square successive differences — primary parasympathetic index, most robust short-term metric.
- SDNN: standard deviation of RR intervals — overall autonomic variability, requires 24-hour recording for full validity.
- pNN50: percentage of successive RR differences greater than 50ms — parasympathetic index, correlates with RMSSD.
- MeanRR: average RR interval — inversely related to mean heart rate.

## Frequency Domain Metrics

- HF power 0.15-0.4 Hz: vagal RSA — primary parasympathetic index synchronized with respiration.
- LF power 0.04-0.15 Hz: mixed sympathetic-parasympathetic — baroreflex and vasomotor oscillations.
- VLF power below 0.04 Hz: thermoregulation, renin-angiotensin, peripheral vascular tone.
- LF/HF ratio: contested sympathovagal balance index — interpret cautiously.

## Resonance Frequency Breathing

- 0.1 Hz breathing rate approximately 6 breaths per minute maximally amplifies RSA.
- Synchronizes respiration with baroreflex natural oscillation frequency — resonance condition.
- Produces maximum HRV amplitude and maximum baroreflex gain — highly efficient training.
- 20 minutes daily for 4-8 weeks produces lasting increases in resting vagal tone.

## TCM Correspondence

- HRV as autonomic flexibility index corresponds to the TCM understanding that Wei Qi adaptability is the primary health indicator.
- RMSSD vagal tone corresponds to the TCM Heart-Lung axis governing the Wei Qi defensive circulation rhythm.
- Resonance frequency breathing corresponds to the TCM Six Healing Sounds and regulated breathing cultivation practices.
- High HRV as emotion regulation capacity corresponds to the TCM cultivated Shen stability that does not react to external perturbation.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: PSINF-EEG-001, PSINF-AFFECT-001, NEURO-PNI-001, NEURO-INTRO-001, PSINF-DIGPHEN-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
---
uid: TCM-PULSE-001
title: Pulse Diagnosis: 28 Classical Pulses, Three Positions, Nine Depths, and Cardiovascular Biomarkers
category: Traditional Chinese Medicine
sub_category: Diagnosis
source_type: Traditional Framework
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [Heart, Lung, Liver, Kidney]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [pulse-diagnosis, 28-pulses, three-positions, cun-guan-chi, nine-depths, mai-jing, li-shi-zhen, floating-sinking, rapid-slow, wiry-slippery]
citations:
  - author: Wang S.H.
    year: 280
    context: Mai Jing Pulse Classic twenty-eight pulse qualities three positions and pulse diagnosis methodology.
  - author: Li S.Z.
    year: 1564
    context: Bin Hu Mai Xue Lake Guest Pulse Studies 27 pulse qualities and clinical correspondence.
---

# Pulse Diagnosis

## Abstract

Pulse diagnosis Mai Zhen is one of the four examinations of TCM alongside looking, asking, and listening-smelling and is considered the most sophisticated and information-dense diagnostic technique in classical Chinese medicine. Wang Shuhe's Mai Jing Pulse Classic compiled in 280 CE systematized 24 pulse qualities. Li Shizhen expanded this to 27 in his Bin Hu Mai Xue and subsequent texts describe 28 classical pulse qualities. The radial pulse is palpated at three positions on each wrist — Cun inch, Guan bar, and Chi cubit — each corresponding to specific organ systems. Cun corresponds to the upper Jiao Heart and Lung on the respective wrists. Guan corresponds to the middle Jiao Liver-Gallbladder on the left and Spleen-Stomach on the right. Chi corresponds to the lower Jiao Kidney on both wrists. Each position is palpated at three depths — superficial, middle, and deep — giving nine positions per wrist and eighteen total. Modern research documents that classical pulse qualities correspond to measurable cardiovascular parameters — the floating pulse correlates with reduced peripheral vascular resistance, the wiry pulse with increased arterial stiffness, the slippery pulse with increased cardiac output, and the rapid pulse with tachycardia.

## 28 Classical Pulses

- Floating Fu pulse felt at the superficial level corresponds to exterior patterns and Lung conditions.
- Sinking Chen pulse felt only at the deep level corresponds to interior patterns and Kidney conditions.
- Rapid Shu pulse above 90 beats per minute corresponds to Heat patterns and Yin deficiency.
- Slow Chi pulse below 60 beats per minute corresponds to Cold patterns and Yang deficiency.
- Wiry Xian pulse like a taut bowstring corresponds to Liver patterns and pain conditions.
- Slippery Hua pulse like pearls rolling corresponds to Phlegm, Dampness, and pregnancy.

## Three Positions

- The Cun position proximal to the wrist crease corresponds to the upper Jiao and Heart-Lung domain.
- The Guan position at the styloid process corresponds to the middle Jiao Liver and Spleen domains.
- The Chi position distal to the styloid corresponds to the lower Jiao Kidney and constitutional domain.
- Left wrist reads Heart, Liver, and Kidney Yin while right wrist reads Lung, Spleen, and Kidney Yang.

## Cardiovascular Correlates

- Floating pulse corresponds to reduced peripheral vascular resistance and vasodilation states.
- Wiry pulse corresponds to increased arterial wall stiffness measurable by pulse wave velocity.
- Slippery pulse corresponds to increased stroke volume and cardiac output states.
- Pulse width and strength correlate with blood pressure and cardiac contractility parameters.

## TCM Correspondence

- Pulse diagnosis provides direct access to the Qi and Blood state of each organ system simultaneously.
- The three depths correspond to Wei Qi at the surface, Ying Qi at middle depth, and Yuan Qi at the bone level.
- Pulse changes during treatment provide real-time feedback on the effectiveness of acupuncture intervention.
- Guardian Interface telemetry HRV data corresponds to the time-domain pulse qualities of rate and rhythm.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: TCM-ACU-001, TCM-HKAXIS-001, TCM-KDYANG-001, NEURO-AUTOHRT-001, TCM-BLSTA-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
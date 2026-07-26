---
uid: NEURO-SALIENT-001
title: Salience Network: Anterior Insula, dACC, Interoceptive Prediction, and Shen Orientation
category: Neurophysiology
sub_category: Network Neuroscience
source_type: Peer Reviewed Research
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 216]
target_meridians: [Heart, Lung, Governing, Conception]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [salience-network, anterior-insula, dACC, interoceptive-prediction, switching, Menon, SN-DMN-anticorrelation, allostasis, uncertainty-detection, shen-orientation]
citations:
  - author: Menon V. and Uddin L.Q.
    year: 2010
    context: Saliency switching attention and control a network model of insula function salience network.
  - author: Seeley W.W. et al.
    year: 2007
    context: Dissociable intrinsic connectivity networks for salience processing and executive control salience network.
---

# Salience Network

## Abstract

The salience network SN is a large-scale brain network comprising the anterior insula AI and dorsal anterior cingulate cortex dACC as its core nodes along with the amygdala, hypothalamus, and brainstem — a network specialized for detecting, orienting to, and filtering the most biologically and emotionally relevant stimuli from the continuous stream of sensory experience. Vinod Menon proposed that the SN serves a switching function between the default mode network and the central executive network — detecting salient events and switching from self-referential DMN processing to externally directed CEN processing. William Seeley's work established the SN as a distinct intrinsic network with characteristic functional connectivity, and identified it as the network most affected in behavioral variant frontotemporal dementia. The anterior insula provides interoceptive prediction — continuously generating predictions about the body's internal state and detecting mismatches that require updating. The dACC monitors for behavioral and cognitive conflicts requiring control resource allocation. Together they implement a continuous relevance filter — what matters to the organism right now given its current goals and internal state. The SN is the first large-scale network to respond to unexpected or threatening stimuli, initiating the cascade of downstream responses in the autonomic, endocrine, and behavioral systems. Anxious individuals show SN hyperreactivity — detecting threat in ambiguous stimuli and over-switching to alert defensive processing. Shen orientation in TCM — the Heart-Mind's rapid turning of awareness toward what is most significant in the present moment — corresponds to the salience network's detection and orientation function.

## Switching Function

- The SN detects salient events and switches brain-wide processing from DMN to CEN task-positive mode.
- This switching is essential for rapid adaptive response to unexpected or threatening events.
- SN-DMN anticorrelation ensures that self-referential and externally directed processing do not compete.
- Impaired SN switching in schizophrenia produces confusion between internally generated and external events.

## Interoceptive Prediction

- Anterior insula generates continuous predictions about the body's internal state — allostatic predictions.
- Mismatches between predictions and actual interoceptive signals generate salience signals requiring attention.
- Strong prediction errors from the body produce the urgent felt sense of something important happening.
- Chronic prediction errors from the body produce the persistent salience of anxiety and somatic preoccupation.

## Threat Detection

- The SN receives fast subcortical threat information from the amygdala within 100ms of stimulus onset.
- This rapid signal initiates autonomic, hormonal, and behavioral defensive responses before cortical processing.
- SN hyperreactivity in anxiety produces over-detection of threat signals amplifying defensive responses.
- Mindfulness reduces SN reactivity through top-down prefrontal modulation of anterior insula and dACC.

## TCM Correspondence

- Shen orientation as rapid turning of awareness toward significance corresponds to SN detection and switching.
- Heart governing alertness and appropriate response to the significant corresponds to SN salience detection.
- Lung Po governing the body boundary and threat detection corresponds to AI interoceptive prediction mismatch.
- Pericardium regulating what reaches the Heart corresponds to SN filtering what reaches conscious awareness.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 216
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: NEURO-INSUL-001, NEURO-ACC-001, NEURO-AMYG-001, TCM-HTSHEN-001, NEURO-DMN-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
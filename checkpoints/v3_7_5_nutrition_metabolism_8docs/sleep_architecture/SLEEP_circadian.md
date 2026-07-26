---
uid: SLEEP-CIRC-001
title: Circadian Rhythm and Clock Biology: SCN CLOCK-BMAL1 PER-CRY Feedback Loop Light Entrainment Melanopsin Cortisol Awakening Response Melatonin and Social Zeitgebers
category: sleep_architecture
sub_category: Circadian Biology
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [circadian-rhythm, SCN, CLOCK, BMAL1, PER, CRY, melanopsin, ipRGC, melatonin, cortisol-awakening-response, zeitgeber, light-entrainment, chronotype, social-jetlag, peripheral-clocks, clock-genes, circadian-disruption]
citations:
  - author: Takahashi J.S.
    year: 2017
    context: Transcriptional architecture of the mammalian circadian clock. Nature Reviews Genetics.
  - author: Roenneberg T.
    year: 2012
    context: Social jetlag: misalignment of biological and social time. Chronobiology International.
  - author: Provencio I.
    year: 2000
    context: A novel human opsin in the inner retina. Journal of Neuroscience. Melanopsin ipRGC light entrainment.
---

# Circadian Rhythm and Clock Biology

## Abstract

The circadian system is a cell-autonomous timekeeping mechanism present in virtually every cell of the body, coordinated by the suprachiasmatic nucleus (SCN) master clock in the hypothalamus. The molecular clock is a transcription-translation feedback loop driven by CLOCK-BMAL1 heterodimer activation of PER and CRY genes, whose protein products feed back to inhibit CLOCK-BMAL1 with an approximately 24-hour period. Light is the primary zeitgeber (time-giver) entraining the SCN via melanopsin-expressing intrinsically photosensitive retinal ganglion cells (ipRGCs). The SCN coordinates peripheral clocks in every organ through hormonal signals (cortisol, melatonin) and autonomic output, synchronizing the entire body to a coherent temporal program. Disruption of this system through shift work, chronic blue light exposure, or social jetlag is associated with metabolic disease, immune dysfunction, and psychiatric disorders.

## SCN Master Clock

- Location: bilateral nuclei in anterior hypothalamus above the optic chiasm
- Cell count: approximately 20,000 neurons per nucleus
- Self-sustaining: SCN maintains 24-hour rhythm in constant darkness
- Output signals: melatonin via pineal, cortisol via HPA, ANS via hypothalamic projections
- Lesion: SCN destruction eliminates all circadian rhythmicity
- Transplant: fetal SCN transplant restores rhythmicity in SCN-lesioned animals

## Molecular Clock - CLOCK-BMAL1 Loop

- Positive arm: CLOCK and BMAL1 heterodimerize - activate transcription of PER1, PER2, CRY1, CRY2
- Negative arm: PER and CRY proteins accumulate, dimerize, re-enter nucleus, inhibit CLOCK-BMAL1
- Period: loop takes approximately 24 hours to complete
- Stabilization: casein kinase phosphorylates PER for gradual degradation - sets period length
- Secondary loop: REV-ERB and ROR compete for BMAL1 transcription - adds robustness
- Output: clock genes regulate thousands of downstream target genes in tissue-specific patterns

## Light Entrainment - Melanopsin ipRGCs

- ipRGCs: intrinsically photosensitive retinal ganglion cells - third photoreceptor type
- Melanopsin: opsin maximally sensitive to 480 nm blue light
- Pathway: retinohypothalamic tract directly from ipRGCs to SCN
- Morning light: phase advances clock - most powerful entraining signal
- Evening blue light: phase delays clock - mimics morning signal at wrong time
- Screen exposure: evening screen use delays melatonin onset by 1.5-3 hours
- Therapeutic: morning bright light (10,000 lux) is primary intervention for circadian disruption

## Melatonin

- Source: pineal gland - driven by SCN via superior cervical ganglion
- Onset: dim-light melatonin onset (DLMO) occurs 2 hours before sleep onset
- Function: darkness signal - tells body it is night - not directly sleep-inducing
- Light suppression: even dim indoor light suppresses melatonin by 50 percent
- Therapeutic: low dose (0.5 mg) melatonin phase-shifts clock - higher doses are sedative only

## Cortisol Awakening Response (CAR)

- Timing: cortisol rises sharply 20-30 minutes after waking - peaks at 30-45 minutes
- Magnitude: 50-100 percent increase above baseline
- Function: mobilizes energy, sharpens cognition, primes immune system for the day
- Light amplification: morning light exposure amplifies CAR
- Stress marker: blunted CAR indicates HPA exhaustion - trauma or burnout pattern
- TCM parallel: Lung Wei Qi opening at 3-5 AM, Yang Qi rising from Kidney at dawn

## Social Zeitgebers and Jetlag

- Zeitgebers: time-givers that entrain the circadian clock
- Primary: light-dark cycle
- Secondary: meal timing, exercise, social interaction, temperature
- Social jetlag: misalignment between biological clock and social schedule
- Chronotype: genetically influenced morning vs evening preference - owl vs lark
- Shift work: chronic circadian disruption increases cancer, metabolic, and cardiovascular risk

## Peripheral Clocks

- Every cell: contains functional clock machinery driven by SCN coordination
- Liver clock: governs metabolic enzyme timing - meal timing entrains independently of SCN
- Immune clock: cytokine production, NK cell activity all circadian-gated
- Gut clock: microbiome composition oscillates with circadian rhythm
- Misalignment: peripheral clocks can desynchronize from SCN with irregular schedules

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: SLEEP-NEURO-001, SLEEP-STAGES-001, SLEEP-TCM-001, TCM-WQCYC-001, TCM-YINQIAO-001, TCM-YANGQIAO-001, ECS-STRESS-001
- Related systems: SomaticGraph, TelemetryManager

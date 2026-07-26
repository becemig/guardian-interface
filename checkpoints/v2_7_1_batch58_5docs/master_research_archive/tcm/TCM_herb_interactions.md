---
uid: TCM-HERBINT-001
title: TCM Herb-Herb Interactions: Eighteen Incompatibilities, Nineteen Antagonisms, MAOI Overlap, CYP450 Inhibition, and Safety Framework
category: Traditional Chinese Medicine
sub_category: Herbal Safety
source_type: Traditional Framework
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [Liver, Heart, Kidney, Spleen]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [herb-herb-interactions, eighteen-incompatibilities, nineteen-antagonisms, MAOI-overlap, CYP450-inhibition, safety-framework, shi-ba-fan, shi-jiu-wei, berberine-MAOI, Dan-Shen-CYP450]
citations:
  - author: Bensky D. and Gamble A.
    year: 1993
    context: Chinese Herbal Medicine Materia Medica eighteen incompatibilities nineteen antagonisms and herb safety framework.
  - author: Zhou S. et al.
    year: 2003
    context: Herbal bioactivation metabolic activation and herb-drug interactions CYP450 inhibition and safety.
---

# TCM Herb-Herb Interactions

## Abstract

Classical TCM pharmacology developed extensive empirical interaction warnings over millennia of clinical observation — codified in the eighteen incompatibilities Shi Ba Fan and nineteen antagonisms Shi Jiu Wei that remain the foundational safety framework of Chinese herbal medicine. Modern pharmacological analysis has begun to elucidate the biochemical mechanisms underlying these traditional warnings, revealing that many correspond to enzyme inhibition, receptor competition, and metabolic interactions that Western pharmacology now recognizes as clinically significant drug-drug interactions. The eighteen incompatibilities list pairs of herbs that should never be combined: Wu Tou aconite with Bei Mu fritillary, Gua Lou trichosanthes, Ban Xia pinellia, Bai Lian and Bai Ji; Gan Cao licorice with Yuan Hua genkwa, Gan Sui euphorbia kansui, Da Ji euphorbia pekinensis, and Hai Zao seaweed; Li Lu veratrum with Ren Shen, Sha Shen, Dan Shen, Xuan Shen, Xi Xin, and Bai Shao. The nineteen antagonisms list herbs that reduce each other's efficacy when combined. MAOI overlap interactions represent the most clinically dangerous modern extension of these traditional warnings: Huang Lian berberine inhibits both MAO-A and MAO-B producing antidepressant activity but also creating tyramine interaction risk when combined with other herbs or foods. Dan Shen Salvia miltiorrhiza significantly inhibits CYP3A4 and CYP2C9 cytochrome P450 enzymes — the primary metabolic enzymes for many drugs — producing potentially dangerous drug level elevations when combined with pharmaceuticals metabolized by these enzymes. Gan Cao licorice inhibits 11-beta-HSD producing cortisol-like effects and potentiates many herbs by extending their metabolic half-life through CYP450 inhibition.

## Eighteen Incompatibilities

- Wu Tou group: aconite incompatible with fritillary, trichosanthes, pinellia, bletilla — mechanism involves competing alkaloid receptor profiles.
- Gan Cao group: licorice incompatible with genkwa, euphorbia, seaweed — opposing actions on fluid metabolism and potential toxicity amplification.
- Li Lu group: veratrum incompatible with ginseng, salvia, adenophora — mechanism involves competing adrenergic and cardiac effects.
- Modern analysis suggests many incompatibilities involve cytotoxicity amplification or opposing receptor actions.

## MAOI Overlap Interactions

- Huang Lian berberine inhibits MAO-A producing antidepressant activity and tyramine interaction risk.
- Combining berberine-containing herbs Huang Lian, Huang Bai, Huang Qi with tyramine-rich foods is contraindicated.
- Shi Chang Pu beta-asarone has MAOI-adjacent activity — combining with other psychoactive herbs amplifies CNS effects.
- Ma Huang ephedrine combined with any MAO-inhibiting herb creates severe hypertensive interaction risk.

## CYP450 Inhibition

- Dan Shen inhibits CYP3A4 and CYP2C9 — raises levels of warfarin, statins, immunosuppressants.
- Gan Cao inhibits multiple CYP450 enzymes extending the half-life and potency of co-administered herbs and drugs.
- Huang Lian berberine inhibits CYP2D6 affecting metabolism of many psychotropic medications.
- These interactions explain why TCM traditionally restricts concurrent use of certain herb combinations.

## TCM Correspondence

- Eighteen incompatibilities correspond to pharmacological antagonism, toxicity amplification, and opposing receptor profiles.
- Nineteen antagonisms correspond to pharmacokinetic interference reducing bioavailability of co-administered herbs.
- The traditional empirical safety framework corresponds to evidence-based herb-drug interaction pharmacology.
- Licorice as universal potentiator and interaction risk corresponds to its CYP450 inhibition extending metabolic half-life.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: TCM-PSYCHO-001, NEURO-AYAH-001, TCM-PHYTO-FOUND-001, TCM-HERBF-001, TCM-PHYTO-NEURO-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
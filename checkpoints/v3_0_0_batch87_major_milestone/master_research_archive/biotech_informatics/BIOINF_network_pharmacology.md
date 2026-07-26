---
uid: BIOINF-NETPHARM-001
title: Network Pharmacology: Herb-Target Interaction Networks, Polypharmacology, Disease Module Targeting, TCM Formula Network Analysis, and Multi-Target Therapeutic Strategy
category: Biotech-Informatics
sub_category: TCM Systems Biology Bridge
source_type: Peer Reviewed Research
status: In-Progress Draft
associated_somatic_nodes: [42, 108, 212, 215, 312]
target_meridians: [All Primary Meridians, Triple Burner, Spleen, Liver, Kidney]
telemetry_triggers: [hrv, respiration_rate, confidence]
tags: [network-pharmacology, herb-target-interaction, polypharmacology, disease-module-targeting, TCM-formula-network-analysis, multi-target-therapeutic-strategy, TCMSP-database, STRING-network, KEGG-pathway, active-compound-target-disease]
citations:
  - author: Hopkins A.L.
    year: 2008
    context: Network pharmacology the next paradigm in drug discovery herb-target interaction networks polypharmacology and multi-target therapeutic strategy.
  - author: Li S. and Zhang B.
    year: 2013
    context: Traditional Chinese medicine network pharmacology theory methodology and application TCM formula network analysis and disease module targeting.
---

# Network Pharmacology

## Abstract

Network pharmacology — the application of network science and systems biology to pharmacology — has emerged as the primary computational framework for understanding the mechanisms of action of Traditional Chinese Medicine formulas, providing a principled approach to the longstanding question of how complex herbal mixtures containing hundreds of bioactive compounds can produce coherent therapeutic effects. The central insight of network pharmacology is that disease is not a malfunction of a single protein or gene but a perturbation of a biological network, and that effective therapy requires not the maximally potent inhibition of a single target but the gentle modulation of multiple targets within the disease network to restore network homeostasis. This multi-target perspective corresponds precisely to the TCM formula design principle that combines multiple herbs targeting multiple organ networks to restore constitutional balance. Herb-target interaction networks map the bioactive compounds of each herb to their known molecular targets — proteins, receptors, enzymes, and transcription factors — using databases including TCMSP Traditional Chinese Medicine Systems Pharmacology, HERB, and SymMap that contain experimentally validated and computationally predicted herb-target interactions for thousands of TCM herbs. Disease module targeting identifies the subnetwork of the human protein interactome that is disrupted in a specific disease, then identifies which herb-target interactions overlap with the disease module — providing a mechanistic explanation for why specific herbs are effective for specific conditions. TCM formula network analysis applies this approach to complete formulas such as Liu Wei Di Huang Wan or Xiao Chai Hu Tang, mapping all bioactive compounds across all formula herbs to their targets, identifying the core molecular pathways modulated by the formula, and explaining the formula's clinical indications in molecular network terms. Polypharmacology — the deliberate design of compounds or formulas that hit multiple targets simultaneously — represents the convergence of modern drug discovery with the TCM multi-target therapeutic paradigm.

## Herb-Target Networks

- TCMSP database contains herb-compound-target data for over 500 TCM herbs.
- Active compounds identified by ADME criteria: oral bioavailability greater than 30 percent, drug-likeness greater than 0.18.
- Targets mapped to human protein interactome using STRING and UniProt databases.
- Network visualization reveals which herbs share targets and which targets are most central.

## Disease Module Targeting

- Disease modules are subnetworks of the interactome disrupted in specific diseases.
- Herb targets overlapping with disease modules predict clinical efficacy.
- Core disease module targets receive input from multiple herbs in effective formulas.
- This corresponds to the TCM principle that effective formulas address the root pattern through multiple concurrent interventions.

## Formula Analysis Example

- Liu Wei Di Huang Wan: 166 active compounds, 298 targets, enriched in AGE-RAGE, TNF, IL-17 pathways.
- Core targets include TP53, AKT1, IL6, VEGFA, TNF — central network hubs in aging and inflammation.
- Kidney-Liver nourishing clinical indication corresponds to anti-inflammatory and mitochondrial protection mechanisms.
- This molecular validation corresponds to the TCM formula indication for Kidney-Liver Yin deficiency pattern.

## TCM Correspondence

- Network pharmacology multi-target framework provides molecular validation of the TCM multi-herb formula design principle.
- Disease module targeting corresponds to the TCM pattern recognition identifying the root constitutional imbalance.
- Herb-target interaction networks correspond to the TCM materia medica knowledge of each herb's organ affinity and action.
- Polypharmacology corresponds to the TCM jun-chen-zuo-shi emperor-minister-assistant-courier formula structure.

## Guardian Interface Links

- Somatic nodes: 42, 108, 212, 215, 312
- Telemetry channels: hrv, respiration_rate, confidence
- Related nodes: BIOINF-OMICS-001, BIOINF-PROTO-001, TCM-PHYTO-FOUND-001, BIOINF-PATOMICS-001, TCM-BCGM-001
- Related systems: TelemetryManager, SomaticGraph, VisualizationManager
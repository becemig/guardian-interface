---
uid: ECS-METAB-001
title: ECS Metabolism: FAAH MAGL Biosynthesis Degradation Anandamide 2-AG Enzymatic Regulation and Lipid Signaling Control
category: endocannabinoid_system
sub_category: ECS Metabolism and Enzymology
source_type: Scientific Literature Review
status: In-Progress Draft
associated_somatic_nodes: [108, 56, 78, 42, 215]
tags: [ECS, FAAH, MAGL, anandamide, 2-AG, biosynthesis, degradation, NAPE-PLD, DAGL, endocannabinoid-metabolism, lipid-signaling, enzyme-inhibition, tonic-ECS-signaling]
citations:
  - author: Cravatt B.F.
    year: 2001
    context: Functional disassociation of the central and peripheral fatty acid amide signaling systems. Proceedings of the National Academy of Sciences.
  - author: Blankman J.L.
    year: 2013
    context: ABHD6 controls brain 2-arachidonoylglycerol levels and the activity of the endocannabinoid system. Journal of Lipid Research.
  - author: Piomelli D.
    year: 2003
    context: The molecular logic of endocannabinoid signalling. Nature Reviews Neuroscience.
---

# ECS Metabolism: Biosynthesis and Degradation

## Abstract

Endocannabinoid signaling is tightly regulated through on-demand biosynthesis and rapid enzymatic degradation, distinguishing it from classical neurotransmission where transmitters are stored in vesicles. Anandamide (AEA) and 2-arachidonoylglycerol (2-AG) are synthesized postsynaptically from membrane phospholipid precursors in response to calcium influx and receptor activation, travel retrogradely to suppress presynaptic activity, then are degraded by distinct enzymatic pathways. FAAH (fatty acid amide hydrolase) is the primary degradative enzyme for anandamide, and MAGL (monoacylglycerol lipase) for 2-AG. Pharmacological inhibition of these enzymes elevates endogenous cannabinoid tone without exogenous receptor activation, representing a key therapeutic strategy.

## Anandamide Biosynthesis

- Precursor: N-arachidonoyl phosphatidylethanolamine (NAPE) in postsynaptic membrane
- Primary enzyme: NAPE-PLD (N-acyl phosphatidylethanolamine phospholipase D)
- Alternative pathways: ABHD4-GDE1 and PLC-PTPN22 routes active when NAPE-PLD is absent
- Trigger: calcium influx and Gq-coupled receptor activation
- Synthesis is on-demand: no vesicular storage, synthesized as needed

## Anandamide Degradation — FAAH

- Enzyme: fatty acid amide hydrolase (FAAH) - intracellular membrane-bound serine hydrolase
- Location: postsynaptic intracellular compartments and smooth ER
- Products: arachidonic acid + ethanolamine
- FAAH inhibitors: URB597, PF-3845 - elevate AEA without direct receptor activation
- Clinical relevance: FAAH inhibition is anxiolytic, analgesic, pro-sleep in animal models
- FAAH polymorphism: C385A variant reduces FAAH activity - associated with reduced anxiety and stress reactivity in humans

## 2-AG Biosynthesis

- Precursor: diacylglycerol (DAG) in postsynaptic membrane
- Primary enzyme: DAGL-alpha (diacylglycerol lipase alpha) - major brain isoform
- Secondary isoform: DAGL-beta predominates in peripheral tissue and immune cells
- Trigger: PLCbeta activation via mGluR5 and other Gq-coupled receptors
- Concentration: 2-AG is 100-1000x more abundant than AEA in brain tissue

## 2-AG Degradation — MAGL

- Primary enzyme: MAGL (monoacylglycerol lipase) - accounts for ~85 percent of 2-AG hydrolysis
- Secondary enzymes: ABHD6 (postsynaptic, 4 percent) and ABHD12 (microglial, 9 percent)
- Products: arachidonic acid + glycerol - arachidonic acid feeds COX-2 neuroinflammation pathway
- MAGL inhibitors: JZL184, KML29 - elevate 2-AG and suppress neuroinflammation
- Dual effect: MAGL inhibition reduces both 2-AG degradation and neuroinflammatory arachidonate pool

## Tonic vs Phasic ECS Signaling

- Tonic: basal level of ECS activity maintained by constitutive synthesis and degradation balance
- Phasic: activity-dependent burst synthesis in response to strong synaptic activation
- FAAH and MAGL set the tonic tone - their inhibition raises the floor of ECS activity
- Pathological disruption: chronic stress depletes AEA via CRF-driven FAAH upregulation

## Guardian Interface Links

- Somatic nodes: 108, 56, 78, 42, 215
- Related nodes: ECS-RECEPTORS-001, ECS-SYNTHESIS-001, ECS-RETROGRADE-001, ECS-PHYTO-001, ECS-STRESS-001
- Related systems: SomaticGraph, TelemetryManager

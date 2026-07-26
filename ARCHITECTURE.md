# Guardian Interface Architecture

## Purpose

Guardian Interface is a research operating system for connecting telemetry, psychology, somatic practice, movement science, and knowledge graph exploration.

The project is designed to support:

- Godot-based visualization
- research archive management
- somatic node mapping
- telemetry and replay systems
- psychology and movement science documentation
- future AI-assisted research tools

---

## Major Layers

### 1. Godot Interface Layer

The Godot layer provides the visual and interactive interface.

Responsibilities:

- dashboards
- somatic graph visualization
- replay views
- atlas exploration
- future XR or interactive learning modes

---

### 2. Tools Layer

The `tools/` directory contains command-line entry points.

Tools should be small wrappers that call reusable code from the `guardian_interface/` package.

Current tools:

- `tools/index_research_graph.py`
- `tools/validate_research_archive.py`

Future tools:

- `tools/search_research.py`
- `tools/dev.py`
- `tools/build_citation_graph.py`

---

### 3. Python Package Layer

The `guardian_interface/` package contains reusable project logic.

Current structure:

guardian_interface/
  archive/
    scanner.py

Planned structure:

guardian_interface/
  archive/
    scanner.py
    parser.py
    validator.py
    graph.py
    search.py

  telemetry/

  visualization/

  atlas/

  ai/

  publication/

---

### 4. Research Archive Layer

The `master_research_archive/` directory stores Markdown research documents.

Each research document should contain front matter metadata, including:

- uid
- title
- category
- associated_somatic_nodes
- tags
- telemetry_triggers
- citations

These documents are indexed into the Knowledge Graph.

---

### 5. Knowledge Graph Layer

The Knowledge Graph is generated as:

master_research_archive/knowledge_graph_index.json

It contains:

- knowledge nodes
- document metadata
- summaries
- headings
- reverse somatic node index

The reverse somatic node index allows one somatic node to link to multiple research documents.

---

### 6. Checkpoint Layer

The `checkpoints/` directory stores stable project milestones.

Each checkpoint should include:

- relevant code
- relevant archive files
- STATUS.md when possible

Checkpoint rule:

Only create a checkpoint after verification.

---

### 7. Specification Layer

The `specifications/` directory stores design documents for future tools and milestones.

Specification workflow:

1. Define purpose.
2. Define inputs.
3. Define outputs.
4. Define validation.
5. Implement.
6. Verify.
7. Checkpoint.

---

## Current Stable Components

### Knowledge Graph Builder

Status:
Stable

File:
tools/index_research_graph.py

Verified checkpoint:
checkpoints/v1_0_2a_knowledge_graph_builder_fixed

---

### Archive Validator

Status:
Phase 1 stable

File:
tools/validate_research_archive.py

Shared library:
guardian_interface/archive/scanner.py

Verified checkpoint:
checkpoints/v1_0_3_archive_library_foundation

---

## Development Principles

### One Tool, One Job

Each command-line tool should have one main responsibility.

Examples:

- validate archive
- build graph
- search archive
- launch replay
- create reports

---

### Shared Logic Belongs in the Package

Reusable logic should go in:

guardian_interface/

Not directly inside tools unless it is only a command-line wrapper.

---

### Checkpoint Before Expansion

Before adding a new feature:

1. Verify the current tool works.
2. Create or update a checkpoint.
3. Document the status.
4. Then continue.

---

### Prefer Small Milestones

Large changes should be broken into smaller verified phases.

Example:

v1.0.3 Archive Validator

Phase 1:
scan archive

Phase 2:
detect front matter

Phase 3:
validate required fields

Phase 4:
duplicate UID detection

Phase 5:
somatic node validation

---

## Near-Term Roadmap

### v1.0.3
Archive Validator

- scan archive
- parse front matter
- validate required metadata
- detect duplicate UIDs
- validate somatic node lists

---

### v1.0.4
Research Search Tool

Example commands:

python3 tools/search_research.py --somatic 42

python3 tools/search_research.py --tag hrv

python3 tools/search_research.py --category Psychology

---

### v1.0.5
Cross Reference Engine

Detect document-to-document references and build relationship maps.

---

### v1.1
Godot Atlas Explorer

Use the generated Knowledge Graph to explore:

- research notes
- somatic nodes
- psychology concepts
- Tai Chi and Qigong links
- telemetry triggers

---

## Long-Term Vision

Guardian Interface is intended to become a research platform connecting:

- psychology
- neuroscience
- anatomy
- physiology
- Tai Chi
- Qigong
- Traditional Chinese Medicine
- movement science
- telemetry
- Godot visualization
- AI-assisted research
- publication workflows

The goal is to keep research, software, and embodied practice organized inside one expandable system.


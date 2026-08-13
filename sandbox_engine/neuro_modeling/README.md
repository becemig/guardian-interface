# Neuro Modeling Sandbox

This directory contains a self-contained sandbox for continuous-time RNN
experiments and Godot-based neuro-modeling demos for the guardian-interface project.

## Structure

- `contracts/`: JSON schema and example parameter packs.
- `python/`: NumPy-based RNN implementation, demos, and tests.
- `godot_demo/`: Godot 4 .NET project for interactive RNN visualization.

## Quick start

1. Run Python tests:

   ```bash
   cd python
   python3 -m unittest discover -s tests -p 'test_*.py' -v
   ```

2. Open `godot_demo` in Godot 4, configure the Mono project, and add
   a scene using `ContinuousTimeRnnNode` once scripts are wired in.

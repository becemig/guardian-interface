# Guardian Interface - Core Pipeline Architecture

## Data Flow Pipeline v0.4

1. Raw telemetry input: `sandbox_engine/live_telemetry.json`
2. Python watcher loop: `sandbox_engine/live_telemetry_enricher.py`
3. State assessment: `GuardianStateEngine`
4. Bridge payload: `sandbox_engine/godot_bridge_payload.json`
5. Godot monitoring: `TelemetryBridgeController.gd`
6. Schema adapter: `DashboardBridgeAdapter.gd`
7. Local model: `TelemetryModel.gd`
8. Staging UI subscribers: `ClusterCard`, `InfluenceGraph`

## Current Best Working State

`checkpoints/staging_model_architecture_verified`

## Containment Rule

The stable v0.3 dashboard remains untouched. All v0.4 integration work happens in staging scenes or sandbox files first.

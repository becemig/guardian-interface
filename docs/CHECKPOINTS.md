# Guardian Interface Checkpoints

## v0.3 Stable Dashboard

**Checkpoint:** `checkpoints/godot_project_v0_3_stable`  
**Status:** Stable  
**Purpose:** Frozen baseline dashboard.  
**Verified behavior:** `TelemetryDashboard.tscn` launches with no parse errors.  
**Rule:** Do not edit stable files directly.

---

## v0.4 Backend Pipeline

**Checkpoint:** `checkpoints/sandbox_engine_v0_4_bridge_live_verified`  
**Status:** Stable  
**Purpose:** Python telemetry enrichment and bridge payload generation.  
**Verified behavior:**
- `live_telemetry.json` updates
- `live_telemetry_enricher.py` detects changes
- `GuardianStateEngine` classifies states
- `godot_bridge_payload.json` updates
- CSV session log appends rows

---

## v0.4 Godot Bridge Test

**Checkpoint:** `checkpoints/godot_bridge_test_scene_verified_clean`  
**Status:** Stable  
**Purpose:** First Godot scene reading bridge payload directly.  
**Verified behavior:**
- `TelemetryBridgeTest.tscn` launches
- `TelemetryBridgeTest.gd` reads JSON bridge file
- Godot console prints state, confidence, HRV, respiration
- Stale warning throttled

---

## v0.4 Signal Bridge

**Checkpoint:** `checkpoints/godot_bridge_controller_signal_test_verified`  
**Status:** Stable  
**Purpose:** Signal-based Godot bridge controller.  
**Verified behavior:**
- `TelemetryBridgeController.gd` initializes
- Emits `telemetry_bridge_updated`
- Emits throttled `telemetry_bridge_stale`
- `TelemetryBridgeTest.gd` receives signal updates

---

## v0.4 Bridge View

**Checkpoint:** `checkpoints/godot_bridge_view_verified`  
**Status:** Stable  
**Purpose:** Visual UI probe for bridge data.  
**Verified behavior:**
- `TelemetryBridgeView.tscn` displays live state
- Confidence, HRV, respiration update
- Health/stale progress bar works

---

## v0.4 Dashboard Adapter

**Checkpoint:** `checkpoints/dashboard_bridge_adapter_verified`  
**Status:** Stable  
**Purpose:** Translate bridge payload into dashboard-compatible payload.  
**Verified behavior:**
- `DashboardBridgeAdapter.gd` receives bridge payload
- Emits dashboard-ready dictionary
- `DashboardBridgeAdapterTest.tscn` prints compatible payload

---

## v0.4 Staging Dashboard

**Checkpoint:** `checkpoints/staging_dashboard_live_visual_verified`  
**Status:** Stable  
**Purpose:** Staging copy of dashboard connected to live bridge.  
**Verified behavior:**
- `StagingDashboard.tscn` launches
- `StagingDashboardController.gd` initializes
- Adapter routes live payload
- ClusterCard receives live updates
- InfluenceGraph receives live updates
- Stress Load → Regulated transitions verified

---

## v0.4 Telemetry Model

**Checkpoint:** `checkpoints/staging_model_architecture_verified`  
**Status:** Stable  
**Purpose:** Local MVC-style telemetry model inside staging controller.  
**Verified behavior:**
- `TelemetryModel.gd` instantiated as child node
- Model receives adapter payload
- Model stores latest telemetry state
- Rolling history buffer exists
- Model routes updates to ClusterCard and InfluenceGraph

---

# Current Best Working State

**Best checkpoint:** `checkpoints/staging_model_architecture_verified`

This represents the current v0.4 staging architecture:

Raw telemetry  
→ Python enricher  
→ GuardianStateEngine  
→ Godot bridge payload  
→ TelemetryBridgeController  
→ DashboardBridgeAdapter  
→ TelemetryModel  
→ ClusterCard + InfluenceGraph

---

# Workflow Rules

1. Do not modify v0.3 stable dashboard files directly.
2. Work in staging scenes first.
3. Checkpoint before every architectural change.
4. Avoid long heredocs when terminal corruption appears.
5. Prefer Python `Path(...).write_text(...)` for complete file replacement.
6. Use `pkill -f live_telemetry_enricher.py` before restarting watcher tests.
7. Verify only one Python watcher is running before live tests.

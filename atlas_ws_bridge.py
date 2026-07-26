#!/usr/bin/env python3
"""
Guardian Atlas — WebSocket Bridge
Runs a local WebSocket server that:
  • Receives JSON signals from your Godot game (GodotAtlasBridge.cs)
  • Forwards them to any connected Atlas web app browser tabs
Usage:
    .venv/bin/python3 atlas_ws_bridge.py
Then in the Atlas web app:
    Live Telemetry → ws://localhost:8765 → Connect
"""

import asyncio
import json
import logging
import signal
import sys
import time

try:
    import websockets
    from websockets.server import serve
except ImportError:
    print("ERROR: websockets not installed.")
    print("Run:  .venv/bin/pip install websockets")
    sys.exit(1)

HOST = "localhost"
PORT = 8765
LOG_LEVEL = logging.INFO

logging.basicConfig(
    level=LOG_LEVEL,
    format="%(asctime)s  %(levelname)-7s  %(message)s",
    datefmt="%H:%M:%S",
)
log = logging.getLogger("atlas-bridge")

class BridgeState:
    godot_clients: set = set()
    atlas_clients: set = set()
    message_count: int = 0
    last_signal: dict = {}

state = BridgeState()

CLASS_TO_TRIGRAM = {
    "LuoPanMatrix":           "Qian",
    "DataHub":                "Kun",
    "SensorWorker":           "Zhen",
    "MuscleMeridianObserver": "Xun",
    "DiagnosticTranslation":  "Kan",
    "DiagnosticWing":         "Li",
    "MockDataDriver":         "Gen",
    "ModalityManager":        "Dui",
}

HZ_TABLE = [
    ("Zhen", 1000), ("Xun", 200), ("Li", 125),
    ("Qian", 100),  ("Kan", 100), ("Dui", 60),
    ("Kun", 30),    ("Gen", 20),
]

MODALITY_TO_L3 = {
    "scan":      "Zhen",
    "assess":    "Li",
    "integrate": "Qian",
    "track":     "Xun",
    "store":     "Kun",
    "switch":    "Dui",
    "replay":    "Gen",
    "translate": "Kan",
}

TRIGRAM_ORDER = ["Qian","Kun","Zhen","Xun","Kan","Li","Gen","Dui"]

def hz_to_trigram(hz: float) -> str:
    return min(HZ_TABLE, key=lambda x: abs(x[1] - hz))[0]

def resolve_trigrams(signal: dict) -> tuple:
    hz       = float(signal.get("sensorHz", 100))
    cls      = signal.get("activeClass", "")
    modality = signal.get("modalityMode", "")
    L1 = CLASS_TO_TRIGRAM.get(cls) or hz_to_trigram(hz)
    L2 = CLASS_TO_TRIGRAM.get(cls, L1)
    L3 = MODALITY_TO_L3.get(modality, L1)
    return L1, L2, L3

def trigrams_to_vol_num(L1: str, L2: str, L3: str) -> int:
    i1 = TRIGRAM_ORDER.index(L1) if L1 in TRIGRAM_ORDER else 0
    i2 = TRIGRAM_ORDER.index(L2) if L2 in TRIGRAM_ORDER else 0
    i3 = TRIGRAM_ORDER.index(L3) if L3 in TRIGRAM_ORDER else 0
    return i1 * 64 + i2 * 8 + i3

def enrich_signal(sig: dict) -> dict:
    L1, L2, L3 = resolve_trigrams(sig)
    vol_num = trigrams_to_vol_num(L1, L2, L3)
    return {
        **sig,
        "timestamp": sig.get("timestamp", time.time()),
        "_resolved": {
            "L1": L1, "L2": L2, "L3": L3,
            "vol_id":  f"VOL-{vol_num:03d}",
            "node_id": f"{L1}.{L2}.{L3}",
        }
    }

async def handler(websocket):
    client_addr = websocket.remote_address
    client_type = None
    log.info(f"New connection from {client_addr}")
    try:
        async for raw in websocket:
            try:
                msg = json.loads(raw)
            except json.JSONDecodeError:
                log.warning(f"Invalid JSON: {raw[:80]}")
                continue

            if msg.get("type") == "register":
                role = msg.get("role", "atlas")
                if role == "godot":
                    client_type = "godot"
                    state.godot_clients.add(websocket)
                    log.info(f"Godot registered: {client_addr}")
                    await websocket.send(json.dumps({"type":"registered","role":"godot","status":"ok"}))
                else:
                    client_type = "atlas"
                    state.atlas_clients.add(websocket)
                    log.info(f"Atlas web registered: {client_addr}")
                    await websocket.send(json.dumps({"type":"registered","role":"atlas","status":"ok"}))
                    if state.last_signal:
                        await websocket.send(json.dumps(state.last_signal))
                continue

            if client_type is None:
                if "sensorHz" in msg or "activeClass" in msg:
                    client_type = "godot"
                    state.godot_clients.add(websocket)
                else:
                    client_type = "atlas"
                    state.atlas_clients.add(websocket)

            if client_type == "godot" and ("sensorHz" in msg or "activeClass" in msg):
                enriched = enrich_signal(msg)
                state.last_signal = enriched
                state.message_count += 1
                resolved = enriched["_resolved"]
                log.info(f"Signal → {resolved['vol_id']} ({resolved['node_id']})  [{len(state.atlas_clients)} atlas clients]")
                if state.atlas_clients:
                    dead = set()
                    for ac in state.atlas_clients:
                        try:
                            await ac.send(json.dumps(enriched))
                        except websockets.exceptions.ConnectionClosed:
                            dead.add(ac)
                    state.atlas_clients -= dead

            elif client_type == "atlas" and msg.get("type") == "command":
                for gc in state.godot_clients:
                    try:
                        await gc.send(json.dumps(msg))
                    except websockets.exceptions.ConnectionClosed:
                        pass

    except websockets.exceptions.ConnectionClosedOK:
        pass
    except websockets.exceptions.ConnectionClosedError as e:
        log.debug(f"Connection error: {e}")
    finally:
        state.godot_clients.discard(websocket)
        state.atlas_clients.discard(websocket)
        log.info(f"Disconnected: {client_addr}")

async def status_ticker():
    while True:
        await asyncio.sleep(30)
        log.info(f"Status — godot:{len(state.godot_clients)}  atlas:{len(state.atlas_clients)}  msgs:{state.message_count}")

async def main():
    loop = asyncio.get_running_loop()
    stop = loop.create_future()
    for sig in (signal.SIGINT, signal.SIGTERM):
        loop.add_signal_handler(sig, stop.set_result, None)
    print()
    print("=" * 58)
    print("  Guardian Atlas — WebSocket Bridge")
    print(f"  Listening on  ws://{HOST}:{PORT}")
    print("  Godot → sends sensor signals here")
    print("  Atlas → open Live Telemetry, enter:")
    print(f"          ws://{HOST}:{PORT}")
    print("  Press Ctrl+C to stop")
    print("=" * 58)
    print()
    async with serve(handler, HOST, PORT):
        asyncio.ensure_future(status_ticker())
        await stop
    print("\nBridge stopped.")

if __name__ == "__main__":
    asyncio.run(main())

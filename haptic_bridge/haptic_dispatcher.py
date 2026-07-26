import asyncio
import json
import socket
import time
from resonance_engine import ResonanceEngine, ResonanceFrame

GODOT_HOST = "127.0.0.1"
GODOT_PORT = 9877

class HapticDispatcher:
    """
    Receives ResonanceFrame from ResonanceEngine via callback,
    serializes to JSON and sends over UDP to FeedbackReceiver in Godot.
    """

    def __init__(self, host: str = GODOT_HOST, port: int = GODOT_PORT):
        self.host = host
        self.port = port
        self._sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self._engine = ResonanceEngine()
        self._engine.register_callback(self.on_frame)
        self._running = False

    async def on_frame(self, frame: ResonanceFrame):
        """Called by ResonanceEngine when a new frame is ready."""
        payload = {
            "timestamp": frame.timestamp,
            "zones": frame.zones,
            "carrier_freq": frame.carrier_freq,
            "phase": frame.phase,
            "confidence": frame.confidence,
            "node_id": frame.node_id,
            "label": frame.label,
        }
        data = json.dumps(payload).encode("utf-8")
        try:
            self._sock.sendto(data, (self.host, self.port))
        except Exception as e:
            print(f"[HapticDispatcher] UDP send error: {e}")

    def push_node(self, node: dict):
        """Call this when the user selects a study node."""
        self._engine.on_node_selected(node)

    def silence(self):
        """Zero all zones and send a silence frame."""
        self._engine.silence_all()
        silence_frame = {
            "timestamp": time.time(),
            "zones": {zid: 0.0 for zid in self._engine.get_zone_list()},
            "carrier_freq": 0.0,
            "phase": "",
            "confidence": 0.0,
            "node_id": "",
            "label": "",
        }
        data = json.dumps(silence_frame).encode("utf-8")
        try:
            self._sock.sendto(data, (self.host, self.port))
        except Exception as e:
            print(f"[HapticDispatcher] silence send error: {e}")

    def close(self):
        self._sock.close()


async def _demo():
    """Quick smoke test — send a Water phase frame."""
    dispatcher = HapticDispatcher()
    test_node = {
        "id": "pat_kidney_yang_def",
        "label": "Kidney Yang Deficiency",
        "subdomain": "Patterns",
        "domain": "TCM",
        "summary": "Cold limbs, lower back ache. Kidney Yang cannot transform fluids.",
        "tags": ["pattern", "kidney", "yang", "deficiency", "water"],
        "confidence": 0.9,
    }
    print("[demo] pushing node:", test_node["label"])
    dispatcher.push_node(test_node)
    await asyncio.sleep(0.1)
    print("[demo] sending silence")
    dispatcher.silence()
    dispatcher.close()
    print("[demo] done")

if __name__ == "__main__":
    asyncio.run(_demo())

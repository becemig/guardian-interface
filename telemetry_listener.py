import socket
import json
from resonance_engine import ResonanceEngine

engine = ResonanceEngine()
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(("127.0.0.1", 8766))
print("[TelemetryListener] Listening on 8766...")

while True:
    data, addr = sock.recvfrom(2048)
    packet = json.loads(data.decode("utf-8"))
    haptic_frame = engine.process(packet)
    print(f"Phase: {packet["phase"]} -> Output Profile: {haptic_frame["profile_id"]} @ {haptic_frame["frequency_hz"]}Hz")
#nullable enable
using Godot;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace GuardianSuit.UDB {
    public sealed class HeartbeatWatchdog : IDisposable {
        private readonly int _timeoutMs;
        private readonly ConcurrentDictionary<Guid, long> _lastAck = new();
        private Action<Guid>? _onFailure;
        private Godot.Timer? _pulseTimer;

        public HeartbeatWatchdog(int timeoutMs = 50) => _timeoutMs = timeoutMs;

        public void Start(Action<Guid> onFailure) {
            _onFailure = onFailure;
            _pulseTimer = new Godot.Timer() { WaitTime = _timeoutMs / 2000.0 };
        }

        public void RegisterDevice(IDockedDevice device) => _lastAck[device.DeviceId] = System.Environment.TickCount64;

        private void Pulse(object? _) {
            long now = System.Environment.TickCount64;
            foreach (var kvp in _lastAck) {
                if (now - kvp.Value > _timeoutMs) {
                    _onFailure?.Invoke(kvp.Key);
                }
            }
        }

        public void Dispose() => _pulseTimer?.Dispose();
    }
}
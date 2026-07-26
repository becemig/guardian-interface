using System;
using System.Runtime.InteropServices;

namespace GuardianSuit.UDB {
    public enum FaultCode { None, HeartbeatTimeout, OverTemperature, TorqueOverload }
    public enum DeviceStatus { Nominal, Fatigued, Tremor, Fault }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public readonly struct TelemetryFrame {
        public readonly Guid DeviceId;
        public readonly long TickId;
        public readonly float JointAngleDeg;
        public readonly float AngularVelocity;
        public readonly float TorqueNm;
        public readonly float FatigueIndex;
        public readonly DeviceStatus Status;

        public TelemetryFrame(Guid deviceId, long tickId, float angle, float velocity, float torque, float fatigue, DeviceStatus status) {
            DeviceId = deviceId;
            TickId = tickId;
            JointAngleDeg = angle;
            AngularVelocity = velocity;
            TorqueNm = torque;
            FatigueIndex = fatigue;
            Status = status;
        }
    }

    public sealed class DeviceManifest {
        public Guid DeviceId { get; init; }
        public string SlotLabel { get; init; } = "";
        public float MaxTorqueNm { get; init; } = 80f;
    }
}
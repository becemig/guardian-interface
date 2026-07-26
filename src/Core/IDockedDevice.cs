using System;
namespace GuardianSuit.UDB {
    public enum DeviceTier { MuscleUnit = 0, JointComplex = 1, KineticChain = 2 }
    public interface IDockedDevice {
        Guid DeviceId { get; }
        string SlotLabel { get; }
        DeviceTier Tier { get; }
        DeviceManifest OnDock(UniversalDockingBus bus);
        void OnUndock();
        TelemetryFrame PollTelemetry(long tickId);
        void AcknowledgeHeartbeat(ulong pulseToken);
        void ApplyModalityProfile(ModalityProfile profile);
        void EmergencyStop(FaultCode reason);
    }
}

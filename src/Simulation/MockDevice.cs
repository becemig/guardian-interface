using Godot;
using System;
using GuardianSuit.UDB;

public partial class MockDevice : Node, IDockedDevice {
    [Export] public string SlotLabel { get; set; } = "MockJoint";
    public Guid DeviceId { get; } = Guid.NewGuid();
    public DeviceTier Tier => DeviceTier.MuscleUnit;

    public override void _Ready() {
        // Register with the UDB singleton
        UniversalDockingBus.Instance?.DockDevice(this);
    }

    public DeviceManifest OnDock(UniversalDockingBus bus) => new() { SlotLabel = this.SlotLabel };
    public void OnUndock() { }
    public TelemetryFrame PollTelemetry(long tickId) => new();

    // The heartbeat response: in a real device, this updates the bus's internal registry
    public void AcknowledgeHeartbeat(ulong pulseToken) {
        // Logic to echo back the heartbeat
    }

    public void ApplyModalityProfile(ModalityProfile profile) { }
    public void EmergencyStop(FaultCode reason) => GD.Print($"[E-STOP] {SlotLabel} triggered by {reason}");
}
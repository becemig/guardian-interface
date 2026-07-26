using Godot;
using System;

public partial class AnomalySentinel : Node
{
    // Define the "Normal" bounds for our alchemical state
    [Export] public float MaxThermalThreshold = 0.85f;
    [Export] public float MinResonanceThreshold = 0.40f;

    [Signal] public delegate void AnomalyDetectedEventHandler(string alertType);

    public override void _Process(double delta)
    {
        MonitorStates();
    }

    private void MonitorStates()
    {
        // Check for Thermal Runaway (FM-002)
        if (SomaticStateMirror.ThermalRunawayIndex > MaxThermalThreshold)
        {
            EmitSignal(SignalName.AnomalyDetected, "THERMAL_RUNAWAY");
        }

        // Check for Systemic Disconnection (Loss of resonance)
        if (SomaticStateMirror.CurrentResonance < MinResonanceThreshold)
        {
            EmitSignal(SignalName.AnomalyDetected, "RESONANCE_COLLAPSE");
        }
    }
}

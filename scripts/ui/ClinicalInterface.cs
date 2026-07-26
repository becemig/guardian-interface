using Godot;
using System;

public partial class ClinicalInterface : Control
{
    public override void _Process(double delta)
    {
        RenderRawData();
    }

    private void RenderRawData()
    {
        // Renders cold, hard engineering metrics
        float thermalRaw = SomaticStateMirror.ThermalRunawayIndex;
        float resonanceRaw = SomaticStateMirror.CurrentResonance;

        // Example output representing a rigid telemetry grid layout
        // GD.Print($"Clinical UI: T_RAW: {thermalRaw:P2} | R_NET: {resonanceRaw:F4} | SECTION_ID: {SomaticStateMirror.ActiveSectionIndex}");
    }
}

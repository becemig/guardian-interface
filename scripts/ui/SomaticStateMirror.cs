using Godot;
using System;

public partial class SomaticStateMirror : Node
{
    // Static mirror of the core suit data, accessible by any UI scene safely
    public static float CurrentResonance { get; set; } = 1.0f;
    public static float ThermalRunawayIndex { get; set; } = 0.0f;
    public static int ActiveSectionIndex { get; set; } = 0;
    
    // Archetypal states derived from the 8 patterns
    public static string PsychologicalArchetype { get; set; } = "The Innocent / Balanced Ground";

    public static void UpdateMirror(float resonance, float thermal, int section)
    {
        CurrentResonance = resonance;
        ThermalRunawayIndex = thermal;
        ActiveSectionIndex = section;

        // Map physical failure boundaries directly to psychological profiles
        if (thermal > 0.8f) PsychologicalArchetype = "The Shadow / Destructive Fire";
        else if (resonance < 0.5f) PsychologicalArchetype = "The Exile / Disconnected Fascia";
        else PsychologicalArchetype = "The Sovereign / Flow State";
    }
}

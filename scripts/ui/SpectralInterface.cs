using Godot;
using System;

public partial class SpectralInterface : Control
{
    private AudioEffectSpectrumAnalyzerInstance _spectrumAnalyzer;
    
    [Export] public int SectionCount = 8; // Locked to our 8-Section Octal Mapping
    [Export] public float FrequencyMax = 11050.0f; // Upper boundary for human movement/somatic track spectrum

    private float[] _currentHarmonics;

    public override void _Ready()
    {
        _currentHarmonics = new float[SectionCount];

        // Access the Spectrum Analyzer effect from Bus 0 (Master), Slot 0
        // Ensure you have added the "SpectrumAnalyzer" effect to your Master Bus in the Godot Audio tab!
        var busEffect = AudioServer.GetBusEffectInstance(0, 0);
        
        if (busEffect is AudioEffectSpectrumAnalyzerInstance instance)
        {
            _spectrumAnalyzer = instance;
            GD.Print("SpectralInterface: Successfully linked to Godot Audio Server Spectrum Analyzer.");
        }
        else
        {
            GD.PrintErr("SpectralInterface CRITICAL: Could not find AudioEffectSpectrumAnalyzerInstance on Master Bus Slot 0.");
        }
    }

    public override void _Process(double delta)
    {
        if (_spectrumAnalyzer == null) return;

        float prevFrequency = 0.0f;
        float frequencyStep = FrequencyMax / SectionCount;

        // Extract magnitudes across the 8 frequency bands mapped to our 8 domains
        for (int i = 0; i < SectionCount; i++)
        {
            float targetFrequency = (i + 1) * frequencyStep;
            
            // Calculate vector length of the frequency range magnitude
            float magnitude = _spectrumAnalyzer.GetMagnitudeForFrequencyRange(prevFrequency, targetFrequency).Length();
            
            // Linear to decibel conversion/normalization for smooth UI scaling
            _currentHarmonics[i] = Mathf.DbToLinear(Mathf.LinearToDb(magnitude) + 45.0f);
            
            prevFrequency = targetFrequency;
        }

        // Pass data downstream to your visual rendering functions
        RenderHarmonicWaves(_currentHarmonics);
    }

    private void RenderHarmonicWaves(float[] harmonicData)
    {
        // This is where your UI Lenses (Line2D, Custom Shaders, or Custom UI Bars) grab the data
        // For debugging, it confirms the 8-channel matrix is actively pulsing
        // GD.Print($"Harmonics (1-8): {string.Join(" | ", harmonicData)}");
        
        // Update the global state mirror so your Time-Shift/Replay system can capture it
        // SomaticStateMirror.UpdateSpectrum(harmonicData);
    }

    /// <summary>
    /// Public getter for other nodes (like the TelemetryLogger or LensController) to pull active spectrum profiles
    /// </summary>
    public float[] GetActiveHarmonics() => _currentHarmonics;
}

// HapticResonanceManager.cs — Guardian Interface · Seasonal Resonance System
// Translates Wu Xing phase + coherence into bio-resonant haptic feedback.
//
// String Instrument Model: a violinist feels resonance, not alerts.
//   Harmonic coherence  -> steady low-frequency whole-body hum
//   Phase transition    -> brief rising bow-change pulse
//   Discordant state    -> staccato correction guide, not alarm
//   Water/deep focus    -> imperceptible slow pulse, like breathing
//   Fire/peak vitality  -> full resonance, high amplitude, warm
//
// Hardware: A) Gamepad Input.StartJoyVibration  B) HapticSuitDriver node
//           C) SimulationMode = true -> logs only, no vibration
// Research: IsResearchMode = true -> logs to user://haptic_log_DATE.jsonl

using Godot;
using System;
using System.Text.Json;

public enum HapticPattern
{
    Sustained,
    Pulse,
    Staccato,
    Rising,
    Breath,
    Silence
}

public class HapticEvent
{
    public WuXingPhase    Phase      { get; set; }
    public PhaseCoherence Coherence  { get; set; }
    public HapticPattern  Pattern    { get; set; }
    public float          Frequency  { get; set; }
    public float          Amplitude  { get; set; }
    public float          Duration   { get; set; }
    public float          PulseRate  { get; set; }
}

public partial class HapticResonanceManager : Node
{
    [Export] public bool  IsResearchMode  = false;
    [Export] public bool  SimulationMode  = false;
    [Export] public int   JoypadIndex     = 0;
    [Export] public float GlobalAmplitude = 0.8f;

    [Export] public NodePath HapticSuitDriverPath = "";
    private  Node            _suitDriver;

    private HapticEvent _currentEvent;
    private float       _pulseTimer  = 0f;
    private bool        _pulseOn     = false;
    private float       _breathPhase = 0f;
    private Godot.FileAccess  _logFile;

    private static HapticEvent _MakeEvent(WuXingPhase phase, PhaseCoherence coherence)
    {
        return (phase, coherence) switch
        {
            (WuXingPhase.Water, PhaseCoherence.Harmonic)   => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Breath,    Frequency=40f,  Amplitude=0.30f, Duration=4.0f, PulseRate=0.25f },
            (WuXingPhase.Water, PhaseCoherence.Neutral)    => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Breath,    Frequency=40f,  Amplitude=0.20f, Duration=4.0f, PulseRate=0.20f },
            (WuXingPhase.Water, PhaseCoherence.Discordant) => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Pulse,     Frequency=40f,  Amplitude=0.35f, Duration=1.5f, PulseRate=0.50f },
            (WuXingPhase.Wood,  PhaseCoherence.Harmonic)   => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Rising,    Frequency=80f,  Amplitude=0.55f, Duration=2.0f, PulseRate=0.40f },
            (WuXingPhase.Wood,  PhaseCoherence.Neutral)    => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Pulse,     Frequency=80f,  Amplitude=0.45f, Duration=1.5f, PulseRate=0.50f },
            (WuXingPhase.Wood,  PhaseCoherence.Discordant) => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Staccato,  Frequency=80f,  Amplitude=0.50f, Duration=0.8f, PulseRate=1.20f },
            (WuXingPhase.Fire,  PhaseCoherence.Harmonic)   => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Sustained, Frequency=160f, Amplitude=0.85f, Duration=2.5f, PulseRate=0.0f  },
            (WuXingPhase.Fire,  PhaseCoherence.Neutral)    => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Pulse,     Frequency=160f, Amplitude=0.70f, Duration=1.0f, PulseRate=0.80f },
            (WuXingPhase.Fire,  PhaseCoherence.Discordant) => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Staccato,  Frequency=160f, Amplitude=0.60f, Duration=0.5f, PulseRate=2.00f },
            (WuXingPhase.Earth, PhaseCoherence.Harmonic)   => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Sustained, Frequency=120f, Amplitude=0.60f, Duration=3.0f, PulseRate=0.0f  },
            (WuXingPhase.Earth, PhaseCoherence.Neutral)    => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Pulse,     Frequency=120f, Amplitude=0.50f, Duration=2.0f, PulseRate=0.40f },
            (WuXingPhase.Earth, PhaseCoherence.Discordant) => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Pulse,     Frequency=120f, Amplitude=0.55f, Duration=1.2f, PulseRate=0.80f },
            (WuXingPhase.Metal, PhaseCoherence.Harmonic)   => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Sustained, Frequency=200f, Amplitude=0.45f, Duration=3.5f, PulseRate=0.0f  },
            (WuXingPhase.Metal, PhaseCoherence.Neutral)    => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Pulse,     Frequency=200f, Amplitude=0.38f, Duration=2.0f, PulseRate=0.30f },
            (WuXingPhase.Metal, PhaseCoherence.Discordant) => new HapticEvent { Phase=phase, Coherence=coherence, Pattern=HapticPattern.Staccato,  Frequency=200f, Amplitude=0.42f, Duration=0.6f, PulseRate=1.50f },
            _ => new HapticEvent { Pattern = HapticPattern.Silence, Amplitude = 0f }
        };
    }

    public override void _Ready()
    {
        if (!string.IsNullOrEmpty(HapticSuitDriverPath))
            _suitDriver = GetNodeOrNull<Node>(HapticSuitDriverPath);

        UniversalDockingBus.Instance.Subscribe(BusEvent.PhaseChanged,    _OnPhaseChanged);
        UniversalDockingBus.Instance.Subscribe(BusEvent.CoherenceChanged, _OnCoherenceChanged);

        if (IsResearchMode) _OpenResearchLog();

        GD.Print("[HapticResonanceManager] Ready" +
                 (SimulationMode ? " [SIMULATION]" : "") +
                 (IsResearchMode ? " [RESEARCH LOG ON]" : ""));
    }

    public override void _ExitTree()
    {
        Input.StopJoyVibration(JoypadIndex);
        UniversalDockingBus.Instance.Unsubscribe(BusEvent.PhaseChanged,    _OnPhaseChanged);
        UniversalDockingBus.Instance.Unsubscribe(BusEvent.CoherenceChanged, _OnCoherenceChanged);
        _logFile?.Close();
    }

    public void _OnPhaseChanged(object data)
    {
        if (data is not PhaseState state) return;
        _SetEvent(_MakeEvent(state.Phase, state.Coherence));
    }

    public void _OnCoherenceChanged(object data)
    {
        if (data is not CoherencePayload c) return;
        if (_currentEvent == null) return;
        _currentEvent.Coherence = c.State;
        _currentEvent.Amplitude = Mathf.Lerp(0.2f, 1.0f, c.Score) * GlobalAmplitude;
    }

    public override void _Process(double delta)
    {
        if (_currentEvent == null) return;
        float dt = (float)delta;
        switch (_currentEvent.Pattern)
        {
            case HapticPattern.Sustained:
                _SendHaptic(_currentEvent.Amplitude, _currentEvent.Amplitude);
                break;
            case HapticPattern.Breath:
                _breathPhase += dt * _currentEvent.PulseRate * Mathf.Tau;
                float breathAmp = (Mathf.Sin(_breathPhase) * 0.5f + 0.5f) * _currentEvent.Amplitude;
                _SendHaptic(breathAmp, breathAmp * 0.6f);
                break;
            case HapticPattern.Pulse:
                _pulseTimer += dt;
                float pulseInterval = 1.0f / Mathf.Max(_currentEvent.PulseRate, 0.01f);
                if (_pulseTimer >= pulseInterval) { _pulseTimer = 0f; _pulseOn = !_pulseOn; }
                _SendHaptic(_pulseOn ? _currentEvent.Amplitude : 0f, _pulseOn ? _currentEvent.Amplitude * 0.5f : 0f);
                break;
            case HapticPattern.Staccato:
                _pulseTimer += dt;
                float staccatoInterval = 1.0f / Mathf.Max(_currentEvent.PulseRate, 0.01f);
                if (_pulseTimer >= staccatoInterval) _pulseTimer = 0f;
                bool staccatoOn = _pulseTimer < staccatoInterval * 0.25f;
                _SendHaptic(staccatoOn ? _currentEvent.Amplitude : 0f, 0f);
                break;
            case HapticPattern.Rising:
                _pulseTimer = Mathf.Min(_pulseTimer + dt, _currentEvent.Duration);
                float risingAmp = Mathf.SmoothStep(0f, _currentEvent.Amplitude, _pulseTimer / _currentEvent.Duration);
                _SendHaptic(risingAmp, risingAmp * 0.7f);
                break;
            case HapticPattern.Silence:
                Input.StopJoyVibration(JoypadIndex);
                break;
        }
    }

    private void _SetEvent(HapticEvent evt)
    {
        _currentEvent = evt;
        _pulseTimer   = 0f;
        _pulseOn      = false;
        _breathPhase  = 0f;
        if (IsResearchMode) _LogEvent(evt);
        GD.Print($"[HapticResonanceManager] {evt.Phase}/{evt.Coherence} -> {evt.Pattern} @ {evt.Frequency}Hz  amp:{evt.Amplitude:F2}");
    }

    private void _SendHaptic(float lowFreqAmp, float highFreqAmp)
    {
        float lo = Mathf.Clamp(lowFreqAmp  * GlobalAmplitude, 0f, 1f);
        float hi = Mathf.Clamp(highFreqAmp * GlobalAmplitude, 0f, 1f);
        if (SimulationMode) return;
        Input.StartJoyVibration(JoypadIndex, lo, hi, 0.05f);
        if (_suitDriver != null && _suitDriver.HasMethod("SendResonance"))
            _suitDriver.Call("SendResonance", _currentEvent?.Frequency ?? 120f, lo);
    }

    private void _OpenResearchLog()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string path = $"user://haptic_log_{date}.jsonl";
        _logFile = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.WriteRead);
        if (_logFile == null)
            GD.PrintErr("[HapticResonanceManager] Could not open research log");
        else
            GD.Print($"[HapticResonanceManager] Research log: {path}");
    }

    private void _LogEvent(HapticEvent evt)
    {
        if (_logFile == null) return;
        var entry = new
        {
            timestamp  = DateTime.UtcNow.ToString("o"),
            phase      = evt.Phase.ToString(),
            coherence  = evt.Coherence.ToString(),
            pattern    = evt.Pattern.ToString(),
            frequency  = evt.Frequency,
            amplitude  = evt.Amplitude,
            duration   = evt.Duration,
            pulse_rate = evt.PulseRate
        };
        _logFile.StoreLine(JsonSerializer.Serialize(entry));
    }

    public void PlayAlignmentPulse()
    {
        var phase = PhaseGovernor.Instance?.Phase ?? WuXingPhase.Earth;
        _SetEvent(new HapticEvent
        {
            Phase     = phase,
            Pattern   = HapticPattern.Rising,
            Frequency = _MakeEvent(phase, PhaseCoherence.Harmonic).Frequency,
            Amplitude = 0.9f,
            Duration  = 1.2f
        });
    }

    public void PlayCorrectionNudge()
    {
        var phase = PhaseGovernor.Instance?.Phase ?? WuXingPhase.Earth;
        _SetEvent(new HapticEvent
        {
            Phase     = phase,
            Pattern   = HapticPattern.Staccato,
            Frequency = _MakeEvent(phase, PhaseCoherence.Discordant).Frequency,
            Amplitude = 0.45f,
            Duration  = 0.6f,
            PulseRate = 3.0f
        });
    }
}

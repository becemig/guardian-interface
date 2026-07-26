using Godot;
using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

/// <summary>
/// Subscribes to BusEvent.TelemetryDataPushed and BusEvent.SensorSignalReceived.
/// Buffers incoming telemetry packets, computes rolling coherence,
/// and re-publishes BusEvent.CoherenceChanged when the score shifts significantly.
/// Also logs packets to session log for research mode IRB compliance.
/// </summary>
public partial class TelemetryPipeline : Node
{
    [Export] public int CoherenceWindowSize = 32;
    [Export] public float CoherenceChangeThreshold = 0.05f;
    [Export] public bool EnableSessionLog = true;

    private readonly Queue<TelemetryPacket> _window = new();
    private float _lastCoherence = 0f;
    private UniversalDockingBus _bus;

    public override void _Ready()
    {
        _bus = GetNodeOrNull<UniversalDockingBus>("/root/UniversalDockingBus");
        if (_bus == null)
        {
            GD.PrintErr("[TelemetryPipeline] UniversalDockingBus not found.");
            return;
        }
        _bus.Subscribe(BusEvent.TelemetryDataPushed, OnTelemetryData);
        _bus.Subscribe(BusEvent.SensorSignalReceived, OnSensorSignal);
        GD.Print("[TelemetryPipeline] Subscribed to bus.");
    }

    private void OnTelemetryData(object raw)
    {
        if (raw is TelemetryPacket pkt)
            ProcessPacket(pkt);
        else if (raw is string json)
        {
            try
            {
                var pkt2 = JsonSerializer.Deserialize<TelemetryPacket>(json);
                if (pkt2 != null) ProcessPacket(pkt2);
            }
            catch (Exception e)
            {
                GD.PrintErr($"[TelemetryPipeline] JSON parse error: {e.Message}");
            }
        }
    }

    private void OnSensorSignal(object raw)
    {
        if (raw is ResonanceFramePacket frame)
        {
            var pkt = new TelemetryPacket
            {
                timestamp = frame.timestamp,
                source = "haptic_bridge",
                phase = frame.phase,
                coherence = frame.confidence,
                carrier_freq = frame.carrier_freq,
                node_id = frame.node_id,
            };
            ProcessPacket(pkt);
        }
    }

    private void ProcessPacket(TelemetryPacket pkt)
    {
        _window.Enqueue(pkt);
        while (_window.Count > CoherenceWindowSize)
            _window.Dequeue();

        float coherence = ComputeCoherence();
        if (Math.Abs(coherence - _lastCoherence) >= CoherenceChangeThreshold)
        {
            _lastCoherence = coherence;
            _bus?.Publish(BusEvent.CoherenceChanged, coherence);
        }

        if (EnableSessionLog)
            AppendSessionLog(pkt);
    }

    private float ComputeCoherence()
    {
        if (_window.Count < 2) return 0f;
        float sum = 0f;
        foreach (var p in _window)
            sum += p.coherence;
        return sum / _window.Count;
    }

    private void AppendSessionLog(TelemetryPacket pkt)
    {
        try
        {
            using var f = Godot.FileAccess.Open(
                "user://telemetry_session.jsonl",
                Godot.FileAccess.ModeFlags.ReadWrite);
            if (f == null) return;
            f.SeekEnd(0);
            f.StoreLine(JsonSerializer.Serialize(pkt));
        }
        catch (Exception e)
        {
            GD.PrintErr($"[TelemetryPipeline] Log write error: {e.Message}");
        }
    }

    public override void _ExitTree()
    {
        if (_bus != null)
        {
            _bus.Unsubscribe(BusEvent.TelemetryDataPushed, OnTelemetryData);
            _bus.Unsubscribe(BusEvent.SensorSignalReceived, OnSensorSignal);
        }
    }
}

public class TelemetryPacket
{
    public double timestamp { get; set; }
    public string source { get; set; } = "";
    public string phase { get; set; } = "";
    public float coherence { get; set; }
    public float carrier_freq { get; set; }
    public string node_id { get; set; } = "";
    public Dictionary<string, float> zones { get; set; } = new();
    public Dictionary<string, object> extra { get; set; } = new();
}

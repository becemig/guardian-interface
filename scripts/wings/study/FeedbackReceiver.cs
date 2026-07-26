using Godot;
using System;
using System.Text;
using System.Text.Json;

/// <summary>
/// Listens on UDP port 9877 for ResonanceFrame JSON packets from resonance_engine.py.
/// Parses each frame and publishes BusEvent.SensorSignalReceived to UniversalDockingBus.
/// Autoloaded or attached to a persistent node in the scene tree.
/// </summary>
public partial class FeedbackReceiver : Node
{
    [Export] public int ListenPort = 9877;

    private PacketPeerUdp _udp;
    private bool _listening = false;

    public override void _Ready()
    {
        _udp = new PacketPeerUdp();
        var err = _udp.Bind(ListenPort);
        if (err == Error.Ok)
        {
            _listening = true;
            GD.Print($"[FeedbackReceiver] Listening on UDP :{ListenPort}");
        }
        else
        {
            GD.PrintErr($"[FeedbackReceiver] Failed to bind UDP :{ListenPort} — {err}");
        }
    }

    public override void _Process(double delta)
    {
        if (!_listening) return;
        while (_udp.GetAvailablePacketCount() > 0)
        {
            var raw = _udp.GetPacket();
            if (raw == null || raw.Length == 0) continue;
            try
            {
                var json = Encoding.UTF8.GetString(raw);
                var frame = JsonSerializer.Deserialize<ResonanceFramePacket>(json);
                if (frame != null)
                    PublishFrame(frame);
            }
            catch (Exception e)
            {
                GD.PrintErr($"[FeedbackReceiver] Parse error: {e.Message}");
            }
        }
    }

    private void PublishFrame(ResonanceFramePacket frame)
    {
        var bus = GetNodeOrNull<UniversalDockingBus>("/root/UniversalDockingBus");
        if (bus == null) return;
        bus.Publish(BusEvent.SensorSignalReceived, frame);
        // Also publish CoherenceChanged if confidence field present
        if (frame.confidence > 0f)
            bus.Publish(BusEvent.CoherenceChanged, frame.confidence);
    }

    public override void _ExitTree()
    {
        _udp?.Close();
        _listening = false;
    }
}

/// <summary>
/// Mirrors the ResonanceFrame dataclass from resonance_engine.py.
/// zones: dict of zone_id -> amplitude (0.0-1.0)
/// </summary>
public class ResonanceFramePacket
{
    public double timestamp { get; set; }
    public System.Collections.Generic.Dictionary<string, float> zones { get; set; }
    public float carrier_freq { get; set; }
    public string phase { get; set; } = "";
    public float confidence { get; set; }
    public string node_id { get; set; } = "";
    public string label { get; set; } = "";
}

using Godot;
using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

public partial class GodotAtlasBridge : Node
{
    public static GodotAtlasBridge Instance { get; private set; }

    [Export] public string BridgeUrl = "ws://localhost:8765/ws";

    private WebSocketPeer _ws;
    private enum ConnectionState { Disconnected, Connecting, Connected }
    private ConnectionState _connState = ConnectionState.Disconnected;

    public override void _Ready()
    {
        Instance = this;
        _ws = new WebSocketPeer();
        Connect();
    }

    public override void _Process(double delta)
    {
        if (_ws == null) return;
        _ws.Poll();
        if (_ws.GetReadyState() == WebSocketPeer.State.Open && _connState == ConnectionState.Connecting)
        {
            _connState = ConnectionState.Connected;
            SendRegistration();
        }
        while (_ws.GetAvailablePacketCount() > 0)
            HandleIncoming(Encoding.UTF8.GetString(_ws.GetPacket()));
    }

    public void Connect() {
        _connState = ConnectionState.Connecting;
        _ws.ConnectToUrl(BridgeUrl);
    }

    private void SendRegistration()
    {
        var reg = new Dictionary<string, object>
        {
            { "type", "register" },
            { "role", "godot" },
            { "volId", "VOL-146" }
        };
        _ws.SendText(JsonSerializer.Serialize(reg));
        GD.Print("[AtlasBridge] Registration sent for VOL-146");
    }

    private void HandleIncoming(string text) => GD.Print($"[AtlasBridge] SERVER RESPONSE: {text}");

    public override void _ExitTree() => _ws?.Close();
}
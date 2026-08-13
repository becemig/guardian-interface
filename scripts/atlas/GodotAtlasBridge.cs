using Godot;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

public partial class GodotAtlasBridge : Node
{
    public static GodotAtlasBridge Instance { get; private set; }

    [Export] public string BridgeUrl = "ws://localhost:8765/ws";

    [Export]
    public bool LogFullServerResponses = false;

    [Export]
    public int SummaryEveryNResponses = 120;

    private WebSocketPeer _ws;
    private int _responseCount;

    private enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected
    }

    private ConnectionState _connState = ConnectionState.Disconnected;

    public override void _Ready()
    {
        Instance = this;
        _ws = new WebSocketPeer();
        Connect();
    }

    public override void _Process(double delta)
    {
        if (_ws == null)
            return;

        _ws.Poll();

        if (
            _ws.GetReadyState() == WebSocketPeer.State.Open &&
            _connState == ConnectionState.Connecting
        )
        {
            _connState = ConnectionState.Connected;
            SendRegistration();
        }

        while (_ws.GetAvailablePacketCount() > 0)
        {
            string message = Encoding.UTF8.GetString(
                _ws.GetPacket()
            );

            HandleIncoming(message);
        }
    }

    public void Connect()
    {
        _connState = ConnectionState.Connecting;
        _ws.ConnectToUrl(BridgeUrl);
    }

    private void SendRegistration()
    {
        var registration = new Dictionary<string, object>
        {
            { "type", "register" },
            { "role", "godot" },
            { "volId", "VOL-146" }
        };

        _ws.SendText(
            JsonSerializer.Serialize(registration)
        );

        GuardianDebug.Atlas(
            "Registration sent for VOL-146"
        );
    }

    private void HandleIncoming(string text)
    {
        _responseCount++;

        if (LogFullServerResponses)
        {
            GD.Print(
                $"[AtlasBridge] SERVER RESPONSE: {text}"
            );

            return;
        }

        int interval = Mathf.Max(1, SummaryEveryNResponses);

        if (_responseCount % interval == 0)
        {
            GuardianDebug.Atlas(
                $"response #{_responseCount}, " +
                $"payloadBytes={text.Length}"
            );
        }
    }

    public override void _ExitTree()
    {
        _ws?.Close();

        if (Instance == this)
            Instance = null;
    }
}

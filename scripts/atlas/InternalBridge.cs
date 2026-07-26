using Godot;
using System.Collections.Generic;

public partial class InternalBridge : Node
{
    private WebSocketMultiplayerPeer _server = new WebSocketMultiplayerPeer();
    private int _port = 8765;

    public override void _Ready()
    {
        Error err = _server.CreateServer(_port);
        if (err == Error.Ok)
        {
            Multiplayer.MultiplayerPeer = _server;
            GD.Print($"Native Guardian Bridge active on ws://localhost:{_port}");
        }
        else
        {
            GD.PrintErr($"Failed to start bridge: {err}");
        }
    }

    public override void _Process(double delta)
    {
        _server.Poll();
    }
}

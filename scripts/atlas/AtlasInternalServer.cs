using Godot;
using System.Collections.Generic;

public partial class AtlasInternalServer : Node
{
    private ENetMultiplayerPeer _peer = new ENetMultiplayerPeer();
    
    public override void _Ready()
    {
        _peer.CreateServer(8765);
        Multiplayer.MultiplayerPeer = _peer;
        GD.Print("Native Guardian Bridge active on port 8765");
    }
}

using Godot;
using System;

public partial class DiagnosticTest : Node
{
    public override void _Ready()
    {
        GD.Print("========================================");
        GD.Print("[DIAGNOSTIC] Headless Engine Pipeline Link: SUCCESS");
        GD.Print("[DIAGNOSTIC] C# Assembly loaded perfectly.");
        GD.Print("========================================");
        // GetTree().Quit(); -- patched for BaguaPhysicsClient connection test
    }
}

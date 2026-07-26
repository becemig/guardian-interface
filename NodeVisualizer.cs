using Godot;
using System;

public partial class NodeVisualizer : Node3D
{
    public override void _Ready()
    {
        var node = GetNode("/root/AcademyManager");
        if (node is AcademyManager manager)
        {
            GD.Print("Nodes found: " + manager.Nodes.Count);
        }
    }
}

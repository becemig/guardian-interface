using Godot;
using System.Collections.Generic;

public partial class AcademyManager : Node
{
    public static AcademyManager Instance { get; private set; }
    public List<Node> Nodes { get; } = new List<Node>();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        GD.Print("[AcademyManager] Initialized. Creating Debug UI...");
        
        // Create a visual label
        Label debugLabel = new Label();
        debugLabel.Text = "Guardian Interface Operational\nYinYang State: " + VariableGovernor.Instance.YinYang;
        debugLabel.Position = new Vector2(50, 50);
        
        // Add to the main scene root
        GetTree().Root.AddChild(debugLabel);
    }
}

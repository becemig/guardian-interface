using Godot;
using Godot.Collections;

public partial class KineticNode : Node
{
    [Export] public string ModuleId { get; set; }
    [Export] public string ModuleName { get; set; }
    [Export] public float PowerCapacity { get; set; } = 500.0f;
    [Export] public float Value { get; set; } = 1.0f; // Added for KLODManager
    [Export] public Array<string> SupportedPrinciples { get; set; } = new Array<string>();
}

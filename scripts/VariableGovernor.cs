using Godot;

public partial class VariableGovernor : Node
{
    public static VariableGovernor Instance { get; private set; }
    
    public float YinYang { get; private set; } = 0.5f;

    public override void _EnterTree()
    {
        Instance = this;
    }
}

using Godot;
public abstract partial class BaseWing : Control
{
    public override void _Ready() { OnEnter(); }
    public override void _ExitTree() { OnExit(); }
    protected virtual void OnEnter() { }
    protected virtual void OnExit() { }
}

using Godot;
using Godot.Collections;
using GodotDictionary = Godot.Collections.Dictionary;

public partial class ActionState : Resource
{
    [Export] public string StateName { get; set; }
    [Export] public Array<string> ActivePrinciples { get; set; } = new Array<string>();
    public System.Collections.Generic.Dictionary<string, float> HardwareConstraints { get; set; } = new System.Collections.Generic.Dictionary<string, float>();
}

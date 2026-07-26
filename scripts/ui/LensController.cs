using System.Text.Json;
using Godot;
using System;

public partial class LensController : Node
{
    public enum LensType { Tensegrity, LuoPan, Calligraphy }
    private LensType _activeLens = LensType.Tensegrity;
    private string lastFocusId = "";
    private string bridgePath = "/home/becemig/GodotProjects/guardian-interface/master_data/current_focus.json";

    public void SwitchLens(int lensIndex)
    {
        _activeLens = (LensType)lensIndex;
        GD.Print($"LensController: Switching to {_activeLens} interface.");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_select"))
        {
            int next = ((int)_activeLens + 1) % 3;
            SwitchLens(next);
        }
    }

    public override void _Process(double delta)
    {
        if (System.IO.File.Exists(bridgePath))
        {
            try
            {
                string json = System.IO.File.ReadAllText(bridgePath);
                var doc = JsonDocument.Parse(json);
                string newFocus = doc.RootElement.GetProperty("focus_node").GetString() ?? "";
                if (newFocus != lastFocusId)
                {
                    lastFocusId = newFocus;
                    GD.Print($"LensController: Focus -> {newFocus}");
                }
            }
            catch { }
        }
    }
}

using Godot;
using System.Collections.Generic;

public enum LayerType { Modality, Principle, Intent }

public class SuitLayer
{
    public string Name;
    public bool IsActive;
    public LayerType Type;
}

public partial class LayeredGuardianCore : Node
{
    private List<SuitLayer> _layers = new List<SuitLayer>();

    public override void _Ready()
    {
        GD.Print("GuardianCore: Layered System Online.");
    }

    public void RegisterLayer(string name, LayerType type, bool active)
    {
        _layers.Add(new SuitLayer { Name = name, Type = type, IsActive = active });
        GD.Print($"Layer Registered: {name} ({type}) - Active: {active}");
    }

    public void ToggleLayer(string name, bool state)
    {
        var layer = _layers.Find(l => l.Name == name);
        if (layer != null) 
        {
            layer.IsActive = state;
            GD.Print($"Layer {name} set to {state}");
        }
    }

    // This loop processes only the layers that are toggled 'ON'
    public void ProcessActiveLayers(float telemetry)
    {
        foreach (var layer in _layers)
        {
            if (layer.IsActive)
            {
                GD.Print($"Computing Physics for: {layer.Name}...");
                // Here you would inject specific logic based on the layer type
            }
        }
    }
}

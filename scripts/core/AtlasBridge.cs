using Godot;
using System;

public partial class AtlasBridge : Node
{
    public override void _Ready()
    {
        GD.Print("AtlasBridge: Systems synchronized with Bus.");
    }

    public void OnSensorDataReceived(float[] data)
    {
        UniversalDockingBus.Instance.Publish(BusEvent.SensorSignalReceived, data);
    }
}

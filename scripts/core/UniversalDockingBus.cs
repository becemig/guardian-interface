using Godot;
using System;
using System.Collections.Generic;

public enum BusEvent { ModeWillChange, ModeChanged, VolumeLoaded, SensorSignalReceived, TelemetryDataPushed, PhaseChanged, CoherenceChanged, UiAccentChanged, PatternSelected, AcupointQueried, HerbQueried, ReasoningModeChanged, EvidenceFilterChanged, RedFlagTriggered }

public partial class UniversalDockingBus : Node
{
    public static UniversalDockingBus Instance { get; private set; }
    private readonly Dictionary<BusEvent, List<Action<object>>> _subscribers = new();

    public override void _Ready()
    {
        if (Instance != null) { QueueFree(); return; }
        Instance = this;
    }

    public void Publish(BusEvent evt, object payload)
    {
        if (_subscribers.ContainsKey(evt))
            foreach (var handler in _subscribers[evt]) handler(payload);
    }

    public void Subscribe(BusEvent evt, Action<object> handler)
    {
        if (!_subscribers.ContainsKey(evt)) _subscribers[evt] = new List<Action<object>>();
        _subscribers[evt].Add(handler);
    }

    public void Unsubscribe(BusEvent evt, Action<object> handler)
    {
        if (_subscribers.TryGetValue(evt, out var list)) list.Remove(handler);
    }

    public void DockDevice(Node device) { GD.Print("Device docked to bus."); }
}

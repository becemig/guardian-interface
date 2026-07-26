using Godot;
using System.Collections.Generic;

public enum TrinityMode { PurePlay, ResearchGame, PureResearch, Study }

public partial class ModeSwitcher : Node
{
    public static ModeSwitcher Instance { get; private set; }
    private static readonly Dictionary<TrinityMode, string> WingScenes = new() {
        { TrinityMode.PurePlay, "res://wings/play/PlayWing.tscn" },
        { TrinityMode.ResearchGame, "res://wings/research_game/ResearchGameWing.tscn" },
        { TrinityMode.PureResearch, "res://wings/research/ResearchWing.tscn" },
        { TrinityMode.Study, "res://scenes/wings/study/StudyWing.tscn" }
    };
    
    private Node _activeWing;
    public override void _Ready() { Instance = this; }

    public void SwitchTo(TrinityMode mode)
    {
        if (_activeWing != null) _activeWing.QueueFree();
        var packed = ResourceLoader.Load<PackedScene>(WingScenes[mode]);
        _activeWing = packed.Instantiate();
        GetTree().Root.AddChild(_activeWing);
    }
}

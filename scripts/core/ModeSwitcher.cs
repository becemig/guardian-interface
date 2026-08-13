using Godot;
using System.Collections.Generic;

public enum TrinityMode
{
    PurePlay,
    ResearchGame,
    PureResearch,
    Study
}

public partial class ModeSwitcher : Node
{
    public static ModeSwitcher Instance { get; private set; }

    private static readonly Dictionary<TrinityMode, string> WingScenes = new()
    {
        {
            TrinityMode.PurePlay,
            "res://scenes/wings/play/PlayWing.tscn"
        },
        {
            TrinityMode.Study,
            "res://scenes/wings/study/StudyWing.tscn"
        }
    };

    private Node _activeWing;

    public override void _Ready()
    {
        Instance = this;
    }

    public void SwitchTo(TrinityMode mode)
    {
        if (!WingScenes.TryGetValue(mode, out string scenePath))
        {
            GD.PushWarning(
                $"Guardian mode '{mode}' is unavailable: " +
                "no validated scene is registered."
            );
            return;
        }

        if (!ResourceLoader.Exists(scenePath, "PackedScene"))
        {
            GD.PushError(
                $"Guardian mode '{mode}' is unavailable: " +
                $"scene not found at '{scenePath}'."
            );
            return;
        }

        PackedScene packedScene = ResourceLoader.Load<PackedScene>(
            scenePath
        );

        if (packedScene == null)
        {
            GD.PushError(
                $"Guardian mode '{mode}' could not load scene " +
                $"'{scenePath}'."
            );
            return;
        }

        Node nextWing = packedScene.Instantiate();

        if (nextWing == null)
        {
            GD.PushError(
                $"Guardian mode '{mode}' could not instantiate " +
                $"'{scenePath}'."
            );
            return;
        }

        GetTree().Root.AddChild(nextWing);

        Node previousWing = _activeWing;
        _activeWing = nextWing;

        if (
            previousWing != null &&
            GodotObject.IsInstanceValid(previousWing)
        )
        {
            previousWing.QueueFree();
        }

        GD.Print(
            $"Guardian mode switched to '{mode}' using '{scenePath}'."
        );
    }
}

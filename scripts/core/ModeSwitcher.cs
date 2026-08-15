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
    private TrinityMode? _activeMode;

    public Node ActiveWing => _activeWing;

    public TrinityMode? ActiveMode => _activeMode;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            GD.PushWarning(
                "[ModeSwitcher] Duplicate instance detected; " +
                "the newest instance will be discarded."
            );
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool SwitchTo(TrinityMode mode)
    {
        if (
            _activeMode == mode &&
            _activeWing != null &&
            GodotObject.IsInstanceValid(_activeWing)
        )
        {
            GD.Print(
                $"[ModeSwitcher] '{mode}' is already active; " +
                "ignoring duplicate switch request."
            );
            return true;
        }

        if (!WingScenes.TryGetValue(mode, out string scenePath))
        {
            GD.PushWarning(
                $"[ModeSwitcher] Guardian mode '{mode}' is unavailable: " +
                "no validated scene is registered."
            );
            return false;
        }

        if (!ResourceLoader.Exists(scenePath, "PackedScene"))
        {
            GD.PushError(
                $"[ModeSwitcher] Guardian mode '{mode}' is unavailable: " +
                $"scene not found at '{scenePath}'."
            );
            return false;
        }

        PackedScene packedScene = ResourceLoader.Load<PackedScene>(scenePath);

        if (packedScene == null)
        {
            GD.PushError(
                $"[ModeSwitcher] Guardian mode '{mode}' could not load " +
                $"scene '{scenePath}'."
            );
            return false;
        }

        Node nextWing = packedScene.Instantiate();

        if (nextWing == null)
        {
            GD.PushError(
                $"[ModeSwitcher] Guardian mode '{mode}' could not " +
                $"instantiate '{scenePath}'."
            );
            return false;
        }

        Window rootWindow = GetTree()?.Root;

        if (rootWindow == null)
        {
            GD.PushError(
                $"[ModeSwitcher] Guardian mode '{mode}' could not " +
                "resolve the root window."
            );
            nextWing.QueueFree();
            return false;
        }

        rootWindow.AddChild(nextWing);

        Node previousWing = _activeWing;
        _activeWing = nextWing;
        _activeMode = mode;

        if (
            previousWing != null &&
            GodotObject.IsInstanceValid(previousWing)
        )
        {
            previousWing.CallDeferred(Node.MethodName.QueueFree);
        }

        GD.Print(
            $"[ModeSwitcher] Guardian mode switched to '{mode}' " +
            $"using '{scenePath}'."
        );

        return true;
    }
}

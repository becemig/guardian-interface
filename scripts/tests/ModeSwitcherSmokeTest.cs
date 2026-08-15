using Godot;
using System;

public partial class ModeSwitcherSmokeTest : Node
{
    private const int SuccessExitCode = 0;
    private const int FailureExitCode = 1;

    private ModeSwitcher _switcher;
    private bool _createdSwitcher;

    public override void _Ready()
    {
        CallDeferred(MethodName.Run);
    }

    private void Run()
    {
        try
        {
            _switcher = ModeSwitcher.Instance;

            if (_switcher == null || !GodotObject.IsInstanceValid(_switcher))
            {
                _switcher = new ModeSwitcher();
                AddChild(_switcher);
                _createdSwitcher = true;
            }

            bool firstSwitch = _switcher.SwitchTo(TrinityMode.PurePlay);

            if (!firstSwitch)
            {
                Fail("PurePlay route returned false.");
                return;
            }

            if (_switcher.ActiveMode != TrinityMode.PurePlay)
            {
                Fail("ActiveMode was not set to PurePlay.");
                return;
            }

            if (
                _switcher.ActiveWing == null ||
                !GodotObject.IsInstanceValid(_switcher.ActiveWing)
            )
            {
                Fail("PurePlay did not create a valid active wing.");
                return;
            }

            if (_switcher.ActiveWing is not PlayWing)
            {
                Fail(
                    "PurePlay created an unexpected root type: " +
                    _switcher.ActiveWing.GetType().Name
                );
                return;
            }

            Node firstWing = _switcher.ActiveWing;
            int rootChildCountBeforeRepeat = GetTree().Root.GetChildCount();

            bool repeatSwitch = _switcher.SwitchTo(TrinityMode.PurePlay);

            if (!repeatSwitch)
            {
                Fail("Repeated PurePlay route returned false.");
                return;
            }

            if (_switcher.ActiveWing != firstWing)
            {
                Fail("Repeated PurePlay route replaced the active wing.");
                return;
            }

            if (
                GetTree().Root.GetChildCount() !=
                rootChildCountBeforeRepeat
            )
            {
                Fail("Repeated PurePlay route added another root child.");
                return;
            }

            bool unavailableSwitch = _switcher.SwitchTo(
                TrinityMode.ResearchGame
            );

            if (unavailableSwitch)
            {
                Fail("Unavailable ResearchGame route returned true.");
                return;
            }

            GD.Print(
                "[ModeSwitcherSmokeTest] PASS: existing or isolated " +
                "switcher routes PurePlay once, ignores duplicates, " +
                "and rejects unavailable modes safely."
            );

            CleanupAndQuit(SuccessExitCode);
        }
        catch (Exception exception)
        {
            GD.PushError(
                "[ModeSwitcherSmokeTest] Unhandled exception: " +
                exception
            );
            CleanupAndQuit(FailureExitCode);
        }
    }

    private void Fail(string message)
    {
        GD.PushError("[ModeSwitcherSmokeTest] FAIL: " + message);
        CleanupAndQuit(FailureExitCode);
    }

    private void CleanupAndQuit(int exitCode)
    {
        if (
            _switcher?.ActiveWing != null &&
            GodotObject.IsInstanceValid(_switcher.ActiveWing)
        )
        {
            _switcher.ActiveWing.QueueFree();
        }

        if (
            _createdSwitcher &&
            _switcher != null &&
            GodotObject.IsInstanceValid(_switcher)
        )
        {
            _switcher.QueueFree();
        }

        GetTree().Quit(exitCode);
    }
}

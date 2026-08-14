using Godot;
using System;

namespace NeuroModelingDemo;

public partial class Node2d : Node2D
{
    [Export] public Label? StatusLabel { get; set; }

    public override void _Ready()
    {
        try
        {
            RnnParityHarness.Verify();

            const string message =
                "RNN parity PASS\n" +
                "C# RK4 fixture matches Python reference.";

            GD.Print($"[Node2d] {message}");
            StatusLabel?.SetText(message);

            QuitIfHeadless(0);
        }
        catch (Exception exception)
        {
            string message =
                $"RNN parity FAILED\n{exception.Message}";

            GD.PushError($"[Node2d] {message}");
            StatusLabel?.SetText(message);

            QuitIfHeadless(1);
        }
    }

    private void QuitIfHeadless(int exitCode)
    {
        if (DisplayServer.GetName() == "headless")
        {
            GetTree().Quit(exitCode);
        }
    }
}

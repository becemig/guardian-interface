using Godot;
using System;

public partial class StressTester : Node
{
    public enum FailureMode { None, PowerStarvation, StructuralBuckling, PrincipleConflict }

    private void AbortHandshake(FailureMode mode, ActionState state, KineticNode module)
    {
        string errorMessage = $"[StressTester] ABORTING: {module.ModuleName} in {state.StateName}. Mode: {mode}";
        GD.PrintErr(errorMessage);

        // Proper FileAccess for Godot 4.x to append data
        string logPath = "user://research_notes/failure_log.txt";
        
        // Open file in Write mode (which overwrites by default)
        using var file = FileAccess.Open(logPath, FileAccess.ModeFlags.ReadWrite);
        
        if (file != null)
        {
            // Move to the end of the file to append
            file.Seek(file.GetLength());
            
            string logEntry = $"{System.DateTime.Now}: {errorMessage}\n";
            file.StoreString(logEntry);
        }
        else
        {
            // If the file doesn't exist yet, create it
            using var newFile = FileAccess.Open(logPath, FileAccess.ModeFlags.Write);
            newFile.StoreString($"{System.DateTime.Now}: {errorMessage}\n");
        }
    }
}

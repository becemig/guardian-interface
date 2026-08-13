using Godot;
using System;

public static class GuardianDebug
{
    public static void Atlas(
        string message,
        string origin = "GodotAtlasBridge.cs")
    {
        Print("#47D7FF", "ATLAS", origin, message);
    }

    public static void Session(
        string message,
        string origin = "SessionRecorder")
    {
        Print("#79E08A", "SESSION", origin, message);
    }

    public static void ObjectDb(
        string message,
        string origin = "ObjectDB")
    {
        Print("#FFD166", "OBJECTDB", origin, message);
    }

    public static void Warn(
        string message,
        string origin = "Guardian")
    {
        Print("#FFB86C", "WARN", origin, message);
    }

    public static void Error(
        string message,
        string origin = "Guardian")
    {
        Print("#FF6B6B", "ERROR", origin, message);
    }

    private static void Print(
        string color,
        string category,
        string origin,
        string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

        GD.PrintRich(
            $"[color={color}]" +
            $"[{timestamp}] [{category}] [{origin}] {message}" +
            "[/color]"
        );
    }
}

using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

public partial class ReplayEngine : Node
{
    private List<string> _frames;
    private int _currentFrame = 0;
    public bool IsPlaying = false;

    public void LoadReplay(string filePath)
    {
        _frames = new List<string>(File.ReadAllLines(filePath));
        _currentFrame = 0;
        GD.Print($"ReplayEngine: Loaded {_frames.Count} frames from {filePath}");
    }

    public void ScrubTo(int frameIndex)
    {
        if (frameIndex >= 0 && frameIndex < _frames.Count)
        {
            _currentFrame = frameIndex;
            PlayFrame(_frames[_currentFrame]);
        }
    }

    private void PlayFrame(string jsonFrame)
    {
        // Deserialize and push the state back into the Mirror
        var frame = JsonSerializer.Deserialize<JsonElement>(jsonFrame);
        var state = frame.GetProperty("state");

        // Broadcast to your interfaces via the Mirror
        SomaticStateMirror.UpdateMirror(
            state.GetProperty("resonanceFlow").GetSingle(),
            state.GetProperty("thermalLoad").GetSingle(),
            state.GetProperty("section").GetInt32()
        );
        
        // This makes the UI pulse exactly as it did when the data was recorded
    }
}

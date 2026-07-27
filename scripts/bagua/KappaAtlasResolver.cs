// KappaAtlasResolver.cs -- Guardian Interface
// Bridges live kappa heatmap to Bagua Atlas VOL lookup.
// Setup: Add as AutoLoad res://scripts/bagua/KappaAtlasResolver.cs
//
using Godot;
using System;

public partial class KappaAtlasResolver : Node
{
    public static KappaAtlasResolver Instance { get; private set; }
    [Export] public Label3D VolLabel;
    [Export] public float KappaThreshold = 0.3f;
    [Export] public bool VerboseLog = false;

    [Signal] public delegate void VolResolvedEventHandler(string volId, string l1, string l2, string l3, string channel, string element, int jointIdx);
    public string CurrentVolId { get; private set; } = "VOL-000";
    public string CurrentChannel { get; private set; } = "";
    public string CurrentElement { get; private set; } = "";
    public int DominantJointIdx { get; private set; } = 0;
    public float DominantKappa { get; private set; } = 0f;
    private static readonly string[] JointZoneNames = { "R_ankle","R_knee","R_hip","R_shoulder","R_elbow","R_wrist","L_wrist","L_elbow","L_shoulder","L_hip","L_knee","L_ankle" };
    private static readonly string[] JointToL1Trigram = { "Kan","Kan","Kun","Qian","Li","Zhen","Zhen","Li","Qian","Kun","Kan","Kan" };
    private static readonly string[] TrigramOrder = { "Qian","Kun","Zhen","Xun","Kan","Li","Gen","Dui" };
    private static readonly System.Collections.Generic.Dictionary<string,string> ChannelToL2 = new(StringComparer.OrdinalIgnoreCase) { {"LU","Dui"},{"LI","Qian"},{"ST","Kun"},{"SP","Gen"},{"HT","Li"},{"SI","Zhen"},{"BL","Kan"},{"KD","Xun"},{"GB","Zhen"},{"LR","Xun"},{"PC","Li"},{"TB","Qian"} };
    private static readonly System.Collections.Generic.Dictionary<string,string> ElementToL3 = new(StringComparer.OrdinalIgnoreCase) { {"Wood","Zhen"},{"Fire","Li"},{"Earth","Kun"},{"Metal","Dui"},{"Water","Kan"} };
    public override void _Ready() {
        if (Instance != null) { QueueFree(); return; }
        Instance = this;
        var client = GetNodeOrNull<Node>("/root/BaguaPhysicsClient");
        if (client == null) { GD.PrintErr("[KappaAtlasResolver] BaguaPhysicsClient not found."); return; }
        client.Connect("BaguaFrameReceived", Callable.From<BaguaFrameGodot>(OnFrameReceived));
        GD.Print("[KappaAtlasResolver] Subscribed to BaguaFrameReceived.");
        if (VolLabel == null) { var n = GetTree().Root.FindChild("VolLabel", true, false); if (n != null) VolLabel = n as Label3D; GD.Print("[KappaAtlasResolver] VolLabel " + (VolLabel != null ? "found" : "NOT found")); }
        if (VolLabel != null) VolLabel.Text = "VOL-000";
    }
    public override void _ExitTree() { if (Instance == this) Instance = null; }
    private void OnFrameReceived(BaguaFrameGodot wrapper) {
        var frame = wrapper.Data;
        if (frame?.Joints == null || frame.Joints.Count == 0) return;
        int bestIdx = 0; float bestKappa = 0f;
        for (int i = 0; i < frame.Joints.Count; i++) { if (frame.Joints[i].Kappa > bestKappa) { bestKappa = frame.Joints[i].Kappa; bestIdx = i; } }
        DominantJointIdx = bestIdx; DominantKappa = bestKappa;
        if (bestKappa < KappaThreshold) return;
        string l1 = bestIdx < JointToL1Trigram.Length ? JointToL1Trigram[bestIdx] : "Qian";
        string chanCode = frame.Channel?.DominantChannel ?? "";
        string l2 = ChannelToL2.TryGetValue(chanCode, out var c2) ? c2 : l1;
        string element = frame.Channel?.DominantElement ?? "";
        string l3 = ElementToL3.TryGetValue(element, out var e3) ? e3 : l1;
        int volNum = Math.Max(Array.IndexOf(TrigramOrder,l1),0)*64 + Math.Max(Array.IndexOf(TrigramOrder,l2),0)*8 + Math.Max(Array.IndexOf(TrigramOrder,l3),0);
        string volId = $"VOL-{volNum:D3}";
        CurrentVolId = volId; CurrentChannel = chanCode; CurrentElement = element;
        string zone = bestIdx < JointZoneNames.Length ? JointZoneNames[bestIdx] : "?";
        if (VolLabel != null) VolLabel.Text = $"{volId}\n{chanCode} | {element}\n{zone} k={bestKappa:F2}";
        if (VerboseLog) GD.Print($"[KappaAtlasResolver] {volId} | {l1}.{l2}.{l3} | joint={zone} kappa={bestKappa:F3}");
        EmitSignal(SignalName.VolResolved, volId, l1, l2, l3, chanCode, element, bestIdx);
    }
}

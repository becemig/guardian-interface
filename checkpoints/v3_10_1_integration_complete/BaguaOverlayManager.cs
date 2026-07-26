// BaguaOverlayManager.cs
// Guardian Interface -- Bagua Physics Integration
// Receives BaguaFrame and drives Godot scene nodes.
// Wire BaguaPhysicsClient.BaguaFrameReceived -> OnBaguaFrameReceived
// Godot 4.6.2 Mono / .NET 8

using Godot;
using System.Collections.Generic;


[GlobalClass]
public partial class BaguaOverlayManager : Node
{
    [Export] public Node3D[]         JointMarkers   = new Node3D[12];
    [Export] public Node3D[]         IcrArrows      = new Node3D[6];
    [Export] public Label3D[]        ElementLabels  = new Label3D[5];
    [Export] public Label            BaGangPattern;
    [Export] public Label            BaGangConfidence;
    [Export] public ProgressBar      YinYangBar;
    [Export] public Node3D[]         MechStressNodes = new Node3D[12];
    [Export] public Label            NeuroReceptorLabel;
    [Export] public Label            NeuroAutonomicLabel;
    [Export] public ProgressBar      ProprioceptBar;
    [Export] public Label            ChannelDominantLabel;
    [Export] public Label            ChannelYinYangLabel;

    [Export] public bool  ApplyJoints   = true;
    [Export] public bool  ApplyIcr      = true;
    [Export] public bool  ApplyBaGang   = true;
    [Export] public bool  ApplyMech     = true;
    [Export] public bool  ApplyNeuro    = true;
    [Export] public bool  ApplyChannel  = true;
    [Export] public bool  ApplyElements = true;
    [Export] public float PositionScale = 1.0f;

    private static readonly Dictionary<string,Color> ChannelColors = new() {
        {"LU",new Color("#BCE2E7")},{"LI",new Color("#FFC553")},
        {"ST",new Color("#E8AF34")},{"SP",new Color("#FFC553")},
        {"HT",new Color("#DD6974")},{"SI",new Color("#A13544")},
        {"BL",new Color("#5591C7")},{"KD",new Color("#006494")},
        {"PC",new Color("#D163A7")},{"TW",new Color("#A86FDF")},
        {"GB",new Color("#6DAA45")},{"LV",new Color("#437A22")},
    };
    private static readonly Dictionary<string,Color> ElementColors = new() {
        {"Wood",new Color("#6DAA45")},{"Fire",new Color("#DD6974")},
        {"Earth",new Color("#E8AF34")},{"Metal",new Color("#BCE2E7")},
        {"Water",new Color("#5591C7")},
    };
    private static readonly string[] ElementOrder = {"Wood","Fire","Earth","Metal","Water"};
    private static readonly int[]    IcrJointIdx  = {3,4,5,6,7,8};

    public override void _Ready()
    {
        var client = GetNode<BaguaPhysicsClient>("/root/BaguaPhysicsClient");
        if (client != null)
            client.BaguaFrameReceived += OnBaguaFrameReceived;
        else
            GD.PrintErr("[BaguaOverlayManager] BaguaPhysicsClient AutoLoad not found");
    }

    public void OnBaguaFrameReceived(BaguaFrameGodot wrapper)
    {
        var f = wrapper.Data;
        if (f == null) return;
        if (ApplyJoints)   _ApplyJoints(f);
        if (ApplyIcr)      _ApplyIcr(f);
        if (ApplyElements) _ApplyElements(f);
        if (ApplyBaGang)   _ApplyBaGang(f);
        if (ApplyMech)     _ApplyMech(f);
        if (ApplyNeuro)    _ApplyNeuro(f);
        if (ApplyChannel)  _ApplyChannel(f);
    }

    private void _ApplyJoints(BaguaFrame f)
    {
        for (int i=0; i<f.Joints.Count && i<JointMarkers.Length; i++) {
            var m = JointMarkers[i]; if (m==null) continue;
            m.GlobalPosition = f.Joints[i].ToVector3() * PositionScale;
            if (m is MeshInstance3D mesh) {
                var mat = mesh.GetActiveMaterial(0) as StandardMaterial3D;
                if (mat != null) {
                    var col = f.Joints[i].ToColor();
                    col.A = 0.4f + f.Joints[i].A * 0.6f;
                    mat.AlbedoColor = col;
                    mat.EmissionEnabled = true;
                    mat.Emission = col * f.Joints[i].A;
                }
            }
        }
    }

    private void _ApplyIcr(BaguaFrame f)
    {
        for (int i=0; i<IcrJointIdx.Length && i<IcrArrows.Length; i++) {
            var arrow = IcrArrows[i]; if (arrow==null) continue;
            int ji = IcrJointIdx[i]; if (ji>=f.Joints.Count) continue;
            arrow.GlobalPosition = f.Joints[ji].ToVector3() * PositionScale;
            var icr = f.Joints[ji].Icr;
            if (icr.Valid) {
                var dir = icr.CentripVec.Normalized();
                if (dir.LengthSquared() > 0.001f)
                    arrow.GlobalBasis = Basis.LookingAt(dir, Vector3.Up);
                arrow.Scale = Vector3.One * Mathf.Clamp(icr.Lam * 1.1f, 0.1f, 2.0f);
            }
        }
    }

    private void _ApplyElements(BaguaFrame f)
    {
        for (int i=0; i<ElementOrder.Length && i<ElementLabels.Length; i++) {
            var lbl = ElementLabels[i]; if (lbl==null) continue;
            var name = ElementOrder[i];
            lbl.Text = name;
            if (ElementColors.TryGetValue(name, out var col)) {
                float score = f.FiveElement.GetScore(name);
                col.A = 0.3f + score * 0.7f;
                lbl.Modulate = col;
                lbl.FontSize = name==f.FiveElement.Dominant ? 18 : 12;
            }
        }
    }

    private void _ApplyBaGang(BaguaFrame f)
    {
        var bg = f.BaGang;
        if (BaGangPattern != null) {
            BaGangPattern.Text = bg.Pattern;
            BaGangPattern.AddThemeColorOverride("font_color",
                bg.IsYang ? new Color("#DD6974") : new Color("#5591C7"));
        }
        if (BaGangConfidence != null)
            BaGangConfidence.Text = $"Confidence: {bg.Confidence*100:F0}%";
        if (YinYangBar != null)
            YinYangBar.Value = bg.Yang * 100.0;
    }

    private void _ApplyMech(BaguaFrame f)
    {
        var mech = f.Mech;
        for (int i=0; i<mech.Stress.Length && i<MechStressNodes.Length; i++) {
            var node = MechStressNodes[i]; if (node==null) continue;
            float s = mech.Stress[i];
            node.Scale = Vector3.One * (0.05f + s * 0.15f);
            node.Visible = s > 0.05f;
            if (node is MeshInstance3D mesh) {
                var mat = mesh.GetActiveMaterial(0) as StandardMaterial3D;
                if (mat != null) {
                    Color col;
                    if (i<mech.YapTaz.Length && mech.YapTaz[i])       col = new Color("#D163A7");
                    else if (i<mech.Integrin.Length && mech.Integrin[i]) col = new Color("#FFC553");
                    else col = new Color("#20808D");
                    col.A = s * 0.8f;
                    mat.AlbedoColor = col;
                    mat.EmissionEnabled = true;
                    mat.Emission = col * s * 0.5f;
                }
            }
        }
    }

    private void _ApplyNeuro(BaguaFrame f)
    {
        var n = f.Neuro;
        if (NeuroReceptorLabel  != null) NeuroReceptorLabel.Text = n.Receptor;
        if (NeuroAutonomicLabel != null) {
            int pct = Mathf.RoundToInt(n.Autonomic * 100);
            NeuroAutonomicLabel.Text = $"{pct}% SNS / {100-pct}% PNS";
            NeuroAutonomicLabel.AddThemeColorOverride("font_color",
                n.IsSympathetic ? new Color("#DD6974") : new Color("#437A22"));
        }
        if (ProprioceptBar != null) ProprioceptBar.Value = n.Prop * 100.0;
    }

    private void _ApplyChannel(BaguaFrame f)
    {
        var ch = f.Channel;
        if (ChannelDominantLabel != null) {
            ChannelDominantLabel.Text = $"{ch.DominantChannel} ({ch.DominantElement})";
            if (ChannelColors.TryGetValue(ch.DominantChannel, out var col))
                ChannelDominantLabel.AddThemeColorOverride("font_color", col);
        }
        if (ChannelYinYangLabel != null)
            ChannelYinYangLabel.Text = $"Yin {ch.YinTotal*100:F0}%  Yang {ch.YangTotal*100:F0}%";
    }
}

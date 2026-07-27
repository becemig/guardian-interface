// JingJinOverlay.cs -- Guardian Interface
// Renders 12 TCM Jing Jin (sinew meridian) pathways on skeleton.
// Driven by VolResolved signal from KappaAtlasResolver.
//
using Godot;
using System;
using System.Collections.Generic;

public partial class JingJinOverlay : Node3D
{
    // Joint world positions updated each frame from BaguaFrameReceived
    private Vector3[] _jointPos = new Vector3[12];
    private float[] _jointAmp = new float[12];
    private string _activeChannel = "";
    private float _activeAmp = 0f;
    // Each entry: channel code -> ordered list of joint indices forming the pathway
    // Joint index: 0=R_ankle,1=R_knee,2=R_hip,3=R_shoulder,4=R_elbow,5=R_wrist
    //              6=L_wrist,7=L_elbow,8=L_shoulder,9=L_hip,10=L_knee,11=L_ankle
    private static readonly Dictionary<string, int[]> JingJinPaths =
        new Dictionary<string, int[]>
    {
        { "BL", new[]{0,1,2,3} },    // Zu Tai Yang: ankle-knee-hip-shoulder (Sup Back Line)
        { "KD", new[]{11,10,9,8} },  // Zu Shao Yin: L ankle-knee-hip-shoulder (Deep Front)
        { "GB", new[]{0,1,2,9,10,11} }, // Zu Shao Yang: R ankle up to L ankle (Lateral)
        { "LR", new[]{11,10,9} },    // Zu Jue Yin: L ankle-knee-hip (Deep Front inner)
        { "ST", new[]{0,1,2,3} },    // Zu Yang Ming: ankle-knee-hip-shoulder (Sup Front)
        { "SP", new[]{11,10,9,8} },  // Zu Tai Yin: L ankle-knee-hip-shoulder
        { "SI", new[]{3,4,5} },      // Shou Tai Yang: R shoulder-elbow-wrist (Back Func)
        { "HT", new[]{8,7,6} },      // Shou Shao Yin: L shoulder-elbow-wrist (inner)
        { "TB", new[]{3,4,5} },      // Shou Shao Yang: R shoulder-elbow-wrist (lateral)
        { "LI", new[]{8,7,6} },      // Shou Yang Ming: L shoulder-elbow-wrist
        { "LU", new[]{3,4,5,8,7,6} }, // Shou Tai Yin: R+L arm lines (chest-to-wrist)
        { "PC", new[]{3,4,5} },      // Shou Jue Yin: R shoulder-elbow-wrist (deep front arm)
    };
    // Base hue per channel (HSV-derived, amplitude scales Value)
    private static readonly Dictionary<string, Color> ChannelColors =
        new Dictionary<string, Color>
    {
        { "BL", new Color(0.2f, 0.5f, 1.0f) },   // Blue -- Water
        { "KD", new Color(0.1f, 0.3f, 0.9f) },   // Dark blue -- Water
        { "GB", new Color(0.2f, 0.9f, 0.3f) },   // Green -- Wood
        { "LR", new Color(0.1f, 0.8f, 0.2f) },   // Dark green -- Wood
        { "ST", new Color(0.9f, 0.8f, 0.1f) },   // Yellow -- Earth
        { "SP", new Color(0.8f, 0.7f, 0.2f) },   // Dark yellow -- Earth
        { "SI", new Color(1.0f, 0.3f, 0.1f) },   // Red -- Fire
        { "HT", new Color(0.9f, 0.1f, 0.1f) },   // Dark red -- Fire
        { "TB", new Color(0.9f, 0.5f, 0.1f) },   // Orange -- Fire
        { "LI", new Color(0.95f, 0.9f, 0.7f) },  // White-gold -- Metal
        { "LU", new Color(1.0f, 1.0f, 0.9f) },   // White -- Metal
        { "PC", new Color(1.0f, 0.2f, 0.5f) },   // Crimson -- Fire
    };
    // One ImmediateMesh per channel for independent color/alpha control
    private Dictionary<string, MeshInstance3D> _meshes = new();
    private Dictionary<string, ImmediateMesh> _imMeshes = new();

    public override void _Ready()
    {
        foreach (var ch in JingJinPaths.Keys)
        {
            var im = new ImmediateMesh();
            var mi = new MeshInstance3D();
            mi.Mesh = im;
            var mat = new StandardMaterial3D();
            mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            mat.VertexColorUseAsAlbedo = true;
            mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            mi.MaterialOverride = mat;
            AddChild(mi);
            _meshes[ch] = mi;
            _imMeshes[ch] = im;
        }
        // Subscribe to frame data for joint positions
        var client = GetTree().Root.FindChild("BaguaPhysicsClient", true, false);
        if (client != null)
            client.Connect("BaguaFrameReceived", new Callable(this, nameof(OnFrame)));
        else
            GD.PrintErr("[JingJinOverlay] BaguaPhysicsClient not found");
        // Subscribe to VOL resolved for active channel
        var resolver = GetTree().Root.FindChild("KappaAtlasResolver", true, false);
        if (resolver != null)
            resolver.Connect("VolResolved", new Callable(this, nameof(OnVolResolved)));
        else
            GD.PrintErr("[JingJinOverlay] KappaAtlasResolver not found");
        GD.Print("[JingJinOverlay] Ready -- 12 jing jin pathways initialized");
    }
    private void OnFrame(Godot.Collections.Dictionary frame)
    {
        if (!frame.ContainsKey("joints")) return;
        var joints = frame["joints"].AsGodotArray();
        for (int i = 0; i < Math.Min(joints.Count, 12); i++)
        {
            var j = joints[i].AsGodotDictionary();
            float x = j.ContainsKey("x") ? (float)j["x"].AsDouble() : 0f;
            float y = j.ContainsKey("y") ? (float)j["y"].AsDouble() : 0f;
            float z = j.ContainsKey("z") ? (float)j["z"].AsDouble() : 0f;
            float a = j.ContainsKey("A") ? (float)j["A"].AsDouble() : 0f;
            _jointPos[i] = new Vector3(x, y, z);
            _jointAmp[i] = a;
        }
        DrawAllLines();
    }
    private void OnVolResolved(int volId, int l1, int l2, int l3,
        string channel, string element, int jointIdx)
    {
        _activeChannel = channel;
        _activeAmp = _jointAmp.Length > jointIdx ? _jointAmp[jointIdx] : 0f;
    }

    private void DrawAllLines()
    {
        foreach (var kvp in JingJinPaths)
        {
            string ch = kvp.Key;
            int[] path = kvp.Value;
            var im = _imMeshes[ch];
            im.ClearSurfaces();
            if (path.Length < 2) continue;
            bool isActive = ch == _activeChannel;
            Color baseCol = ChannelColors.ContainsKey(ch) ? ChannelColors[ch] : Colors.White;
            // Amplitude drives alpha: active channel full brightness,
            // others dimmed to 15% so all 12 lines stay visible
            float alpha = isActive ? Mathf.Clamp(_activeAmp * 1.2f, 0.5f, 1.0f) : 0.15f;
            Color col = new Color(baseCol.R, baseCol.G, baseCol.B, alpha);
            im.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
            foreach (int idx in path)
            {
                if (idx < 0 || idx >= _jointPos.Length) continue;
                im.SurfaceSetColor(col);
                im.SurfaceAddVertex(_jointPos[idx]);
            }
            im.SurfaceEnd();
        }
    }
}

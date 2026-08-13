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

    // Avatar-local reference anchors for the synthetic preview only.
    // Coordinates are visual scaffolding, not tracked anatomy or measurement.
    // Positive Z positions the ribbons slightly in front of the avatar mesh.
    // Visual alignment offset for the synthetic reference scaffold only.
    // Keeps synthetic paths registered to the displayed avatar, not telemetry.
    private static readonly Vector3 SyntheticReferenceOffset =
        new Vector3(0.0f, -0.80f, 0.0f);

    // Compress the visual scaffold laterally to match the displayed avatar.
    // Synthetic preview only; this does not alter telemetry or skeleton mapping.
    private const float SyntheticReferenceXScale = 1.00f;

    private static Vector3 CalibrateSyntheticReference(Vector3 point)
    {
        return new Vector3(
            point.X * SyntheticReferenceXScale,
            point.Y,
            point.Z
        ) + SyntheticReferenceOffset;
    }

    private static readonly Vector3[] SyntheticReferenceJoints = new Vector3[]
    {
        new Vector3( 0.22f, 0.06f, 0.26f), // 0  R ankle
        new Vector3( 0.25f, 0.52f, 0.26f), // 1  R knee
        new Vector3( 0.20f, 1.03f, 0.26f), // 2  R hip
        new Vector3( 0.18f, 1.62f, 0.20f), // 3  R shoulder
        new Vector3( 0.47f, 1.42f, 0.20f), // 4  R elbow
        new Vector3( 0.67f, 1.23f, 0.20f), // 5  R wrist
        new Vector3(-0.67f, 1.23f, 0.20f), // 6  L wrist
        new Vector3(-0.47f, 1.42f, 0.20f), // 7  L elbow
        new Vector3(-0.18f, 1.62f, 0.20f), // 8  L shoulder
        new Vector3(-0.20f, 1.03f, 0.26f), // 9  L hip
        new Vector3(-0.25f, 0.52f, 0.26f), // 10 L knee
        new Vector3(-0.22f, 0.06f, 0.26f), // 11 L ankle
    };
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
    // Naturalistic element palette -- desaturated, clinical
    private static readonly Dictionary<string, Color> ChannelColors =
        new Dictionary<string, Color>
    {
        { "BL", new Color(0.165f, 0.290f, 0.368f) },  // deep ocean -- Water
        { "KD", new Color(0.120f, 0.235f, 0.320f) },  // dark slate blue -- Water
        { "GB", new Color(0.240f, 0.353f, 0.243f) },  // forest sage -- Wood
        { "LR", new Color(0.180f, 0.290f, 0.185f) },  // dark moss -- Wood
        { "ST", new Color(0.478f, 0.392f, 0.208f) },  // warm ochre -- Earth
        { "SP", new Color(0.400f, 0.330f, 0.175f) },  // dark amber -- Earth
        { "SI", new Color(0.478f, 0.250f, 0.250f) },  // muted ember -- Fire
        { "HT", new Color(0.420f, 0.190f, 0.190f) },  // deep rose -- Fire
        { "TB", new Color(0.520f, 0.310f, 0.210f) },  // burnt sienna -- Fire
        { "LI", new Color(0.680f, 0.670f, 0.640f) },  // cool silver -- Metal
        { "LU", new Color(0.750f, 0.745f, 0.720f) },  // pale silver-white -- Metal
        { "PC", new Color(0.500f, 0.220f, 0.280f) },  // muted crimson -- Fire
    };
    // One ImmediateMesh per channel for independent color/alpha control
    private Dictionary<string, MeshInstance3D> _meshes = new();
    private Dictionary<string, ImmediateMesh> _imMeshes = new();
    // Smoothed alpha and thickness per channel for lerp transitions
    private Dictionary<string, float> _alphaSmooth = new();
    private Dictionary<string, float> _thickSmooth = new();

    // Preview-only visual state. This represents a software reference model,
    // not EMG, tendon force, or clinical assessment.
    private bool _syntheticActivationActive;
    private float _syntheticLeftLoad = 50.0f;
    private float _syntheticRightLoad = 50.0f;
    private double _syntheticElapsedSeconds;

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

            // Temporary high-visibility rendering for synthetic reference paths.
            mat.NoDepthTest = true;
            mat.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
            mat.EmissionEnabled = true;
            mat.Emission = Colors.White;
            mat.EmissionEnergyMultiplier = 2.5f;

            mi.MaterialOverride = mat;
            mi.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
            AddChild(mi);
            _meshes[ch] = mi;
            _imMeshes[ch] = im;
        }
        // Subscribe to typed frame data for joint positions.
        var client = GetNodeOrNull<BaguaPhysicsClient>("/root/BaguaPhysicsClient");
        if (client != null)
            client.BaguaFrameReceived += OnFrame;
        else
            GD.PrintErr("[JingJinOverlay] BaguaPhysicsClient not found");
        // Subscribe to VOL resolved for active channel
        var resolver = GetTree().Root.FindChild("KappaAtlasResolver", true, false);
        if (resolver != null)
            resolver.Connect("VolResolved", new Callable(this, nameof(OnVolResolved)));
        else
            GD.PrintErr("[JingJinOverlay] KappaAtlasResolver not found");
        GD.Print(
            $"[JingJinOverlay] Ready -- 12 jing jin pathways initialized | path={GetPath()}"
        );
    }
    private void OnFrame(BaguaFrameGodot wrapper)
    {
        var joints = wrapper.Data?.Joints;
        if (joints == null || joints.Count < 2)
            return;

        for (int i = 0; i < Math.Min(joints.Count, 12); i++)
        {
            _jointPos[i] = joints[i].ToVector3();
            _jointAmp[i] = joints[i].A;
        }

        DrawAllLines();
    }
    private void OnVolResolved(int volId, int l1, int l2, int l3,
        string channel, string element, int jointIdx)
    {
        _activeChannel = channel;
        _activeAmp = _jointAmp.Length > jointIdx ? _jointAmp[jointIdx] : 0f;
    }

    public void SetSyntheticLoad(
        float leftLoadPercent,
        float rightLoadPercent,
        double elapsedSeconds)
    {
        _syntheticActivationActive = true;
        _syntheticLeftLoad = Mathf.Clamp(leftLoadPercent, 0.0f, 100.0f);
        _syntheticRightLoad = Mathf.Clamp(rightLoadPercent, 0.0f, 100.0f);
        _syntheticElapsedSeconds = elapsedSeconds;
        DrawAllLines();
    }

    public void ClearSyntheticLoad()
    {
        _syntheticActivationActive = false;
        DrawAllLines();
    }

    private float SyntheticPathActivation(string channel)
    {
        float left = _syntheticLeftLoad / 100.0f;
        float right = _syntheticRightLoad / 100.0f;
        float bilateral = (left + right) * 0.5f;
        float asymmetry = Mathf.Abs(left - right);

        return channel switch
        {
            // Right lower-limb reference pathways.
            "BL" => Mathf.Lerp(0.10f, 1.00f, right),
            "ST" => Mathf.Lerp(0.12f, 1.00f, right),

            // Left lower-limb reference pathways.
            "KD" => Mathf.Lerp(0.10f, 1.00f, left),
            "LR" => Mathf.Lerp(0.12f, 0.92f, left),
            "SP" => Mathf.Lerp(0.10f, 0.78f, left),

            // Lateral stabilizing pathway: responds to overall load and
            // slightly more to left/right imbalance.
            "GB" => Mathf.Clamp(
                0.18f + bilateral * 0.42f + asymmetry * 0.40f,
                0.0f,
                1.0f
            ),

            // Upper-body paths remain a dim contextual overlay in this
            // lower-limb synthetic weight-shift reference.
            _ => 0.06f
        };
    }

    // Camera reference for billboard quads
    private Camera3D _cam;

    private void DrawAllLines()
    {
        if (_cam == null) _cam = GetViewport().GetCamera3D();
        foreach (var kvp in JingJinPaths)
        {
            string ch = kvp.Key;
            int[] path = kvp.Value;
            var im = _imMeshes[ch];
            im.ClearSurfaces();
            if (path.Length < 2) continue;
            bool isActive = ch == _activeChannel;
            Color baseCol = ChannelColors.ContainsKey(ch) ? ChannelColors[ch] : Colors.White;

            float pathwayActivation;
            if (_syntheticActivationActive)
                pathwayActivation = SyntheticPathActivation(ch);
            else
                pathwayActivation = isActive
                    ? Mathf.Sqrt(Mathf.Clamp(_activeAmp, 0.0f, 1.0f))
                    : 0.0f;

            float alphaTarget = _syntheticActivationActive
                ? Mathf.Lerp(0.10f, 0.94f, pathwayActivation)
                : isActive
                    ? Mathf.Lerp(0.55f, 0.88f, pathwayActivation)
                    : 0.08f;

            float thickTarget = _syntheticActivationActive
                ? Mathf.Lerp(0.018f, 0.100f, pathwayActivation)
                : isActive
                    ? Mathf.Lerp(0.018f, 0.055f, pathwayActivation)
                    : 0.006f;
            // Lerp toward target for smooth transitions
            if (!_alphaSmooth.ContainsKey(ch)) _alphaSmooth[ch] = 0.08f;
            if (!_thickSmooth.ContainsKey(ch)) _thickSmooth[ch] = 0.006f;
            _alphaSmooth[ch] = Mathf.Lerp(_alphaSmooth[ch], alphaTarget, 0.10f);
            _thickSmooth[ch] = Mathf.Lerp(_thickSmooth[ch], thickTarget, 0.10f);
            float alpha = _alphaSmooth[ch];
            float thick = _thickSmooth[ch];
            Color visualBase = _syntheticActivationActive
                ? baseCol.Lerp(Colors.White, pathwayActivation * 0.32f)
                : baseCol;

            float pulse = _syntheticActivationActive
                ? 0.94f + 0.06f * Mathf.Sin((float)_syntheticElapsedSeconds * 2.0f)
                : 1.0f;

            Color col = new Color(
                visualBase.R * pulse,
                visualBase.G * pulse,
                visualBase.B * pulse,
                alpha
            );
            im.SurfaceBegin(Mesh.PrimitiveType.Triangles);
            for (int s = 0; s < path.Length - 1; s++)
            {
                int ia = path[s]; int ib = path[s + 1];
                if (ia < 0 || ia >= _jointPos.Length) continue;
                if (ib < 0 || ib >= _jointPos.Length) continue;
                Vector3 a = _syntheticActivationActive
                    ? CalibrateSyntheticReference(SyntheticReferenceJoints[ia])
                    : _jointPos[ia];

                Vector3 b = _syntheticActivationActive
                    ? CalibrateSyntheticReference(SyntheticReferenceJoints[ib])
                    : _jointPos[ib];
                // Billboard: perpendicular to segment in camera-facing plane
                Vector3 seg = (b - a).Normalized();
                Vector3 toCam = _cam != null
                    ? ((_cam.GlobalPosition - a).Normalized()) : Vector3.Up;
                Vector3 perp = seg.Cross(toCam).Normalized() * (thick * 0.5f);
                // Quad: two triangles
                im.SurfaceSetColor(col); im.SurfaceAddVertex(a - perp);
                im.SurfaceSetColor(col); im.SurfaceAddVertex(a + perp);
                im.SurfaceSetColor(col); im.SurfaceAddVertex(b + perp);
                im.SurfaceSetColor(col); im.SurfaceAddVertex(a - perp);
                im.SurfaceSetColor(col); im.SurfaceAddVertex(b + perp);
                im.SurfaceSetColor(col); im.SurfaceAddVertex(b - perp);
            }
            im.SurfaceEnd();
        }
    }
}

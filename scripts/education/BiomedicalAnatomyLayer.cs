using Godot;
using System.Collections.Generic;

public enum AnatomyStudyRegion
{
    All,
    AxialSkeleton,
    UpperLimbs,
    LowerLimbs
}

public partial class BiomedicalAnatomyLayer : Node3D
{
    private sealed class LabelAnchor
    {
        public string RigBoneName { get; }
        public string FallbackLabel { get; }
        public Label3D Label { get; set; }
        public int BoneIndex { get; set; } = -1;

        public LabelAnchor(string rigBoneName, string fallbackLabel)
        {
            RigBoneName = rigBoneName;
            FallbackLabel = fallbackLabel;
        }
    }

    private readonly List<LabelAnchor> _anchors = new()
    {
        new("spine",       "Axial spine\nroot proxy"),
        new("shoulder.L",  "L shoulder-girdle\nanchor"),
        new("shoulder.R",  "R shoulder-girdle\nanchor"),
        new("upper_arm.L", "L humeral\nsegment"),
        new("upper_arm.R", "R humeral\nsegment"),
        new("forearm.L",   "L radius-ulna\nsegment"),
        new("forearm.R",   "R radius-ulna\nsegment"),
        new("thigh.L",     "L femoral\nsegment"),
        new("thigh.R",     "R femoral\nsegment"),
        new("shin.L",      "L tibial-fibular\nsegment"),
        new("shin.R",      "R tibial-fibular\nsegment"),
        new("foot.L",      "L ankle-foot\nsegment"),
        new("foot.R",      "R ankle-foot\nsegment"),
    };

    private Skeleton3D _skeleton;
    private bool _anchorsResolved;
    private bool _studyMode;
    private bool _focusMode;
    private AnatomyStudyRegion _studyRegion = AnatomyStudyRegion.All;

    private static readonly HashSet<string> StudyModeBones = new()
    {
        "spine",
        "shoulder.L",
        "shoulder.R",
        "upper_arm.L",
        "upper_arm.R",
        "thigh.L",
        "thigh.R",
        "shin.L",
        "shin.R"
    };

    public override void _Ready()
    {
        Visible = false;
        CallDeferred(nameof(ResolveRigAnchors));
    }

    public void SetLayerVisible(bool isVisible)
    {
        Visible = isVisible;
        ApplyLabelVisibility();
    }

    public void SetStudyMode(bool isStudyMode)
    {
        _studyMode = isStudyMode;
        ApplyLabelVisibility();
    }

    public void SetFocusMode(bool focusMode)
    {
        _focusMode = focusMode;
        ApplyLabelVisibility();
    }

    public void SetStudyRegion(AnatomyStudyRegion region)
    {
        _studyRegion = region;
        ApplyLabelVisibility();
    }

    private bool IsInStudyRegion(string rigBoneName)
    {
        return _studyRegion switch
        {
            AnatomyStudyRegion.All => true,

            AnatomyStudyRegion.AxialSkeleton =>
                rigBoneName == "spine",

            AnatomyStudyRegion.UpperLimbs =>
                rigBoneName.StartsWith("shoulder.") ||
                rigBoneName.StartsWith("upper_arm."),

            AnatomyStudyRegion.LowerLimbs =>
                rigBoneName.StartsWith("thigh.") ||
                rigBoneName.StartsWith("shin."),

            _ => true
        };
    }

    private Vector3 GetStudyLabelOffset(string rigBoneName)
    {
        return rigBoneName switch
        {
            "spine" => new Vector3(0.0f, 0.28f, 0.0f),

            "shoulder.L" => new Vector3(-0.72f, 0.24f, 0.0f),
            "shoulder.R" => new Vector3( 0.72f, 0.24f, 0.0f),

            "upper_arm.L" => new Vector3(-0.88f, 0.05f, 0.0f),
            "upper_arm.R" => new Vector3( 0.88f, 0.05f, 0.0f),

            "thigh.L" => new Vector3(-0.82f, 0.12f, 0.0f),
            "thigh.R" => new Vector3( 0.82f, 0.12f, 0.0f),

            "shin.L" => new Vector3(-0.78f, -0.10f, 0.0f),
            "shin.R" => new Vector3( 0.78f, -0.10f, 0.0f),

            _ => Vector3.Zero
        };
    }

    private void ApplyLabelVisibility()
    {
        foreach (LabelAnchor anchor in _anchors)
        {
            if (anchor.Label == null)
                continue;

            bool focusedLabelLayout = _studyMode || _focusMode;

            anchor.Label.Visible =
                !focusedLabelLayout ||
                (StudyModeBones.Contains(anchor.RigBoneName) &&
                 (!_studyMode || IsInStudyRegion(anchor.RigBoneName)));
        }
    }

    private Skeleton3D FindSkeleton(Node root)
    {
        if (root is Skeleton3D skeleton)
            return skeleton;

        foreach (Node child in root.GetChildren())
        {
            Skeleton3D found = FindSkeleton(child);
            if (found != null)
                return found;
        }

        return null;
    }

    private void ResolveRigAnchors()
    {
        _skeleton = FindSkeleton(GetParent());

        if (_skeleton == null)
        {
            GD.PushWarning(
                "[BiomedicalAnatomyLayer] Skeleton3D unavailable; labels disabled."
            );
            return;
        }

        foreach (LabelAnchor anchor in _anchors)
        {
            anchor.BoneIndex = _skeleton.FindBone(anchor.RigBoneName);

            if (anchor.BoneIndex < 0)
            {
                GD.PushWarning(
                    $"[BiomedicalAnatomyLayer] Missing rig bone: {anchor.RigBoneName}"
                );
                continue;
            }

            string labelText = anchor.FallbackLabel;

            if (AnatomicalCrosswalk.TryGet(
                anchor.RigBoneName,
                out AnatomicalCrosswalkEntry entry))
            {
                labelText = entry.EducationalLabel;
            }

            var label = new Label3D
            {
                Name = $"Label_{anchor.RigBoneName}",
                Text = labelText,
                FontSize = 28,
                OutlineSize = 6,
                Modulate = new Color(0.38f, 0.90f, 1.00f, 0.96f),
                Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
                PixelSize = 0.0022f,
                NoDepthTest = true
            };

            label.Position = Vector3.Zero;
            AddChild(label);
            anchor.Label = label;
        }

        _anchorsResolved = true;
        ApplyLabelVisibility();

        GD.Print(
            "[BiomedicalAnatomyLayer] Conservative rig-proxy labels resolved."
        );
    }

    public override void _Process(double delta)
    {
        if (!Visible || !_anchorsResolved)
            return;

        foreach (LabelAnchor anchor in _anchors)
        {
            if (anchor.Label == null || anchor.BoneIndex < 0)
                continue;

            Transform3D bonePose =
                _skeleton.GetBoneGlobalPose(anchor.BoneIndex);

            Vector3 worldPoint = _skeleton.ToGlobal(bonePose.Origin);

            // Offset toward the presentation camera. This is visual registration,
            // not a claim about surface anatomy or exact landmark location.
            worldPoint += new Vector3(0.0f, 0.0f, 0.18f);

            if (_studyMode || _focusMode)
                worldPoint += GetStudyLabelOffset(anchor.RigBoneName);

            anchor.Label.Position = ToLocal(worldPoint);
        }
    }
}

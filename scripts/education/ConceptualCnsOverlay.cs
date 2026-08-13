using Godot;
using System.Collections.Generic;

public partial class ConceptualCnsOverlay : Node3D
{
    private readonly ImmediateMesh _pathMesh = new();
    private readonly MeshInstance3D _pathInstance = new();
    private readonly Color _pathColor = new(0.67f, 0.42f, 0.95f, 0.86f);

    private Camera3D _camera;

    private static readonly Vector3 SyntheticReferenceOffset =
        new(0.0f, -0.80f, 0.0f);

    public override void _Ready()
    {
        Name = "ConceptualCnsOverlay";

        var material = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            NoDepthTest = true,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
            EmissionEnabled = true,
            Emission = new Color(0.56f, 0.28f, 0.88f),
            EmissionEnergyMultiplier = 1.25f
        };

        _pathInstance.Mesh = _pathMesh;
        _pathInstance.MaterialOverride = material;
        _pathInstance.CastShadow =
            GeometryInstance3D.ShadowCastingSetting.Off;

        AddChild(_pathInstance);

        CreateBrainMarker();
        CreateConceptualLabel(
            "Conceptual brain marker",
            ReferencePoint(new Vector3(0.0f, 2.28f, 0.10f))
        );

        CreateConceptualLabel(
            "Conceptual spinal axis",
            ReferencePoint(new Vector3(0.0f, 1.08f, 0.15f))
        );

        CreateConceptualLabel(
            "Bilateral limb-output paths",
            ReferencePoint(new Vector3(0.0f, 0.56f, 0.16f))
        );

        DrawConceptualPaths();
        Visible = false;

        GD.Print(
            "[ConceptualCnsOverlay] Ready — non-diagnostic teaching diagram."
        );
    }

    private Vector3 ReferencePoint(Vector3 point)
    {
        return point + SyntheticReferenceOffset;
    }

    private void CreateBrainMarker()
    {
        var marker = new MeshInstance3D
        {
            Name = "ConceptualBrainMarker",
            Mesh = new SphereMesh
            {
                Radius = 0.13f,
                Height = 0.26f,
                RadialSegments = 20,
                Rings = 10
            },
            Position = ReferencePoint(new Vector3(0.0f, 2.18f, 0.10f)),
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        };

        marker.MaterialOverride = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = new Color(0.72f, 0.48f, 0.98f, 0.82f),
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            EmissionEnabled = true,
            Emission = new Color(0.50f, 0.22f, 0.82f),
            EmissionEnergyMultiplier = 1.4f
        };

        AddChild(marker);
    }

    private void CreateConceptualLabel(string text, Vector3 position)
    {
        var label = new Label3D
        {
            Text = text,
            Position = position,
            FontSize = 28,
            OutlineSize = 5,
            Modulate = new Color(0.77f, 0.61f, 1.0f, 0.95f),
            PixelSize = 0.0032f,
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
            NoDepthTest = true
        };

        AddChild(label);
    }

    private void DrawConceptualPaths()
    {
        _camera = GetViewport().GetCamera3D();

        _pathMesh.ClearSurfaces();
        _pathMesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

        DrawRibbon(
            new[]
            {
                ReferencePoint(new Vector3(0.0f, 2.08f, 0.12f)),
                ReferencePoint(new Vector3(0.0f, 1.62f, 0.16f)),
                ReferencePoint(new Vector3(0.0f, 1.06f, 0.18f)),
                ReferencePoint(new Vector3(0.0f, 0.40f, 0.20f))
            },
            0.035f
        );

        DrawRibbon(
            new[]
            {
                ReferencePoint(new Vector3(0.0f, 1.58f, 0.16f)),
                ReferencePoint(new Vector3(-0.22f, 1.58f, 0.18f)),
                ReferencePoint(new Vector3(-0.50f, 1.35f, 0.20f)),
                ReferencePoint(new Vector3(-0.64f, 1.12f, 0.22f))
            },
            0.027f
        );

        DrawRibbon(
            new[]
            {
                ReferencePoint(new Vector3(0.0f, 1.58f, 0.16f)),
                ReferencePoint(new Vector3(0.22f, 1.58f, 0.18f)),
                ReferencePoint(new Vector3(0.50f, 1.35f, 0.20f)),
                ReferencePoint(new Vector3(0.64f, 1.12f, 0.22f))
            },
            0.027f
        );

        DrawRibbon(
            new[]
            {
                ReferencePoint(new Vector3(0.0f, 0.98f, 0.18f)),
                ReferencePoint(new Vector3(-0.20f, 0.92f, 0.22f)),
                ReferencePoint(new Vector3(-0.27f, 0.43f, 0.24f)),
                ReferencePoint(new Vector3(-0.24f, 0.03f, 0.25f))
            },
            0.030f
        );

        DrawRibbon(
            new[]
            {
                ReferencePoint(new Vector3(0.0f, 0.98f, 0.18f)),
                ReferencePoint(new Vector3(0.20f, 0.92f, 0.22f)),
                ReferencePoint(new Vector3(0.27f, 0.43f, 0.24f)),
                ReferencePoint(new Vector3(0.24f, 0.03f, 0.25f))
            },
            0.030f
        );

        _pathMesh.SurfaceEnd();
    }

    private void DrawRibbon(IReadOnlyList<Vector3> points, float width)
    {
        for (int index = 0; index < points.Count - 1; index++)
        {
            Vector3 start = points[index];
            Vector3 end = points[index + 1];
            Vector3 segment = (end - start).Normalized();

            Vector3 toCamera = _camera != null
                ? (_camera.GlobalPosition - start).Normalized()
                : Vector3.Forward;

            Vector3 perpendicular = segment.Cross(toCamera).Normalized()
                * width * 0.5f;

            _pathMesh.SurfaceSetColor(_pathColor);
            _pathMesh.SurfaceAddVertex(start - perpendicular);

            _pathMesh.SurfaceSetColor(_pathColor);
            _pathMesh.SurfaceAddVertex(start + perpendicular);

            _pathMesh.SurfaceSetColor(_pathColor);
            _pathMesh.SurfaceAddVertex(end + perpendicular);

            _pathMesh.SurfaceSetColor(_pathColor);
            _pathMesh.SurfaceAddVertex(start - perpendicular);

            _pathMesh.SurfaceSetColor(_pathColor);
            _pathMesh.SurfaceAddVertex(end + perpendicular);

            _pathMesh.SurfaceSetColor(_pathColor);
            _pathMesh.SurfaceAddVertex(end - perpendicular);
        }
    }
}

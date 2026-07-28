using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class BaguaJointSpheres : Node3D
{
    [Export] public NodePath AvatarScenePath = "..";
    private BaguaPhysicsClient _client;
    private List<MeshInstance3D> _spheres = new();
    private List<ShaderMaterial> _mats = new();

    private static readonly string[] JointNames = {
        "R_Ankle","R_Knee","R_Hip","R_Shoulder",
        "R_Elbow","R_Wrist","L_Wrist","L_Elbow",
        "L_Shoulder","L_Hip","L_Knee","L_Ankle"
    };

    public override void _Ready()
    {
        var sphere = new SphereMesh();
        sphere.Radius = 0.07f;
        sphere.Height = 0.14f;

        for (int i = 0; i < 12; i++)
        {
            var mi = new MeshInstance3D();
            mi.Mesh = sphere;
            mi.Name = JointNames[i];

            var mat = new ShaderMaterial();
            var shader = GD.Load<Shader>("res://assets/avatars/bagua_sphere.gdshader");
            mat.Shader = shader;
            mat.SetShaderParameter("sphere_color", new Color(0.05f, 0.12f, 0.25f, 0.0f));
            mi.MaterialOverride = mat;

            AddChild(mi);
            _spheres.Add(mi);
            _mats.Add(mat);
        }

        _client = GetNode<BaguaPhysicsClient>("/root/BaguaPhysicsClient");
        _client.BaguaFrameReceived += OnFrame;
        GD.Print("[BaguaJointSpheres] 12 spheres ready");
    }

    private void OnFrame(BaguaFrameGodot wrapper)
    {
        var joints = wrapper.Data.Joints;
        if (joints == null || joints.Count < 12) return;

        // Use hip joints (6,7) as anchor -- average their XZ as the body center
        var hipL = joints[9].ToVector3();
        var hipR = joints[2].ToVector3();
        float anchorX = (hipL.X + hipR.X) * 0.5f;
        float anchorZ = (hipL.Z + hipR.Z) * 0.5f;
        float anchorY = (hipL.Y + hipR.Y) * 0.5f;

        for (int i = 0; i < 12; i++)
        {
            var raw = joints[i].ToVector3();
            // Re-center XZ around hip anchor, keep Y relative to hip height
            float rx = raw.X - anchorX;
            float ry = raw.Y - anchorY;
            float rz = raw.Z - anchorZ;
            _spheres[i].Position = new Godot.Vector3(rx * 0.5f, ry * 0.8f + 0.95f, rz * 0.5f);
            // Thermography ramp: kappa -> desaturated naturalistic color
            float k = joints[i].Kappa;
            Color target;
            if (k < 0.5f)
                target = new Color(0.05f, 0.12f, 0.25f, 0.0f);
            else if (k < 1.0f)
                target = Color.FromHsv(0.55f, 0.45f, 0.35f, Mathf.InverseLerp(0.5f, 1.0f, k) * 0.35f);
            else if (k < 1.5f)
                target = Color.FromHsv(0.42f, 0.38f, 0.45f, Mathf.InverseLerp(1.0f, 1.5f, k) * 0.55f);
            else if (k < 2.0f)
                target = Color.FromHsv(0.10f, 0.50f, 0.65f, Mathf.InverseLerp(1.5f, 2.0f, k) * 0.70f);
            else
                target = Color.FromHsv(0.97f, 0.38f, 0.78f, Mathf.Clamp((k - 2.0f) * 0.4f + 0.70f, 0.7f, 0.92f));
            // Smooth lerp toward target (0.12 = ~8 frames at 60fps)
            var prev = (Color)_mats[i].GetShaderParameter("sphere_color");
            var smooth = prev.Lerp(target, 0.12f);
            _mats[i].SetShaderParameter("sphere_color", smooth);
        }
    }
}

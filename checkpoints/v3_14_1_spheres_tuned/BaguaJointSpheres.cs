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
            mat.SetShaderParameter("sphere_color", new Color(0.196f, 0.506f, 0.51f, 1f));
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
            _mats[i].SetShaderParameter("sphere_color", joints[i].ToColor());
        }
    }
}

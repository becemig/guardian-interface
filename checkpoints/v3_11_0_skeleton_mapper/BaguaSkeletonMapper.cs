// BaguaSkeletonMapper.cs
// Guardian Interface -- Bagua Physics Skeleton Driver
// Maps 12 server joints to Skeleton3D bone poses in real time

using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class BaguaSkeletonMapper : Node
{
    [Export] public NodePath AvatarPath = new NodePath("../BaguaAvatar");

    // Bone name mapping: server joint index -> bone name in GLB rig
    private static readonly string[] BoneNames = new string[12]
    {
        "upper_arm.L",  // 0  left_shoulder
        "upper_arm.R",  // 1  right_shoulder
        "forearm.L",    // 2  left_elbow
        "forearm.R",    // 3  right_elbow
        "hand.L",       // 4  left_wrist
        "hand.R",       // 5  right_wrist
        "thigh.L",      // 6  left_hip
        "thigh.R",      // 7  right_hip
        "shin.L",       // 8  left_knee
        "shin.R",       // 9  right_knee
        "foot.L",       // 10 left_ankle
        "foot.R",       // 11 right_ankle
    };

    private Skeleton3D _skeleton;
    private int[] _boneIdx = new int[12];
    private BaguaPhysicsClient _client;

    public override void _Ready()
    {
        var avatar = GetNode<Node3D>(AvatarPath);
        _skeleton = FindSkeleton(avatar);
        if (_skeleton == null)
        {
            GD.PrintErr("[BaguaSkeletonMapper] Skeleton3D not found under avatar");
            return;
        }
        GD.Print("[BaguaSkeletonMapper] Skeleton3D found: " + _skeleton.Name);

        for (int i = 0; i < 12; i++)
        {
            _boneIdx[i] = _skeleton.FindBone(BoneNames[i]);
            if (_boneIdx[i] == -1)
                GD.PrintErr("[BaguaSkeletonMapper] Bone not found: " + BoneNames[i]);
            else
                GD.Print("[BaguaSkeletonMapper] Mapped bone " + i + " -> " + BoneNames[i] + " idx=" + _boneIdx[i]);
        }

        _client = GetNode<BaguaPhysicsClient>("/root/BaguaPhysicsClient");
        if (_client != null)
            _client.BaguaFrameReceived += OnFrame;
        else
            GD.PrintErr("[BaguaSkeletonMapper] BaguaPhysicsClient not found");
    }

    private Skeleton3D FindSkeleton(Node root)
    {
        if (root is Skeleton3D sk) return sk;
        foreach (Node child in root.GetChildren())
        {
            var found = FindSkeleton(child);
            if (found != null) return found;
        }
        return null;
    }

    private void OnFrame(BaguaFrameGodot wrapper)
    {
        if (_skeleton == null) return;
        var joints = wrapper.Data.Joints;
        if (joints == null || joints.Count < 12) return;

        for (int i = 0; i < 12; i++)
        {
            int idx = _boneIdx[i];
            if (idx == -1) continue;

            var pos = joints[i].ToVector3();
            _skeleton.SetBonePosePosition(idx, pos);
        }
    }
}

// BaguaFrameData.cs
// Guardian Interface -- Bagua Physics Integration
// Typed C# structs matching BaguaViewer JSON frame schema.
// Ground-truth verified 2026-07-20.
// Godot 4.6.2 Mono / .NET 8

using System.Collections.Generic;
using Godot;


public class BaguaFrame
{
    public int    FrameIdx    { get; set; }
    public float  GlobalA     { get; set; }
    public float  Wave        { get; set; }
    public List<JointData>  Joints      { get; set; } = new();
    public GrfData          Grf         { get; set; } = new();
    public List<float[]>    Vel         { get; set; } = new();
    public List<float[]>    Acc         { get; set; } = new();
    public FascialData      Fascial     { get; set; } = new();
    public FiveElementData  FiveElement { get; set; } = new();
    public BaGangData       BaGang      { get; set; } = new();
    public MechData         Mech        { get; set; } = new();
    public NeuroData        Neuro       { get; set; } = new();
    public ChannelData      Channel     { get; set; } = new();
    public Vector3 GetJointPos(int i) => i < Joints.Count ? Joints[i].ToVector3() : Vector3.Zero;
    public Vector3 GetVel(int i) => i < Vel.Count ? new Vector3(Vel[i][0],Vel[i][1],Vel[i][2]) : Vector3.Zero;
}
public class JointData
{
    public float X { get; set; } public float Y { get; set; } public float Z { get; set; }
    public float Kappa { get; set; } public float A { get; set; } public string Rgb { get; set; } = "#20808D";
    public IcrData Icr { get; set; } = new();
    public Vector3 ToVector3() => new Vector3(X,Y,Z);
    public Color ToColor() => new Color(Rgb);
}
public class IcrData
{
    public bool Valid { get; set; }
    public float[] Cp { get; set; } = new float[3];
    public float[] Cf { get; set; } = new float[3];
    public float Lam { get; set; } public float Mag { get; set; }
    public Vector3 CentripVec => new Vector3(Cp[0],Cp[1],Cp[2]);
    public Vector3 CentrifVec => new Vector3(Cf[0],Cf[1],Cf[2]);
}
public class GrfData
{
    public float[] R { get; set; } = new float[3];
    public float[] L { get; set; } = new float[3];
    public float Mag { get; set; }
    public Vector3 RightVec => new Vector3(R[0],R[1],R[2]);
    public Vector3 LeftVec  => new Vector3(L[0],L[1],L[2]);
}
public class FascialData
{
    public Dictionary<string,float> At { get; set; } = new();
    public Dictionary<string,float> Jj { get; set; } = new();
    public string YjjStage { get; set; } = "";
    public float GetAt(string k) => At.TryGetValue(k,out var v)?v:0f;
    public float GetJj(string k) => Jj.TryGetValue(k,out var v)?v:0f;
}
public class FiveElementData
{
    public Dictionary<string,float> Scores { get; set; } = new();
    public string Dominant { get; set; } = "";
    public string ResonantHerbs { get; set; } = "";
    public float GetScore(string e) => Scores.TryGetValue(e,out var v)?v:0f;
}
public class BaGangData
{
    public float Yin { get; set; } public float Yang { get; set; }
    public float Interior { get; set; } public float Exterior { get; set; }
    public float Cold { get; set; } public float Hot { get; set; }
    public float Deficient { get; set; } public float Excess { get; set; }
    public string Pattern { get; set; } = ""; public float Confidence { get; set; }
    public bool IsYang     => Yang     > Yin;
    public bool IsExterior => Exterior > Interior;
    public bool IsHot      => Hot      > Cold;
    public bool IsExcess   => Excess   > Deficient;
}
public class MechData
{
    public float[] Stress { get; set; } = new float[12];
    public float[] Piezo  { get; set; } = new float[12];
    public bool[]  Integrin { get; set; } = new bool[12];
    public bool[]  YapTaz   { get; set; } = new bool[12];
    public bool[]  Remodel  { get; set; } = new bool[12];
    public float   MechIndex    { get; set; }
    public string  DominantZone { get; set; } = "";
}
public class NeuroData
{
    public float[] Ruffini { get; set; } = new float[12];
    public float[] Pacini  { get; set; } = new float[12];
    public bool[]  Golgi   { get; set; } = new bool[12];
    public float[] Spindle { get; set; } = new float[12];
    public float[] Pulse   { get; set; } = new float[12];
    public float Prop { get; set; } public float Autonomic { get; set; }
    public string Receptor { get; set; } = "";
    public bool IsSympathetic => Autonomic > 0.5f;
}
public class ChannelData
{
    public Dictionary<string,float> Activation  { get; set; } = new();
    public Dictionary<string,float> ElementLoad { get; set; } = new();
    public Dictionary<string,float> Asymmetry   { get; set; } = new();
    public string DominantChannel { get; set; } = "";
    public string DominantElement { get; set; } = "";
    public float YinTotal { get; set; } public float YangTotal { get; set; }
    public float GetChannel(string k) => Activation.TryGetValue(k,out var v)?v:0f;
    public float GetElement(string k) => ElementLoad.TryGetValue(k,out var v)?v:0f;
}

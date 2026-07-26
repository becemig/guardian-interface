// BaguaPhysicsClient.cs
// Guardian Interface -- Bagua Physics Integration
// WebSocket client node for Godot 4.6.2 Mono / .NET 8
// Add as AutoLoad: Project > Project Settings > AutoLoad
// ServerUrl: ws://localhost:8765/ws

using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;


[GlobalClass]
public partial class BaguaPhysicsClient : Node
{
    [Export] public string ServerUrl    = "ws://localhost:8765/ws";
    [Export] public bool   AutoConnect  = true;
    [Export] public float  ReconnectSec = 3.0f;

    [Signal] public delegate void BaguaFrameReceivedEventHandler(BaguaFrameGodot frame);
    [Signal] public delegate void ConnectedEventHandler();
    [Signal] public delegate void DisconnectedEventHandler();

    private WebSocketPeer _ws             = new WebSocketPeer();
    private bool          _connected      = false;
    private float         _reconnectTimer = 0f;
    private BaguaFrame    _lastFrame;

    public BaguaFrame LastFrame   => _lastFrame;
    public bool       IsConnected => _connected;

    public override void _Ready()  { if (AutoConnect) Connect(); }

    public override void _Process(double delta)
    {
        _ws.Poll();
        var state = _ws.GetReadyState();
        switch (state)
        {
            case WebSocketPeer.State.Open:
                if (!_connected) {
                    _connected = true;
                    EmitSignal(SignalName.Connected);
                    GD.Print("[BaguaPhysicsClient] Connected to ", ServerUrl);
                }
                while (_ws.GetAvailablePacketCount() > 0)
                    _HandlePacket(_ws.GetPacket());
                break;
            case WebSocketPeer.State.Closed:
                if (_connected) {
                    _connected = false;
                    EmitSignal(SignalName.Disconnected);
                    GD.Print("[BaguaPhysicsClient] Disconnected.");
                }
                _reconnectTimer += (float)delta;
                if (_reconnectTimer >= ReconnectSec) { _reconnectTimer = 0f; Connect(); }
                break;
        }
    }

    public void Connect()
    {
        var err = _ws.ConnectToUrl(ServerUrl);
        if (err != Error.Ok) GD.PrintErr("[BaguaPhysicsClient] Error: ", err);
    }
    public void Disconnect() => _ws.Close();

    private void _HandlePacket(byte[] data)
    {
        var json = "";
        try {
            json = System.Text.Encoding.UTF8.GetString(data);
            var root  = JsonDocument.Parse(json).RootElement;
            var frame = _ParseFrame(root);
            _lastFrame = frame;
            EmitSignal(SignalName.BaguaFrameReceived, new BaguaFrameGodot(frame));
        } catch (Exception e) {
            GD.PrintErr("[BaguaPhysicsClient] Parse error: ", e.Message, " | JSON snippet: ", json.Length > 200 ? json.Substring(0,200) : json);
        }
    }

    private static BaguaFrame _ParseFrame(JsonElement r)
    {
        var f = new BaguaFrame();
        f.FrameIdx = _Int(r,   "frame");
        f.GlobalA  = _Float(r, "global_A");
        f.Wave     = _Float(r, "wave");
        if (r.TryGetProperty("joints", out var joints))
            foreach (var j in joints.EnumerateArray()) f.Joints.Add(_ParseJoint(j));
        if (r.TryGetProperty("grf", out var grf)) {
            f.Grf.R   = _Vec3Arr(grf, "R");
            f.Grf.L   = _Vec3Arr(grf, "L");
            f.Grf.Mag = _Float(grf, "mag");
        }
        if (r.TryGetProperty("vel", out var vel))
            foreach (var v in vel.EnumerateArray()) f.Vel.Add(_FloatArr(v));
        if (r.TryGetProperty("acc", out var acc))
            foreach (var a in acc.EnumerateArray()) f.Acc.Add(_FloatArr(a));
            GD.Print("[DEBUG] parsing fascial");
        if (r.TryGetProperty("fascial", out var fasc)) {
            f.Fascial.YjjStage = _Int(fasc, "yjj_stage").ToString();
            if (fasc.TryGetProperty("at", out var at))
                foreach (var kv in at.EnumerateObject()) f.Fascial.At[kv.Name] = kv.Value.TryGetProperty("act", out var actA) ? actA.GetSingle() : kv.Value.GetSingle();
            if (fasc.TryGetProperty("jj", out var jj))
                foreach (var kv in jj.EnumerateObject()) f.Fascial.Jj[kv.Name] = kv.Value.TryGetProperty("act", out var actJ) ? actJ.GetSingle() : kv.Value.GetSingle();
        }
            GD.Print("[DEBUG] parsing five_element");
        if (r.TryGetProperty("five_element", out var el)) {
            f.FiveElement.Dominant      = _Str(el, "dominant");
            if (el.TryGetProperty("resonant_herbs", out var rh)) f.FiveElement.ResonantHerbs = rh.ValueKind == System.Text.Json.JsonValueKind.String ? rh.GetString() ?? "" : rh.GetRawText();
            if (el.TryGetProperty("scores", out var sc))
                foreach (var kv in sc.EnumerateObject()) f.FiveElement.Scores[kv.Name] = kv.Value.GetSingle();
        }
            GD.Print("[DEBUG] parsing ba_gang");
        if (r.TryGetProperty("ba_gang", out var bg)) {
            f.BaGang.Yin        = _Float(bg, "yin");
            f.BaGang.Yang       = _Float(bg, "yang");
            f.BaGang.Interior   = _Float(bg, "interior");
            f.BaGang.Exterior   = _Float(bg, "exterior");
            f.BaGang.Cold       = _Float(bg, "cold");
            f.BaGang.Hot        = _Float(bg, "hot");
            f.BaGang.Deficient  = _Float(bg, "deficient");
            f.BaGang.Excess     = _Float(bg, "excess");
            f.BaGang.Pattern    = _Str(bg,   "pattern");
            f.BaGang.Confidence = _Float(bg, "confidence");
        }
            GD.Print("[DEBUG] parsing mech");
        if (r.TryGetProperty("mech", out var mech)) {
            f.Mech.Stress       = _FloatArrFlat(mech, "stress");
            f.Mech.Piezo        = _FloatArrFlat(mech, "piezo");
            f.Mech.Integrin     = _BoolArrFlat(mech,  "integrin");
            f.Mech.YapTaz       = _BoolArrFlat(mech,  "yap_taz");
            f.Mech.Remodel      = _BoolArrFlat(mech,  "remodel");
            f.Mech.MechIndex    = _Float(mech, "mech_index");
            f.Mech.DominantZone = _Int(mech, "dominant_zone").ToString();
        }
            GD.Print("[DEBUG] parsing neuro");
        if (r.TryGetProperty("neuro", out var neuro)) {
            f.Neuro.Ruffini   = _FloatArrFlat(neuro, "ruffini");
            f.Neuro.Pacini    = _FloatArrFlat(neuro, "pacini");
            f.Neuro.Golgi     = _BoolArrFlat(neuro,  "golgi");
            f.Neuro.Spindle   = _FloatArrFlat(neuro, "spindle");
            f.Neuro.Pulse     = _FloatArrFlat(neuro, "pulse");
            f.Neuro.Prop      = _Float(neuro, "prop");
            f.Neuro.Autonomic = _Float(neuro, "autonomic");
            f.Neuro.Receptor  = _Str(neuro,   "receptor");
        }
            GD.Print("[DEBUG] parsing channel");
        if (r.TryGetProperty("channel", out var ch)) {
            f.Channel.DominantChannel = _Str(ch,   "dominant_channel");
            f.Channel.DominantElement = _Str(ch,   "dominant_element");
            f.Channel.YinTotal        = _Float(ch, "yin_total");
            f.Channel.YangTotal       = _Float(ch, "yang_total");
            if (ch.TryGetProperty("activation",   out var act))
                foreach (var kv in act.EnumerateObject())   f.Channel.Activation[kv.Name]  = kv.Value.GetSingle();
            if (ch.TryGetProperty("element_load", out var eload))
                foreach (var kv in eload.EnumerateObject()) f.Channel.ElementLoad[kv.Name] = kv.Value.GetSingle();
            if (ch.TryGetProperty("asymmetry",    out var asym))
                foreach (var kv in asym.EnumerateObject())  f.Channel.Asymmetry[kv.Name]   = kv.Value.GetSingle();
        }
        return f;
    }

    private static JointData _ParseJoint(JsonElement j)
    {
        var jd = new JointData {
            X=_Float(j,"x"), Y=_Float(j,"y"), Z=_Float(j,"z"),
            Kappa=_Float(j,"kappa"), A=_Float(j,"A"), Rgb=_RgbToHex(j,"rgb")
        };
        if (j.TryGetProperty("icr", out var icr))
            jd.Icr = new IcrData {
                Valid=_Bool(icr,"valid"), Cp=_Vec3Arr(icr,"cp"),
                Cf=_Vec3Arr(icr,"cf"),   Lam=_Float(icr,"lam"), Mag=_Float(icr,"mag")
            };
        return jd;
    }

    private static int    _Int(JsonElement e,string k)   => e.TryGetProperty(k,out var v)?v.GetInt32():0;
    private static float  _Float(JsonElement e,string k) => e.TryGetProperty(k,out var v)?v.GetSingle():0f;
    private static string _Str(JsonElement e,string k)   => e.TryGetProperty(k,out var v)?v.GetString()??"":"";
    private static bool   _Bool(JsonElement e,string k)  => e.TryGetProperty(k,out var v)&&v.GetBoolean();
    private static string _RgbToHex(JsonElement e,string k) {
        if (!e.TryGetProperty(k,out var v)) return "#808080";
        var arr = v.EnumerateArray().GetEnumerator();
        float[] rgb = new float[3];
        int idx = 0;
        while (arr.MoveNext() && idx < 3) rgb[idx++] = arr.Current.GetSingle();
        if (idx < 3) return "#808080";
        int r = (int)(rgb[0]*255f);
        int g = (int)(rgb[1]*255f);
        int b = (int)(rgb[2]*255f);
        return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
    }

    private static float[] _Vec3Arr(JsonElement e,string k) {
        if (!e.TryGetProperty(k,out var v)) return new float[3];
        var a=new float[3]; int i=0;
        foreach(var el in v.EnumerateArray()) if(i<3) a[i++]=el.GetSingle();
        return a;
    }
    private static float[] _FloatArr(JsonElement v) {
        var l=new List<float>();
        foreach(var el in v.EnumerateArray()) l.Add(el.GetSingle());
        return l.ToArray();
    }
    private static float[] _FloatArrFlat(JsonElement e,string k) {
        if (!e.TryGetProperty(k,out var v)) return Array.Empty<float>();
        var l=new List<float>();
        foreach(var el in v.EnumerateArray()) l.Add(el.GetSingle());
        return l.ToArray();
    }
    private static bool[] _BoolArrFlat(JsonElement e,string k) {
        if (!e.TryGetProperty(k,out var v)) return Array.Empty<bool>();
        var l=new List<bool>();
        foreach(var el in v.EnumerateArray()) l.Add(el.GetBoolean());
        return l.ToArray();
    }
}

public partial class BaguaFrameGodot : GodotObject
{
    public BaguaFrame Data { get; }
    public BaguaFrameGodot(BaguaFrame frame) { Data = frame; }
}

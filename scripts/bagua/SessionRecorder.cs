// SessionRecorder.cs -- Guardian Interface
// Logs TCM context annotations to JSON per session.
// One file per session: session_YYYYMMDD_HHMMSS.json
// Subscribes to TCMContextResolver.TCMContextResolved and
// KappaAtlasResolver.VolResolved for full per-frame capture.
//
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[GlobalClass]
public partial class SessionRecorder : Node
{
    private string _sessionPath = "";
    private StreamWriter _writer = null;
    private bool _firstFrame = true;
    private int _frameCount = 0;
    private string _sessionStart = "";

    // Latest kappa and joint from VolResolved
    private float _lastKappa = 0f;
    private int _lastJoint = 0;
    private int _lastVol = 0;
    public override void _Ready()
    {
        _sessionStart = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string dir = "/home/becemig/guardian-sessions";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        _sessionPath = dir + "/session_" + _sessionStart + ".json";
        _writer = new StreamWriter(_sessionPath, false, Encoding.UTF8);
        _writer.WriteLine("{");
        _writer.WriteLine("  \"session_start\": \"" + _sessionStart + "\",");
        _writer.WriteLine("  \"frames\": [");
        _writer.AutoFlush = true;
        // Wire VolResolved for kappa + joint
        var kappaResolver = GetTree().Root.FindChild("KappaAtlasResolver", true, false);
        if (kappaResolver != null)
            kappaResolver.Connect("VolResolved", new Callable(this, nameof(OnVolResolved)));
        else
            GD.PrintErr("[SessionRecorder] KappaAtlasResolver not found");
        // Wire TCMContextResolved for full annotation
        var tcmResolver = GetTree().Root.FindChild("TCMContextResolver", true, false);
        if (tcmResolver != null)
            tcmResolver.Connect("TCMContextResolved", new Callable(this, nameof(OnTCMContext)));
        else
            GD.PrintErr("[SessionRecorder] TCMContextResolver not found");
        GD.Print("[SessionRecorder] Recording to: " + _sessionPath);
    }
    private void OnVolResolved(int volId, int l1, int l2, int l3,
        string channel, string element, int jointIdx)
    {
        _lastVol   = volId;
        _lastJoint = jointIdx;
    }
    private void OnTCMContext(string channel, string element, string tissue,
        string neuroscience, string horary, string nutrition,
        string acupoints, string koSheng, string qigongForm)
    {
        if (_writer == null) return;
        string ts = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        var sb = new StringBuilder();
        if (!_firstFrame) sb.Append(",\n");
        sb.Append("    {\n");
        sb.Append("      \"timestamp\": \"" + ts + "\",\n");
        sb.Append("      \"frame\": " + _frameCount + ",\n");
        sb.Append("      \"vol\": " + _lastVol + ",\n");
        sb.Append("      \"joint_idx\": " + _lastJoint + ",\n");
        sb.Append("      \"channel\": \"" + channel + "\",\n");
        sb.Append("      \"element\": \"" + element + "\",\n");
        sb.Append("      \"tissue\": \"" + Esc(tissue) + "\",\n");
        sb.Append("      \"neuroscience\": \"" + Esc(neuroscience) + "\",\n");
        sb.Append("      \"horary\": \"" + Esc(horary) + "\",\n");
        sb.Append("      \"ko_sheng\": \"" + Esc(koSheng) + "\",\n");
        sb.Append("      \"qigong_form\": \"" + Esc(qigongForm) + "\",\n");
        sb.Append("      \"nutrition\": \"" + Esc(nutrition) + "\",\n");
        sb.Append("      \"acupoints\": \"" + Esc(acupoints) + "\"\n");
        sb.Append("    }");
        _writer.Write(sb.ToString());
        _firstFrame = false;
        _frameCount++;
    }
    private static string Esc(string s)
    {
        if (s == null) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", "");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest || what == NotificationExitTree)
            CloseSession();
    }

    private void CloseSession()
    {
        if (_writer == null) return;
        _writer.WriteLine("\n  ],");
        _writer.WriteLine("  \"session_end\": \"" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "\",");
        _writer.WriteLine("  \"total_frames\": " + _frameCount);
        _writer.WriteLine("}");
        _writer.Flush();
        _writer.Close();
        _writer = null;
        GD.Print("[SessionRecorder] Session closed. Frames: " + _frameCount);
    }
}

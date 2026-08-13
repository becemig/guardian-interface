using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public partial class ARPreviewController : Node3D
{
    private const double WeightShiftCycleSeconds = 6.0;

    private Label _sourceLabel;
    private Label _sessionLabel;
    private Label _cueLabel;
    private Label _loadLabel;
    private ProgressBar _loadBar;
    private LoadDynamicsPlot _dynamicsPlot;
    private Label _dynamicsLabel;
    private JingJinOverlay _jingJinOverlay;
    private Label _feedbackLabel;
    private Label _logLabel;
    private Label _timelineLabel;
    private Button _previewButton;
    private Button _anatomyToggle;
    private Button _studyToggle;
    private OptionButton _anatomyRegionSelector;
    private Label _anatomyLegend;
    private Button _jingJinFrameworkToggle;
    private Label _jingJinFrameworkLegend;
    private bool _jingJinFrameworkVisible;
    private Button _conceptualCnsToggle;
    private Label _conceptualCnsLegend;
    private Label _displayLayersKey;
    private OptionButton _displayPresetSelector;
    private Button _focusViewToggle;
    private bool _focusView;
    private ConceptualCnsOverlay _conceptualCnsOverlay;
    private bool _conceptualCnsVisible;
    private BiomedicalAnatomyLayer _biomedicalAnatomyLayer;
    private Camera3D _presentationCamera;
    private Node3D _jointSpheres;
    private bool _anatomyStudyMode;
    private bool _anatomyVisibleBeforeStudy;
    private Vector3 _standardCameraPosition;

    private bool _previewActive;
    private double _elapsedSeconds;

    private string _sessionPath = "";
    private string _sessionFileName = "";
    private string _lastCue = "";
    private float _lastLeftLoad = 50.0f;
    private float _lastRightLoad = 50.0f;
    private readonly List<string> _recentEventRows = new();

    public override void _Ready()
    {
        _sourceLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/SourceLabel"
        );

        _sessionLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/SessionLabel"
        );

        _cueLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/CueLabel"
        );

        _loadLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/LoadLabel"
        );

        _loadBar = GetNode<ProgressBar>(
            "CanvasLayer/HUD/Panel/Margin/VBox/LoadBar"
        );

        VBoxContainer vBox = GetNode<VBoxContainer>(
            "CanvasLayer/HUD/Panel/Margin/VBox"
        );

        _dynamicsPlot = new LoadDynamicsPlot
        {
            Name = "LoadDynamicsPlot"
        };

        vBox.AddChild(_dynamicsPlot);

        _dynamicsLabel = new Label
        {
            Name = "DynamicsLabel",
            Text = "MODEL DYNAMICS — SYNTHETIC REFERENCE\nAwaiting synthetic session.",
            HorizontalAlignment = HorizontalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };

        _dynamicsLabel.AddThemeFontSizeOverride("font_size", 10);
        _dynamicsLabel.AddThemeColorOverride(
            "font_color",
            new Color(0.62f, 0.76f, 0.82f, 1.0f)
        );

        vBox.AddChild(_dynamicsLabel);

        _feedbackLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/FeedbackLabel"
        );

        _logLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/LogLabel"
        );

        _timelineLabel = GetNode<Label>(
            "CanvasLayer/HUD/Panel/Margin/VBox/TimelineLabel"
        );

        _previewButton = GetNode<Button>(
            "CanvasLayer/HUD/Panel/Margin/VBox/PreviewButton"
        );

        _previewButton.Pressed += TogglePreview;

        _anatomyToggle = GetNode<Button>(
            "CanvasLayer/AnatomyToggle"
        );

        _biomedicalAnatomyLayer =
            GetNodeOrNull<BiomedicalAnatomyLayer>(
                "AvatarStage/BiomedicalAnatomyLayer"
            );

        _anatomyToggle.Pressed += ToggleBiomedicalAnatomy;

        _studyToggle = GetNode<Button>(
            "CanvasLayer/StudyToggle"
        );

        _studyToggle.Pressed += ToggleAnatomyStudyMode;

        _anatomyRegionSelector = GetNode<OptionButton>(
            "CanvasLayer/AnatomyRegionSelector"
        );

        _anatomyLegend = GetNode<Label>(
            "CanvasLayer/AnatomyLegend"
        );

        _anatomyRegionSelector.ItemSelected += SetAnatomyStudyRegion;

        _jingJinFrameworkToggle = GetNode<Button>(
            "CanvasLayer/JingJinFrameworkToggle"
        );

        _jingJinFrameworkLegend = GetNode<Label>(
            "CanvasLayer/JingJinFrameworkLegend"
        );

        _jingJinFrameworkToggle.Pressed += ToggleJingJinFramework;
        _jingJinFrameworkLegend.Visible = false;

        _conceptualCnsToggle = GetNode<Button>(
            "CanvasLayer/ConceptualCnsToggle"
        );

        _conceptualCnsLegend = GetNode<Label>(
            "CanvasLayer/ConceptualCnsLegend"
        );

        _displayLayersKey = GetNode<Label>(
            "CanvasLayer/DisplayLayersKey"
        );

        _displayPresetSelector = GetNode<OptionButton>(
            "CanvasLayer/DisplayPresetSelector"
        );

        _displayPresetSelector.ItemSelected += ApplyDisplayPreset;

        _focusViewToggle = GetNode<Button>(
            "CanvasLayer/FocusViewToggle"
        );

        _focusViewToggle.Pressed += ToggleFocusView;
        _focusView = false;

        _conceptualCnsToggle.Pressed += ToggleConceptualCnsDiagram;
        _conceptualCnsLegend.Visible = false;
        _conceptualCnsVisible = false;

        var avatarStage = GetNodeOrNull<Node3D>("AvatarStage");

        if (avatarStage != null)
        {
            _conceptualCnsOverlay = new ConceptualCnsOverlay();
            avatarStage.AddChild(_conceptualCnsOverlay);
            _conceptualCnsOverlay.Visible = false;
        }
        else
        {
            GD.PushWarning(
                "[ARPreview] AvatarStage not found; conceptual CNS diagram unavailable."
            );
        }

        _presentationCamera = GetNodeOrNull<Camera3D>(
            "AvatarStage/Camera3D"
        );

        if (_presentationCamera != null)
            _standardCameraPosition = _presentationCamera.Position;

        _jointSpheres = GetNodeOrNull<Node3D>(
            "AvatarStage/BaguaJointSpheres"
        );

        _jingJinOverlay = GetNodeOrNull<JingJinOverlay>(
            "AvatarStage/JingJinOverlay"
        );

        _jingJinFrameworkVisible = false;

        if (_jingJinOverlay != null)
            _jingJinOverlay.Visible = false;

        _jingJinFrameworkToggle.Text = "Show Supplementary Jing Jin";
        _jingJinFrameworkLegend.Visible = false;

        if (_jingJinOverlay == null)
            GD.PushWarning(
                "[ARPreview] AvatarStage/JingJinOverlay not found; activation overlay unavailable."
            );

        UpdateHud();
    }

    public override void _Process(double delta)
    {
        if (!_previewActive)
            return;

        _elapsedSeconds += delta;
        UpdateHud();
    }

    private void ToggleBiomedicalAnatomy()
    {
        if (_biomedicalAnatomyLayer == null)
        {
            GD.PushWarning(
                "[ARPreview] BiomedicalAnatomyLayer not found."
            );

            _anatomyToggle.Text = "Biomedical Anatomy Unavailable";
            return;
        }

        bool nextVisible = !_biomedicalAnatomyLayer.Visible;

        _biomedicalAnatomyLayer.SetLayerVisible(nextVisible);

        _anatomyToggle.Text = nextVisible
            ? "Hide Biomedical Anatomy"
            : "Show Biomedical Anatomy";

        GD.Print(
            $"[ARPreview] Biomedical anatomy labels: " +
            $"{(nextVisible ? "VISIBLE" : "HIDDEN")}"
        );
    }

    private void SetFocusView(bool visible)
    {
        if (_anatomyStudyMode)
            visible = false;

        _focusView = visible;

        _biomedicalAnatomyLayer?.SetFocusMode(visible);

        if (_presentationCamera != null)
        {
            _presentationCamera.Position = visible
                ? new Vector3(0.0f, 1.0f, 3.8f)
                : _standardCameraPosition;
        }

        _focusViewToggle.Text = visible
            ? "Exit Focus View"
            : "Enter Focus View";

        GD.Print(
            visible
                ? "[ARPreview] Focus View enabled."
                : "[ARPreview] Focus View disabled."
        );
    }

    private void ToggleFocusView()
    {
        SetFocusView(!_focusView);
    }

    private void ExitStudyModeIfNeeded()
    {
        if (_anatomyStudyMode)
            ToggleAnatomyStudyMode();
    }

    private void SetBiomedicalLayerVisible(bool visible)
    {
        _biomedicalAnatomyLayer?.SetLayerVisible(visible);
        _anatomyToggle.Text = visible
            ? "Hide Biomedical Anatomy"
            : "Show Biomedical Anatomy";
    }

    private void SetJingJinFrameworkVisible(bool visible)
    {
        _jingJinFrameworkVisible = visible;

        if (_jingJinOverlay != null)
            _jingJinOverlay.Visible = visible;

        _jingJinFrameworkLegend.Visible = visible;

        _jingJinFrameworkToggle.Text = visible
            ? "Hide Supplementary Jing Jin"
            : "Show Supplementary Jing Jin";
    }

    private void SetConceptualCnsVisible(bool visible)
    {
        _conceptualCnsVisible = visible;

        if (_conceptualCnsOverlay != null)
            _conceptualCnsOverlay.Visible = visible;

        _conceptualCnsLegend.Visible = visible;

        _conceptualCnsToggle.Text = visible
            ? "Hide Conceptual CNS Diagram"
            : "Show Conceptual CNS Diagram";
    }

    private void ApplyDisplayPreset(long selectedIndex)
    {
        if (selectedIndex == 0)
            return;

        ExitStudyModeIfNeeded();

        switch (selectedIndex)
        {
            case 1:
                SetFocusView(false);
                SetBiomedicalLayerVisible(false);
                SetJingJinFrameworkVisible(false);
                SetConceptualCnsVisible(false);

                GD.Print("[ARPreview] Display preset: Motion Preview.");
                break;

            case 2:
                SetJingJinFrameworkVisible(false);
                SetConceptualCnsVisible(false);
                SetBiomedicalLayerVisible(true);

                if (!_anatomyStudyMode)
                    ToggleAnatomyStudyMode();

                _anatomyRegionSelector.Selected = 0;
                _biomedicalAnatomyLayer?.SetStudyRegion(
                    AnatomyStudyRegion.All
                );

                GD.Print("[ARPreview] Display preset: Biomedical Study.");
                break;

            case 3:
                SetFocusView(true);
                SetBiomedicalLayerVisible(true);
                SetJingJinFrameworkVisible(true);
                SetConceptualCnsVisible(true);

                GD.Print(
                    "[ARPreview] Display preset: Comparative Teaching."
                );
                break;

            case 4:
                SetFocusView(false);
                SetBiomedicalLayerVisible(false);
                SetJingJinFrameworkVisible(false);
                SetConceptualCnsVisible(false);

                GD.Print("[ARPreview] Display reset to baseline.");
                break;
        }

        _displayPresetSelector.Selected = 0;
    }

    private void ToggleConceptualCnsDiagram()
    {
        _conceptualCnsVisible = !_conceptualCnsVisible;

        if (_conceptualCnsOverlay != null)
            _conceptualCnsOverlay.Visible = _conceptualCnsVisible;

        _conceptualCnsLegend.Visible = _conceptualCnsVisible;

        _conceptualCnsToggle.Text = _conceptualCnsVisible
            ? "Hide Conceptual CNS Diagram"
            : "Show Conceptual CNS Diagram";

        GD.Print(
            _conceptualCnsVisible
                ? "[ARPreview] Conceptual CNS teaching diagram shown."
                : "[ARPreview] Conceptual CNS teaching diagram hidden."
        );
    }

    private void ToggleJingJinFramework()
    {
        _jingJinFrameworkVisible = !_jingJinFrameworkVisible;

        if (_jingJinOverlay != null)
            _jingJinOverlay.Visible = _jingJinFrameworkVisible;

        _jingJinFrameworkLegend.Visible = _jingJinFrameworkVisible;

        _jingJinFrameworkToggle.Text = _jingJinFrameworkVisible
            ? "Hide Supplementary Jing Jin"
            : "Show Supplementary Jing Jin";

        GD.Print(
            _jingJinFrameworkVisible
                ? "[ARPreview] Supplementary Jing Jin framework shown."
                : "[ARPreview] Supplementary Jing Jin framework hidden."
        );
    }

    private void SetAnatomyStudyRegion(long selectedIndex)
    {
        if (_biomedicalAnatomyLayer == null)
            return;

        AnatomyStudyRegion region = selectedIndex switch
        {
            1 => AnatomyStudyRegion.AxialSkeleton,
            2 => AnatomyStudyRegion.UpperLimbs,
            3 => AnatomyStudyRegion.LowerLimbs,
            _ => AnatomyStudyRegion.All
        };

        _biomedicalAnatomyLayer.SetStudyRegion(region);

        GD.Print(
            $"[ARPreview] Anatomy Study Region: {region}"
        );
    }

    private void ToggleAnatomyStudyMode()
    {
        _anatomyStudyMode = !_anatomyStudyMode;

        if (_anatomyStudyMode)
        {
            _anatomyVisibleBeforeStudy =
                _biomedicalAnatomyLayer != null &&
                _biomedicalAnatomyLayer.Visible;

            _biomedicalAnatomyLayer?.SetLayerVisible(true);
            _biomedicalAnatomyLayer?.SetStudyMode(true);
            _biomedicalAnatomyLayer?.SetStudyRegion(
                AnatomyStudyRegion.All
            );

            _anatomyRegionSelector.Selected = 0;
            _anatomyRegionSelector.Visible = true;
            _anatomyLegend.Visible = true;

            _jingJinFrameworkToggle.Visible = false;
            _jingJinFrameworkLegend.Visible = false;

            _conceptualCnsToggle.Visible = false;
            _conceptualCnsLegend.Visible = false;
            SetFocusView(false);

            _displayLayersKey.Visible = false;
            _displayPresetSelector.Visible = false;
            _focusViewToggle.Visible = false;

            if (_conceptualCnsOverlay != null)
                _conceptualCnsOverlay.Visible = false;

            _anatomyToggle.Text = "Hide Biomedical Anatomy";

            if (_presentationCamera != null)
                _presentationCamera.Position = new Vector3(0.0f, 1.0f, 3.8f);

            if (_jingJinOverlay != null)
                _jingJinOverlay.Visible = false;

            if (_jointSpheres != null)
                _jointSpheres.Visible = false;

            if (_dynamicsPlot != null)
                _dynamicsPlot.Visible = false;

            if (_dynamicsLabel != null)
                _dynamicsLabel.Visible = false;

            _studyToggle.Text = "Exit Anatomy Study Mode";

            GD.Print("[ARPreview] Anatomy Study Mode: ENABLED");
        }
        else
        {
            _biomedicalAnatomyLayer?.SetStudyMode(false);
            _biomedicalAnatomyLayer?.SetStudyRegion(
                AnatomyStudyRegion.All
            );

            _anatomyRegionSelector.Visible = false;
            _anatomyLegend.Visible = false;

            _jingJinFrameworkToggle.Visible = true;
            _jingJinFrameworkLegend.Visible = _jingJinFrameworkVisible;

            _conceptualCnsToggle.Visible = true;
            _conceptualCnsLegend.Visible = _conceptualCnsVisible;
            _displayLayersKey.Visible = true;
            _displayPresetSelector.Visible = true;
            _focusViewToggle.Visible = true;
            _focusViewToggle.Text = "Enter Focus View";

            if (_conceptualCnsOverlay != null)
                _conceptualCnsOverlay.Visible = _conceptualCnsVisible;

            _biomedicalAnatomyLayer?.SetLayerVisible(
                _anatomyVisibleBeforeStudy
            );

            _anatomyToggle.Text = _anatomyVisibleBeforeStudy
                ? "Hide Biomedical Anatomy"
                : "Show Biomedical Anatomy";

            if (_presentationCamera != null)
                _presentationCamera.Position = _standardCameraPosition;

            if (_jingJinOverlay != null)
                _jingJinOverlay.Visible = _jingJinFrameworkVisible;

            if (_jointSpheres != null)
                _jointSpheres.Visible = true;

            if (_dynamicsPlot != null)
                _dynamicsPlot.Visible = true;

            if (_dynamicsLabel != null)
                _dynamicsLabel.Visible = true;

            _studyToggle.Text = "Enter Anatomy Study Mode";

            GD.Print("[ARPreview] Anatomy Study Mode: DISABLED");
        }
    }

    private void TogglePreview()
    {
        if (_previewActive)
        {
            AppendEvent(
                "session_stopped",
                _lastCue,
                _lastLeftLoad,
                _lastRightLoad
            );

            _previewActive = false;

            GD.Print(
                $"[ARPreview] Synthetic preview stopped. Log: {_sessionPath}"
            );
        }
        else
        {
            _elapsedSeconds = 0.0f;
            _recentEventRows.Clear();
            _dynamicsPlot.Reset();
            _lastCue = "CENTER TRANSITION";
            _lastLeftLoad = 50.0f;
            _lastRightLoad = 50.0f;

            StartSyntheticSession();

            _previewActive = true;

            GD.Print(
                $"[ARPreview] Synthetic preview started. Log: {_sessionPath}"
            );
        }

        UpdateHud();
    }

    private void StartSyntheticSession()
    {
        string directory = ProjectSettings.GlobalizePath(
            "user://ar_preview_sessions"
        );

        Directory.CreateDirectory(directory);

        _sessionFileName =
            $"ar_preview_{DateTime.Now:yyyyMMdd_HHmmss}.jsonl";

        _sessionPath = Path.Combine(directory, _sessionFileName);

        AppendEvent(
            "session_started",
            _lastCue,
            _lastLeftLoad,
            _lastRightLoad
        );
    }

    private void AppendEvent(
        string eventType,
        string cue,
        float leftLoad,
        float rightLoad)
    {
        if (string.IsNullOrWhiteSpace(_sessionPath))
            return;

        var sessionEvent = new PreviewEvent
        {
            Schema = "guardian_ar_preview_event_v1",
            EventType = eventType,
            TimestampUtc = DateTimeOffset.UtcNow.ToString("O"),
            ElapsedSeconds = Math.Round(_elapsedSeconds, 3),
            SourceType = "synthetic",
            HumanData = false,
            Cue = cue,
            LeftLoadPercent = Math.Round(leftLoad, 2),
            RightLoadPercent = Math.Round(rightLoad, 2)
        };

        try
        {
            string line =
                JsonSerializer.Serialize(sessionEvent) + System.Environment.NewLine;

            File.AppendAllText(_sessionPath, line);
            AddTimelineEvent(eventType, cue, leftLoad, rightLoad);
        }
        catch (Exception exception)
        {
            GD.PrintErr(
                $"[ARPreview] Could not write session event: {exception.Message}"
            );
        }
    }


    private void AddTimelineEvent(
        string eventType,
        string cue,
        float leftLoad,
        float rightLoad)
    {
        string displayType = eventType switch
        {
            "session_started" => "START",
            "session_stopped" => "STOP",
            "cue_changed" => "CUE",
            _ => eventType.ToUpperInvariant()
        };

        _recentEventRows.Add(
            $"{FormatTime(_elapsedSeconds)}  {displayType,-5}  {cue}  |  " +
            $"L{leftLoad:0}% / R{rightLoad:0}%"
        );

        if (_recentEventRows.Count > 5)
            _recentEventRows.RemoveAt(0);

        UpdateTimeline();
    }

    private void UpdateTimeline()
    {
        if (_timelineLabel == null)
            return;

        string rows = _recentEventRows.Count == 0
            ? "— no synthetic events yet —"
            : string.Join("\n", _recentEventRows);

        _timelineLabel.Text =
            "RECENT SYNTHETIC EVENTS (SESSION ONLY)\n" + rows;
    }

    private void UpdateHud()
    {
        _sourceLabel.Text =
            "DATA SOURCE: SYNTHETIC — NOT HUMAN DATA";

        float leftLoad = 50.0f;
        string cue = "READY";

        if (_previewActive)
        {
            double cyclePosition =
                _elapsedSeconds / WeightShiftCycleSeconds;

            float phase =
                (float)(cyclePosition * Mathf.Pi * 2.0);

            leftLoad = 50.0f + 30.0f * Mathf.Sin(phase);

            if (leftLoad >= 55.0f)
                cue = "SHIFT LEFT";
            else if (leftLoad <= 45.0f)
                cue = "SHIFT RIGHT";
            else
                cue = "CENTER TRANSITION";
        }

        float rightLoad = 100.0f - leftLoad;

        bool cueChanged = _previewActive && cue != _lastCue;
        _lastCue = cue;
        _lastLeftLoad = leftLoad;
        _lastRightLoad = rightLoad;

        _loadBar.Value = leftLoad;

        if (_previewActive)
            _dynamicsPlot.AddSample(_elapsedSeconds, leftLoad, rightLoad);

        UpdateDynamicsReadout(leftLoad, rightLoad);

        if (_previewActive)
        {
            _jingJinOverlay?.SetSyntheticLoad(
                leftLoad,
                rightLoad,
                _elapsedSeconds
            );
        }
        else
        {
            _jingJinOverlay?.ClearSyntheticLoad();
        }

        _loadLabel.Text =
            $"SIMULATED LOAD:  LEFT {leftLoad:0}%   |   RIGHT {rightLoad:0}%";

        _cueLabel.Text = $"REFERENCE CUE: {cue}";

        if (_previewActive)
        {
            _sessionLabel.Text =
                $"PREVIEW STATUS: ACTIVE  |  {FormatTime(_elapsedSeconds)}";

            _feedbackLabel.Text =
                    "Synthetic 6-second left/right weight-shift cycle. " +
                    "Values are a software test fixture, not sensor data.\n" +
                    "EDUCATIONAL MODEL: segment-level rig proxies only; " +
                    "Jing Jin is a supplementary traditional-framework overlay, " +
                    "not an anatomical or physiological measurement.";

            _logLabel.Text =
                $"SESSION LOG: {_sessionFileName}";

            _previewButton.Text = "Stop Preview";

            if (cueChanged)
            {
                _dynamicsPlot.AddCueMarker(_elapsedSeconds, cue);

                AppendEvent(
                    "cue_changed",
                    cue,
                    leftLoad,
                    rightLoad
                );
            }
        }
        else
        {
            _sessionLabel.Text =
                "PREVIEW STATUS: READY";

            _feedbackLabel.Text =
                    "Activate preview to begin a synthetic weight-shift session.\n" +
                    "Educational model: segment-level rig proxies; no CNS anatomy; " +
                    "Jing Jin is a supplementary traditional-framework overlay.";

            _logLabel.Text =
                string.IsNullOrWhiteSpace(_sessionFileName)
                    ? "SESSION LOG: not started"
                    : $"LAST SESSION LOG: {_sessionFileName}";

            _previewButton.Text = "Activate Preview";
        }
    }

    private void UpdateDynamicsReadout(
        float leftLoad,
        float rightLoad)
    {
        if (_dynamicsLabel == null)
            return;

        if (!_previewActive)
        {
            _dynamicsLabel.Text =
                "MODEL DYNAMICS — SYNTHETIC REFERENCE\n" +
                "Ready: analytic reference model; no human measurements.";
            return;
        }

        double omega = 2.0 * Math.PI / WeightShiftCycleSeconds;
        double phaseRadians = _elapsedSeconds * omega;
        double phaseDegrees =
            (phaseRadians * 180.0 / Math.PI) % 360.0;

        if (phaseDegrees < 0.0)
            phaseDegrees += 360.0;

        double frequency = 1.0 / WeightShiftCycleSeconds;
        double leftVelocity = 30.0 * omega * Math.Cos(phaseRadians);
        double leftAcceleration =
            -30.0 * omega * omega * Math.Sin(phaseRadians);

        double asymmetry = (leftLoad - rightLoad) / 100.0;
        string direction = asymmetry > 0.01
            ? "L > R"
            : asymmetry < -0.01
                ? "R > L"
                : "BALANCED";

        _dynamicsLabel.Text =
            "MODEL DYNAMICS — SYNTHETIC REFERENCE\n" +
            $"φ {phaseDegrees:0}°  |  f {frequency:0.000} Hz  |  " +
            $"ω {omega:0.000} rad/s  |  vL {leftVelocity:+0.0;-0.0;0.0} pp/s\n" +
            $"aL {leftAcceleration:+0.0;-0.0;0.0} pp/s²  |  " +
            $"A {asymmetry:+0.00;-0.00;0.00}  |  {direction}";
    }

    private static string FormatTime(double seconds)
    {
        int totalSeconds = Mathf.FloorToInt((float)seconds);
        int minutes = totalSeconds / 60;
        int remainder = totalSeconds % 60;

        return $"{minutes:00}:{remainder:00}";
    }

    private sealed class PreviewEvent
    {
        public string Schema { get; set; } = "";
        public string EventType { get; set; } = "";
        public string TimestampUtc { get; set; } = "";
        public double ElapsedSeconds { get; set; }
        public string SourceType { get; set; } = "";
        public bool HumanData { get; set; }
        public string Cue { get; set; } = "";
        public double LeftLoadPercent { get; set; }
        public double RightLoadPercent { get; set; }
    }
}

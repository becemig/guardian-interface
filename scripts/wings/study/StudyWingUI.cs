// StudyWingUI.cs -- Guardian Interface Study Wing
// Bidirectional semantic web: symptom<->pattern<->remedy<->discipline
// Inductive (bottom-up) + Deductive (top-down) reasoning display
// Connects to UniversalDockingBus, ContentRegistry, AcademyManager
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GuardianInterface.Wings.Study
{
    public partial class StudyWingUI : Control
    {
        private const string DATA_DIR        = "res://data/";
        private const string STUDY_STATE_PATH = "user://study_state.json";
        private const float  GLOW_SELECTED   = 1.0f;
        private const float  GLOW_CONNECTED  = 0.55f;
        private const float  GLOW_DISTANT    = 0.15f;
        private const float  EMERGENCE_THRESHOLD = 0.6f;
        private const int    MAX_SELECTED    = 8;
        private static readonly Color BG_PRIMARY   = new Color("#0D0F0E");
        private static readonly Color ACCENT_AMBER = new Color("#C9962A");
        private static readonly Color ACCENT_TEAL  = new Color("#2AB8C9");
        private static readonly Color ACCENT_JADE  = new Color("#2AC97A");
        private static readonly Color ACCENT_RED   = new Color("#C92A2A");
        private static readonly Color TEXT_MUTED   = new Color("#7A7874");
        private static readonly Color TEXT_FAINT   = new Color("#404440");
        private static readonly Color TEXT_BRIGHT  = new Color("#CDCCCA");

        public class StudyNode
        {
            public string Id          { get; set; } = "";
            public string Label       { get; set; } = "";
            public string Domain      { get; set; } = "TCM";
            public string Subdomain   { get; set; } = "";
            public string Phase       { get; set; } = "";
            public string Summary     { get; set; } = "";
            public string Detail      { get; set; } = "";
            public string ImageRef    { get; set; } = "";
            public List<string> Symptoms    { get; set; } = new();
            public List<string> Patterns    { get; set; } = new();
            public List<string> Techniques  { get; set; } = new();
            public List<string> Herbs       { get; set; } = new();
            public List<string> Anatomy     { get; set; } = new();
            public List<string> Physiology  { get; set; } = new();
            public List<string> Research    { get; set; } = new();
            public List<string> Disciplines { get; set; } = new();
            public int   TimesVisited    { get; set; } = 0;
            public long  LastVisitedMs   { get; set; } = 0;
            public float Sm2IntervalDays { get; set; } = 1.0f;
            public float Sm2Easiness     { get; set; } = 2.5f;
            public float Mastery         { get; set; } = 0.0f;
            public float GlowAlpha       { get; set; } = 0.15f;
            public Vector2 CanvasPos     { get; set; } = Vector2.Zero;
            public bool IsSelected       { get; set; } = false;
            public List<string> AllEdgeIds()
            {
                var all = new List<string>();
                all.AddRange(Symptoms); all.AddRange(Patterns);
                all.AddRange(Techniques); all.AddRange(Herbs);
                all.AddRange(Anatomy); all.AddRange(Physiology);
                all.AddRange(Research); all.AddRange(Disciplines);
                return all;
            }
        }

        public class EmergenceResult
        {
            public string NodeId       { get; set; } = "";
            public string Label        { get; set; } = "";
            public float  Confidence   { get; set; } = 0f;
            public List<string> SupportingNodes { get; set; } = new();
            public string ReasoningPath{ get; set; } = "";
        }

        public enum ReasoningMode { Inductive, Deductive, Both }

        private static readonly string[] DOMAINS = {
            // TCM CORE
            "TCM", "Materia Medica", "Botany", "Plant ID", "Plant Terminology",
            // BODY SCIENCES
            "Anatomy", "Physiology", "Neurology", "Neurophysics", "Neurochemistry",
            "Pharmacology", "Pharmacokinetics", "Medical Terminology",
            "Microbiology", "Nutrition", "Kinesiology", "Kinesthetics",
            "Exercise Prescription", "Osteopathy", "Biomechanics",
            // LANGUAGE AND LITERACY
            "English Language Arts", "Reading Comprehension",
            "Writing Composition", "Advanced Persuasive Writing",
            "Advanced Analysis Writing", "Advanced Research Writing",
            "Argumentation", "Communication Interpersonal", "Communication Public",
            // LAW AND ADVOCACY
            "Legal Terminology", "Paralegal", "Law", "Real Estate Law",
            "Guardianship Law", "Patients Rights Advocacy",
            // PSYCHOLOGY AND MIND
            "Psychology", "Psychology of Communication", "Talk Therapy",
            "Trauma Awareness", "Cognitive Science",
            // ENGINEERING AND TECHNOLOGY
            "Robotics Engineering", "Mechanics Engineering", "Materials Engineering",
            "Structural Engineering", "Automotive Mechanics", "Motorcycle Mechanics",
            "Aerodynamics", "Aeronautics", "Space Aeronautics",
            "Cybernetics", "Biotech Engineering",
            // PHYSICAL SCIENCES
            "Physics", "Thermophysics", "Nuclear Physics", "Quantum Biology",
            "Chemistry", "Advanced Chemistry", "Medical Chemistry",
            "Herbal Chemistry", "Physics Chemistry", "Chemical Engineering",
            "Mathematics", "Computer Science", "Programming",
            // HUMANITIES
            "Philosophy", "Logic", "Ethics", "History", "Art", "Music"
        };

        private readonly HashSet<string> _activeDomains = new();

        private Dictionary<string, StudyNode> _nodes = new();
        private List<string> _selectedIds            = new();
        private List<EmergenceResult> _emerging      = new();
        private ReasoningMode _reasoningMode         = ReasoningMode.Both;
        private string _activeDomain                 = "TCM";
        private string _activePhase                  = "Water";
        private string _focusedNodeId                = "";
        private Panel       _rootPanel;
        private VBoxContainer _domainNav;
        private Control     _semanticCanvas;
        private Panel       _nodeDetailPanel;
        private Panel       _reasoningPanel;
        private Label       _nodeDetailTitle;
        private Label       _nodeDetailDomain;
        private Label       _nodeDetailPhase;
        private RichTextLabel _nodeDetailSummary;
        private RichTextLabel _reasoningTrace;
        private Label       _emergenceLabel;
        private OptionButton _reasoningToggle;
        private LineEdit    _searchBox;
        private HBoxContainer _domainTray;
        private List<(Vector2 from, Vector2 to, float alpha)> _edgeLines = new();
        private float _canvasTick = 0f;

        public override void _Ready()
        {
            BuildLayout();
            LoadNodesFromRegistry();
            LoadStudyState();
            foreach (string d in DOMAINS) _activeDomains.Add(d);
            RefreshDomainNavigator();
            RefreshDomainTray();
            LayoutCanvasPositions();
            UpdateSemanticCanvas();
            ConnectBusEvents();
        }

        private void ConnectBusEvents()
        {
            var bus = GetNodeOrNull<UniversalDockingBus>("/root/UniversalDockingBus");
            if (bus == null) return;
            bus.Subscribe(BusEvent.PhaseChanged, OnPhaseChanged);
            bus.Subscribe(BusEvent.ModeChanged, OnModeChanged);
            bus.Subscribe(BusEvent.ReasoningModeChanged, OnReasoningModeChanged);
        }

        private void BuildLayout()
        {
            AnchorRight = 1; AnchorBottom = 1;
            _rootPanel = new Panel();
            _rootPanel.AnchorRight = 1; _rootPanel.AnchorBottom = 1;
            _rootPanel.AddThemeStyleboxOverride("panel", MakeFlat(BG_PRIMARY));
            AddChild(_rootPanel);
            _searchBox = new LineEdit();
            _searchBox.PlaceholderText = "Search all disciplines...";
            _searchBox.SetPosition(new Vector2(8, 8));
            _searchBox.SetSize(new Vector2(GetViewportRect().Size.X - 16, 32));
            _searchBox.TextChanged += OnSearchChanged;
            _rootPanel.AddChild(_searchBox);
            float topOffset = 48f;
            float trayH     = 44f;
            float w = GetViewportRect().Size.X;
            float h = GetViewportRect().Size.Y - topOffset - trayH;
            float navW    = 220f;
            float detailH = h * 0.32f;
            float canvasH = h - detailH;
            var navScroll = new ScrollContainer();
            navScroll.SetPosition(new Vector2(0, topOffset));
            navScroll.SetSize(new Vector2(navW, canvasH));
            _rootPanel.AddChild(navScroll);
            _domainNav = new VBoxContainer();
            _domainNav.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            navScroll.AddChild(_domainNav);
            _semanticCanvas = new Control();
            _semanticCanvas.SetPosition(new Vector2(navW, topOffset));
            _semanticCanvas.SetSize(new Vector2(w - navW, canvasH));
            _semanticCanvas.Draw += OnCanvasDraw;
            _rootPanel.AddChild(_semanticCanvas);
            _nodeDetailPanel = new Panel();
            _nodeDetailPanel.SetPosition(new Vector2(0, topOffset + canvasH));
            _nodeDetailPanel.SetSize(new Vector2(w * 0.5f, detailH));
            _nodeDetailPanel.AddThemeStyleboxOverride("panel", MakeFlat(new Color("#13160F")));
            _rootPanel.AddChild(_nodeDetailPanel);
            BuildNodeDetailPanel();
            _reasoningPanel = new Panel();
            _reasoningPanel.SetPosition(new Vector2(w * 0.5f, topOffset + canvasH));
            _reasoningPanel.SetSize(new Vector2(w * 0.5f, detailH));
            _reasoningPanel.AddThemeStyleboxOverride("panel", MakeFlat(new Color("#0F1310")));
            _rootPanel.AddChild(_reasoningPanel);
            BuildReasoningTracePanel();
            _domainTray = new HBoxContainer();
            _domainTray.SetPosition(new Vector2(0, topOffset + canvasH + detailH));
            _domainTray.SetSize(new Vector2(w, trayH));
            _rootPanel.AddChild(_domainTray);
        }

        private void RefreshDomainNavigator()
        {
            foreach (Node c in _domainNav.GetChildren()) c.QueueFree();
            foreach (string d in DOMAINS)
            {
                if (!_activeDomains.Contains(d)) continue;
                var btn = new Button();
                btn.Text = d;
                btn.Flat = true;
                btn.Alignment = HorizontalAlignment.Left;
                btn.AddThemeColorOverride("font_color", d == _activeDomain ? ACCENT_AMBER : TEXT_MUTED);
                string cap = d;
                btn.Pressed += () => OnDomainSelected(cap);
                _domainNav.AddChild(btn);
            }
        }

        private void RefreshDomainTray()
        {
            foreach (Node c in _domainTray.GetChildren()) c.QueueFree();
            var scroll = new ScrollContainer();
            scroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            _domainTray.AddChild(scroll);
            var hbox = new HBoxContainer();
            scroll.AddChild(hbox);
            foreach (string d in DOMAINS)
            {
                bool active = _activeDomains.Contains(d);
                var chip = new Button();
                chip.Text = d;
                chip.ToggleMode = true;
                chip.ButtonPressed = active;
                chip.AddThemeColorOverride("font_color", active ? ACCENT_AMBER : TEXT_FAINT);
                string cap = d;
                chip.Toggled += (on) => OnDomainToggled(cap, on);
                hbox.AddChild(chip);
            }
        }

        private void OnDomainToggled(string domain, bool on)
        {
            if (on) _activeDomains.Add(domain);
            else _activeDomains.Remove(domain);
            RefreshDomainNavigator();
            LayoutCanvasPositions();
            UpdateSemanticCanvas();
        }

        private void BuildNodeDetailPanel()
        {
            var vbox = new VBoxContainer();
            vbox.SetPosition(new Vector2(10, 8));
            vbox.SetSize(new Vector2(_nodeDetailPanel.Size.X - 20, _nodeDetailPanel.Size.Y - 16));
            _nodeDetailPanel.AddChild(vbox);
            _nodeDetailTitle = new Label();
            _nodeDetailTitle.Text = "-- Select a node --";
            _nodeDetailTitle.AddThemeColorOverride("font_color", ACCENT_AMBER);
            _nodeDetailTitle.AddThemeFontSizeOverride("font_size", 16);
            vbox.AddChild(_nodeDetailTitle);
            _nodeDetailDomain = new Label();
            _nodeDetailDomain.AddThemeColorOverride("font_color", ACCENT_TEAL);
            vbox.AddChild(_nodeDetailDomain);
            _nodeDetailPhase = new Label();
            _nodeDetailPhase.AddThemeColorOverride("font_color", TEXT_MUTED);
            vbox.AddChild(_nodeDetailPhase);
            vbox.AddChild(new HSeparator());
            _nodeDetailSummary = new RichTextLabel();
            _nodeDetailSummary.BbcodeEnabled = true;
            _nodeDetailSummary.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            _nodeDetailSummary.AddThemeColorOverride("default_color", TEXT_BRIGHT);
            vbox.AddChild(_nodeDetailSummary);
        }

        private void BuildReasoningTracePanel()
        {
            var vbox = new VBoxContainer();
            vbox.SetPosition(new Vector2(10, 8));
            vbox.SetSize(new Vector2(_reasoningPanel.Size.X - 20, _reasoningPanel.Size.Y - 16));
            _reasoningPanel.AddChild(vbox);
            var headerRow = new HBoxContainer();
            vbox.AddChild(headerRow);
            var traceTitle = new Label();
            traceTitle.Text = "Reasoning Trace";
            traceTitle.AddThemeColorOverride("font_color", ACCENT_JADE);
            traceTitle.AddThemeFontSizeOverride("font_size", 16);
            traceTitle.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            headerRow.AddChild(traceTitle);
            _reasoningToggle = new OptionButton();
            _reasoningToggle.AddItem("Both", 0);
            _reasoningToggle.AddItem("Inductive", 1);
            _reasoningToggle.AddItem("Deductive", 2);
            _reasoningToggle.Selected = 0;
            _reasoningToggle.ItemSelected += OnReasoningToggle;
            headerRow.AddChild(_reasoningToggle);
            _emergenceLabel = new Label();
            _emergenceLabel.Text = "Select nodes to reveal emerging patterns";
            _emergenceLabel.AddThemeColorOverride("font_color", TEXT_MUTED);
            _emergenceLabel.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(_emergenceLabel);
            vbox.AddChild(new HSeparator());
            _reasoningTrace = new RichTextLabel();
            _reasoningTrace.BbcodeEnabled = true;
            _reasoningTrace.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            _reasoningTrace.AddThemeColorOverride("default_color", TEXT_BRIGHT);
            vbox.AddChild(_reasoningTrace);
        }

        private void LoadNodesFromRegistry()
        {
            _nodes.Clear();
            string kmPath = "res://data/knowledge_graph/knowledge_map.json";
            if (FileAccess.FileExists(kmPath)) LoadFromJson(kmPath);
            string studyPath = "res://data/study_nodes.json";
            if (FileAccess.FileExists(studyPath)) LoadFromJson(studyPath);
            if (_nodes.Count == 0) SeedTcmSkeleton();
            GD.Print("[StudyWingUI] Loaded " + _nodes.Count + " nodes");
        }

        private void LoadFromJson(string path)
        {
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            if (file == null) return;
            try
            {
                var doc = JsonDocument.Parse(file.GetAsText());
                var root = doc.RootElement;
                if (root.TryGetProperty("nodes", out var nodesEl))
                    ParseNodeArray(nodesEl);
                else if (root.ValueKind == JsonValueKind.Array)
                    ParseNodeArray(root);
            }
            catch (Exception e)
            { GD.PrintErr("[StudyWingUI] JSON error: " + e.Message); }
        }

        private void ParseNodeArray(JsonElement arr)
        {
            foreach (var el in arr.EnumerateArray())
            {
                var n = new StudyNode();
                if (el.TryGetProperty("id",        out var v)) n.Id        = v.GetString() ?? "";
                if (el.TryGetProperty("label",     out v))     n.Label     = v.GetString() ?? n.Id;
                if (el.TryGetProperty("domain",    out v))     n.Domain    = v.GetString() ?? "TCM";
                if (el.TryGetProperty("subdomain", out v))     n.Subdomain = v.GetString() ?? "";
                if (el.TryGetProperty("phase",     out v))     n.Phase     = v.GetString() ?? "";
                if (el.TryGetProperty("summary",   out v))     n.Summary   = v.GetString() ?? "";
                if (el.TryGetProperty("detail",    out v))     n.Detail    = v.GetString() ?? "";
                if (el.TryGetProperty("image_ref", out v))     n.ImageRef  = v.GetString() ?? "";
                ReadStringList(el, "symptoms",    n.Symptoms);
                ReadStringList(el, "patterns",    n.Patterns);
                ReadStringList(el, "techniques",  n.Techniques);
                ReadStringList(el, "herbs",       n.Herbs);
                ReadStringList(el, "anatomy",     n.Anatomy);
                ReadStringList(el, "physiology",  n.Physiology);
                ReadStringList(el, "research",    n.Research);
                ReadStringList(el, "disciplines", n.Disciplines);
                if (n.Id != "") _nodes[n.Id] = n;
            }
        }

        private static void ReadStringList(JsonElement el, string key, List<string> target)
        {
            if (el.TryGetProperty(key, out var arr) && arr.ValueKind == JsonValueKind.Array)
                foreach (var item in arr.EnumerateArray())
                { string s = item.GetString() ?? ""; if (s != "") target.Add(s); }
        }

        private void SeedTcmSkeleton()
        {
            string[] phases = { "Water", "Wood", "Fire", "Earth", "Metal" };
            string[] organs = { "Kidney/Bladder", "Liver/Gallbladder",
                                "Heart/SmallIntestine", "Spleen/Stomach",
                                "Lung/LargeIntestine" };
            for (int i = 0; i < phases.Length; i++)
            {
                var n = new StudyNode {
                    Id = "phase_" + phases[i], Label = phases[i] + " Phase",
                    Domain = "TCM", Subdomain = "WuXing", Phase = phases[i],
                    Summary = organs[i] + " organ system."
                };
                _nodes[n.Id] = n;
            }
        }

        private void LayoutCanvasPositions()
        {
            if (_semanticCanvas == null) return;
            var size = _semanticCanvas.Size;
            var list = _nodes.Values
                .Where(n => _activeDomains.Contains(n.Domain)).ToList();
            if (list.Count == 0) return;
            float cx = size.X * 0.5f;
            float cy = size.Y * 0.5f;
            float radius = Mathf.Min(size.X, size.Y) * 0.38f;
            for (int i = 0; i < list.Count; i++)
            {
                float angle = (float)(2.0 * Math.PI * i / list.Count);
                float r = radius * (1.0f - list[i].Mastery * 0.5f);
                list[i].CanvasPos = new Vector2(
                    cx + r * Mathf.Cos(angle), cy + r * Mathf.Sin(angle));
            }
        }

        private void OnNodeClicked(string nodeId, bool additive)
        {
            if (!_nodes.TryGetValue(nodeId, out var node)) return;
            if (additive)
            {
                if (_selectedIds.Contains(nodeId))
                { _selectedIds.Remove(nodeId); node.IsSelected = false; }
                else if (_selectedIds.Count < MAX_SELECTED)
                { _selectedIds.Add(nodeId); node.IsSelected = true; }
            }
            else
            {
                foreach (var id in _selectedIds)
                    if (_nodes.TryGetValue(id, out var old)) old.IsSelected = false;
                _selectedIds.Clear();
                _selectedIds.Add(nodeId); node.IsSelected = true;
            }
            _focusedNodeId = nodeId;
            node.TimesVisited++;
            node.LastVisitedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            EmitStudyBusEvents(node);
            _emerging = ComputePatternEmergence();
            UpdateSemanticCanvas();
            UpdateNodeDetail(node);
            UpdateReasoningTrace();
        }

        private void EmitStudyBusEvents(StudyNode node)
        {
            var bus = GetNodeOrNull<UniversalDockingBus>("/root/UniversalDockingBus");
            if (bus == null) return;
            if (node.Subdomain == "Acupoints")
                bus.Publish(BusEvent.AcupointQueried, new object[]{node.Id, node.Label});
            if (node.Subdomain == "Materia Medica")
                bus.Publish(BusEvent.HerbQueried, new object[]{node.Id, node.Label});
            if (node.Subdomain == "Patterns")
                bus.Publish(BusEvent.PatternSelected, node.Id);
        }

        private void OnDomainSelected(string domain)
        {
            _activeDomain = domain;
            foreach (var n in _nodes.Values) n.IsSelected = false;
            _selectedIds.Clear(); _emerging.Clear();
            RefreshDomainNavigator();
            LayoutCanvasPositions();
            UpdateSemanticCanvas();
            if (_reasoningTrace != null) _reasoningTrace.Text = "";
            if (_emergenceLabel != null)
                _emergenceLabel.Text = "Select nodes to reveal emerging patterns";
        }

        private void OnSearchChanged(string query)
        {
            if (query.Length < 2) { RefreshDomainNavigator(); return; }
            string q = query.ToLower();
            var matches = _nodes.Values
                .Where(n => n.Label.ToLower().Contains(q) ||
                            n.Domain.ToLower().Contains(q) ||
                            n.Summary.ToLower().Contains(q)).ToList();
            foreach (Node c in _domainNav.GetChildren()) c.QueueFree();
            var header = new Label();
            header.Text = matches.Count + " results: " + query;
            header.AddThemeColorOverride("font_color", ACCENT_TEAL);
            _domainNav.AddChild(header);
            foreach (var n in matches.Take(40))
            {
                var btn = new Button();
                btn.Text = n.Label + " [" + n.Domain + "]";
                btn.Flat = true;
                btn.Alignment = HorizontalAlignment.Left;
                btn.AddThemeColorOverride("font_color", TEXT_BRIGHT);
                string cap = n.Id;
                btn.Pressed += () => OnNodeClicked(cap, false);
                _domainNav.AddChild(btn);
            }
        }

        private List<EmergenceResult> ComputePatternEmergence()
        {
            var results = new List<EmergenceResult>();
            if (_selectedIds.Count == 0) return results;
            var neighborScore = new Dictionary<string, int>();
            var supportMap    = new Dictionary<string, List<string>>();
            foreach (string selId in _selectedIds)
            {
                if (!_nodes.TryGetValue(selId, out var selNode)) continue;
                foreach (string edgeId in selNode.AllEdgeIds())
                {
                    if (_selectedIds.Contains(edgeId)) continue;
                    if (!neighborScore.ContainsKey(edgeId))
                    { neighborScore[edgeId] = 0; supportMap[edgeId] = new List<string>(); }
                    neighborScore[edgeId]++;
                    supportMap[edgeId].Add(selNode.Label);
                }
            }
            int total = _selectedIds.Count;
            foreach (var kvp in neighborScore)
            {
                float confidence = (float)kvp.Value / total;
                if (confidence < EMERGENCE_THRESHOLD) continue;
                if (!_nodes.TryGetValue(kvp.Key, out var candidate)) continue;
                results.Add(new EmergenceResult {
                    NodeId = kvp.Key, Label = candidate.Label,
                    Confidence = confidence,
                    SupportingNodes = supportMap[kvp.Key],
                    ReasoningPath = BuildReasoningPath(candidate, supportMap[kvp.Key])
                });
            }
            results.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));
            return results;
        }

        private string BuildReasoningPath(StudyNode candidate, List<string> supporting)
        {
            string mode = _reasoningMode switch {
                ReasoningMode.Inductive => "Bottom-Up",
                ReasoningMode.Deductive => "Top-Down",
                _                       => "Bidirectional"
            };
            string s = string.Join(" + ", supporting.Take(4));
            return mode + ": [" + s + "] -> " + candidate.Label +
                   " (" + candidate.Domain + "/" + candidate.Subdomain + ")";
        }

        private void UpdateSemanticCanvas()
        {
            var emergingIds = new HashSet<string>(_emerging.Select(e => e.NodeId));
            foreach (var n in _nodes.Values)
            {
                if (n.IsSelected)                    n.GlowAlpha = GLOW_SELECTED;
                else if (emergingIds.Contains(n.Id)) n.GlowAlpha = GLOW_CONNECTED + 0.2f;
                else if (_selectedIds.Count > 0 && IsConnectedToSelection(n))
                                                     n.GlowAlpha = GLOW_CONNECTED;
                else                                 n.GlowAlpha = GLOW_DISTANT;
            }
            _edgeLines.Clear();
            foreach (string selId in _selectedIds)
            {
                if (!_nodes.TryGetValue(selId, out var selNode)) continue;
                foreach (string edgeId in selNode.AllEdgeIds())
                {
                    if (!_nodes.TryGetValue(edgeId, out var en)) continue;
                    float a = emergingIds.Contains(edgeId) ? 0.75f : 0.3f;
                    _edgeLines.Add((selNode.CanvasPos, en.CanvasPos, a));
                }
            }
            _semanticCanvas?.QueueRedraw();
        }

        private bool IsConnectedToSelection(StudyNode n)
        {
            var edges = n.AllEdgeIds();
            return _selectedIds.Any(id => edges.Contains(id));
        }

        private void OnCanvasDraw()
        {
            if (_semanticCanvas == null) return;
            foreach (var (from, to, alpha) in _edgeLines)
            {
                if (from == Vector2.Zero || to == Vector2.Zero) continue;
                _semanticCanvas.DrawLine(from, to,
                    new Color(ACCENT_TEAL.R, ACCENT_TEAL.G, ACCENT_TEAL.B, alpha), 1.2f);
            }
            var visible = _nodes.Values
                .Where(n => _activeDomains.Contains(n.Domain)).ToList();
            foreach (var n in visible)
            {
                if (n.CanvasPos == Vector2.Zero) continue;
                Color nc = PhaseColor(n.Phase);
                float r = 8f + n.Mastery * 6f;
                _semanticCanvas.DrawCircle(n.CanvasPos, r,
                    new Color(nc.R, nc.G, nc.B, n.GlowAlpha));
                if (n.GlowAlpha > 0.4f)
                    _semanticCanvas.DrawString(ThemeDB.FallbackFont,
                        n.CanvasPos + new Vector2(r + 3f, 4f),
                        n.Label.Length > 18 ? n.Label[..18] + "..." : n.Label,
                        HorizontalAlignment.Left, -1, 11,
                        new Color(TEXT_BRIGHT.R, TEXT_BRIGHT.G, TEXT_BRIGHT.B, n.GlowAlpha));
            }
        }

        private void UpdateNodeDetail(StudyNode node)
        {
            if (_nodeDetailTitle == null) return;
            _nodeDetailTitle.Text  = node.Label;
            _nodeDetailDomain.Text = node.Domain + " / " + node.Subdomain;
            _nodeDetailPhase.Text  = node.Phase != "" ?
                "Phase: " + node.Phase + "  Mastery: " +
                (node.Mastery * 100f).ToString("F0") + "%" : "";
            string body = "[color=#CDCCCA]" + node.Summary + "[/color]";
            if (node.Symptoms.Count    > 0) body += "\n[color=#C9962A]Symptoms:[/color] "    + string.Join(", ", node.Symptoms.Take(6));
            if (node.Patterns.Count    > 0) body += "\n[color=#2AB8C9]Patterns:[/color] "    + string.Join(", ", node.Patterns.Take(6));
            if (node.Herbs.Count       > 0) body += "\n[color=#2AC97A]Herbs:[/color] "       + string.Join(", ", node.Herbs.Take(6));
            if (node.Anatomy.Count     > 0) body += "\n[color=#7A7874]Anatomy:[/color] "     + string.Join(", ", node.Anatomy.Take(4));
            if (node.Disciplines.Count > 0) body += "\n[color=#C9962A]Disciplines:[/color] " + string.Join(", ", node.Disciplines.Take(6));
            if (node.Research.Count    > 0) body += "\n[color=#404440]Research:[/color] "    + string.Join(", ", node.Research.Take(3));
            _nodeDetailSummary.Text = body;
        }

        private void UpdateReasoningTrace()
        {
            if (_reasoningTrace == null) return;
            if (_selectedIds.Count == 0)
            {
                _reasoningTrace.Text = "";
                _emergenceLabel.Text = "Select nodes to reveal emerging patterns";
                return;
            }
            var selected = _selectedIds.Where(id => _nodes.ContainsKey(id))
                .Select(id => _nodes[id].Label).ToList();
            string selText = "[color=#C9962A]Selected:[/color] " + string.Join(" + ", selected);
            if (_emerging.Count == 0)
            {
                _emergenceLabel.Text = "No strong patterns yet -- select more nodes";
                _reasoningTrace.Text = selText;
                return;
            }
            _emergenceLabel.Text = _emerging.Count + " pattern(s) emerging";
            string trace = selText + "\n\n";
            foreach (var er in _emerging.Take(8))
            {
                string pct = (er.Confidence * 100f).ToString("F0");
                trace += "[color=#2AC97A]" + er.Label + "[/color]";
                trace += " [color=#7A7874](" + pct + "%)[/color]\n";
                trace += "[color=#404440]" + er.ReasoningPath + "[/color]\n\n";
            }
            _reasoningTrace.Text = trace;
        }

        private void OnPhaseChanged(object raw)
        { _activePhase = raw as string ?? ""; _semanticCanvas?.QueueRedraw(); }

        private void OnModeChanged(object raw) { Visible = true; }

        private void OnReasoningModeChanged(object raw)
        {
            var modeStr = raw as string ?? "";
            _reasoningMode = modeStr switch {
                "Inductive" => ReasoningMode.Inductive,
                "Deductive" => ReasoningMode.Deductive,
                _           => ReasoningMode.Both
            };
            _emerging = ComputePatternEmergence();
            UpdateReasoningTrace();
        }

        private void OnReasoningToggle(long idx)
        {
            _reasoningMode = (int)idx switch {
                1 => ReasoningMode.Inductive,
                2 => ReasoningMode.Deductive,
                _ => ReasoningMode.Both
            };
            _emerging = ComputePatternEmergence();
            UpdateReasoningTrace();
        }

        private void PersistStudyState()
        {
            var stateMap = new Dictionary<string, object>();
            foreach (var kvp in _nodes)
            {
                var n = kvp.Value;
                if (n.TimesVisited == 0) continue;
                stateMap[n.Id] = new { times_visited = n.TimesVisited,
                    last_visited_ms = n.LastVisitedMs,
                    sm2_interval = n.Sm2IntervalDays,
                    sm2_easiness = n.Sm2Easiness,
                    mastery = n.Mastery };
            }
            string json = JsonSerializer.Serialize(stateMap,
                new JsonSerializerOptions { WriteIndented = true });
            using var file = FileAccess.Open(STUDY_STATE_PATH, FileAccess.ModeFlags.Write);
            if (file != null) file.StoreString(json);
        }

        private void LoadStudyState()
        {
            if (!FileAccess.FileExists(STUDY_STATE_PATH)) return;
            using var file = FileAccess.Open(STUDY_STATE_PATH, FileAccess.ModeFlags.Read);
            if (file == null) return;
            try
            {
                var doc = JsonDocument.Parse(file.GetAsText());
                foreach (var entry in doc.RootElement.EnumerateObject())
                {
                    if (!_nodes.TryGetValue(entry.Name, out var n)) continue;
                    var v = entry.Value;
                    if (v.TryGetProperty("times_visited",   out var tv)) n.TimesVisited    = tv.GetInt32();
                    if (v.TryGetProperty("last_visited_ms", out var lv)) n.LastVisitedMs   = lv.GetInt64();
                    if (v.TryGetProperty("mastery",         out var mv)) n.Mastery         = mv.GetSingle();
                    if (v.TryGetProperty("sm2_interval",    out var si)) n.Sm2IntervalDays = si.GetSingle();
                    if (v.TryGetProperty("sm2_easiness",    out var se)) n.Sm2Easiness     = se.GetSingle();
                }
            }
            catch (Exception e)
            { GD.PrintErr("[StudyWingUI] State load error: " + e.Message); }
        }

        public override void _Process(double delta)
        {
            _canvasTick += (float)delta;
            if (_canvasTick > 0.5f)
            { _canvasTick = 0f;
              if (_selectedIds.Count > 0) _semanticCanvas?.QueueRedraw(); }
        }

        public override void _Input(InputEvent evt)
        {
            if (evt is not InputEventMouseButton mb) return;
            if (!mb.Pressed || mb.ButtonIndex != MouseButton.Left) return;
            if (_semanticCanvas == null) return;
            Vector2 localPos = _semanticCanvas.GetLocalMousePosition();
            bool additive = Input.IsKeyPressed(Key.Shift);
            foreach (var n in _nodes.Values.Where(n => _activeDomains.Contains(n.Domain)))
            {
                if (n.CanvasPos == Vector2.Zero) continue;
                if (localPos.DistanceTo(n.CanvasPos) <= 10f + n.Mastery * 6f)
                { OnNodeClicked(n.Id, additive); break; }
            }
        }

        public override void _Notification(int what)
        {
            if (what == NotificationWMCloseRequest || what == NotificationExitTree)
                PersistStudyState();
        }

        private static StyleBoxFlat MakeFlat(Color col)
        { var sb = new StyleBoxFlat(); sb.BgColor = col; return sb; }

        private Color PhaseColor(string phase) => phase switch {
            "Water" => new Color("#2AB8C9"),
            "Wood"  => new Color("#2AC97A"),
            "Fire"  => new Color("#C92A2A"),
            "Earth" => new Color("#C9962A"),
            "Metal" => new Color("#CDCCCA"),
            _       => new Color("#7A7874")
        };
    }
}

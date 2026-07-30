// TCMAnnotationPanel.cs -- Guardian Interface
// CanvasLayer HUD panel showing full TCM context annotation per frame.
// Subscribes to TCMContextResolver.TCMContextResolved signal.
//
using Godot;

[GlobalClass]
public partial class TCMAnnotationPanel : CanvasLayer
{
    private PanelContainer _panel;
    private VBoxContainer _vbox;
    private Label _channel;
    private Label _element;
    private Label _tissue;
    private Label _neuro;
    private Label _horary;
    private Label _koSheng;
    private Label _qigong;
    private Label _nutrition;
    private Label _acupoints;

    // Lerp alpha for fade-in on update
    private float _alpha = 0.0f;
    private bool _dirty = false;
    public override void _Ready()
    {
        Layer = 10;
        _panel = new PanelContainer();
        _panel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _panel.Position = new Vector2(-320, 12);
        _panel.CustomMinimumSize = new Vector2(300, 0);
        var style = new StyleBoxFlat();
        style.BgColor = new Color(0.04f, 0.04f, 0.06f, 0.82f);
        style.BorderColor = new Color(0.25f, 0.45f, 0.42f, 0.70f);
        style.SetBorderWidthAll(1);
        style.SetCornerRadiusAll(6);
        style.SetContentMarginAll(12);
        _panel.AddThemeStyleboxOverride("panel", style);
        AddChild(_panel);
        _vbox = new VBoxContainer();
        _vbox.AddThemeConstantOverride("separation", 4);
        _panel.AddChild(_vbox);
        _channel   = MakeLabel("", 15, new Color(0.55f, 0.90f, 0.85f));
        _element   = MakeLabel("", 13, new Color(0.75f, 0.75f, 0.60f));
        _horary    = MakeLabel("", 11, new Color(0.60f, 0.70f, 0.80f));
        _koSheng   = MakeLabel("", 10, new Color(0.65f, 0.55f, 0.75f));
        _tissue    = MakeLabel("", 10, new Color(0.80f, 0.72f, 0.60f));
        _neuro     = MakeLabel("", 10, new Color(0.60f, 0.78f, 0.65f));
        _qigong    = MakeLabel("", 10, new Color(0.72f, 0.80f, 0.72f));
        _nutrition = MakeLabel("", 10, new Color(0.78f, 0.82f, 0.58f));
        _acupoints = MakeLabel("", 10, new Color(0.70f, 0.65f, 0.80f));
        // reorder vbox: channel, element, sep, horary, koSheng, sep, tissue, neuro, sep, qigong, sep, nutrition, acupoints
        _vbox.MoveChild(_horary,    2);
        _vbox.MoveChild(_koSheng,   3);
        MakeSeparator();
        _vbox.MoveChild(_tissue,    5);
        _vbox.MoveChild(_neuro,     6);
        MakeSeparator();
        _vbox.MoveChild(_qigong,    8);
        MakeSeparator();
        _vbox.MoveChild(_nutrition, 10);
        _vbox.MoveChild(_acupoints, 11);
        // connect to TCMContextResolver
        var resolver = GetTree().Root.FindChild("TCMContextResolver", true, false);
        if (resolver != null)
            resolver.Connect("TCMContextResolved", new Callable(this, nameof(OnTCMContext)));
        else
            GD.PrintErr("[TCMAnnotationPanel] TCMContextResolver not found");
        _panel.Modulate = new Color(1, 1, 1, 0);
    }
    private Label MakeLabel(string text, int size, Color color)
    {
        var lbl = new Label();
        lbl.Text = text;
        lbl.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        lbl.AddThemeFontSizeOverride("font_size", size);
        lbl.AddThemeColorOverride("font_color", color);
        _vbox.AddChild(lbl);
        return lbl;
    }

    private Label MakeSeparator()
    {
        var lbl = new Label();
        lbl.Text = "─────────────────────────";
        lbl.AddThemeFontSizeOverride("font_size", 8);
        lbl.AddThemeColorOverride("font_color", new Color(0.30f, 0.35f, 0.32f));
        _vbox.AddChild(lbl);
        return lbl;
    }
    private void OnTCMContext(string channel, string element, string tissue,
        string neuroscience, string horary, string nutrition,
        string acupoints, string koSheng, string qigongForm)
    {
        _channel.Text   = channel + "  |  " + element;
        _element.Text   = element + " element";
        _horary.Text    = "Horary: " + horary;
        _koSheng.Text   = "Cycle:  " + koSheng;
        _tissue.Text    = "Tissue: " + tissue;
        _neuro.Text     = "Neuro:  " + neuroscience;
        _qigong.Text    = "Qigong: " + qigongForm;
        _nutrition.Text = "Food:   " + nutrition;
        _acupoints.Text = "Acu:    " + acupoints;
        _dirty = true;
    }
    public override void _Process(double delta)
    {
        if (_dirty)
        {
            _alpha = Mathf.Lerp(_alpha, 1.0f, (float)delta * 3.0f);
            _panel.Modulate = new Color(1, 1, 1, _alpha);
            if (_alpha > 0.98f) { _alpha = 1.0f; _dirty = false; }
        }
    }
}

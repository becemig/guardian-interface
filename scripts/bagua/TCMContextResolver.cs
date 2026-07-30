// TCMContextResolver.cs -- Guardian Interface
// Annotates each VolResolved frame with full TCM theoretical context.
// Tissue, neuroscience, Ko/Sheng cycle, horary clock, nutrition, acupoints.
//
using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class TCMContextResolver : Node
{
    [Signal] public delegate void TCMContextResolvedEventHandler(
        string channel, string element, string tissue,
        string neuroscience, string horary, string nutrition,
        string acupoints, string koSheng, string qigongForm);

    // --- Tissue governed by each channel ---
    private static readonly Dictionary<string, string> Tissue = new()
    {
        {"BL","spine, posterior chain, occiput"},
        {"KD","bones, marrow, adrenal cortex, brain"},
        {"GB","tendons, ligament insertions, lateral fascia"},
        {"LR","joint capsules, eyes, blood storage"},
        {"ST","stomach wall, anterior fascia, quadriceps"},
        {"SP","muscle belly, myofascial sheaths, visceral ligaments"},
        {"SI","scapular stabilizers, small intestine wall"},
        {"HT","cardiac muscle, blood vessels, tongue"},
        {"TB","fascia lata, lateral arm, triple burner membrane"},
        {"LI","large intestine, skin of arm, neck fascia"},
        {"LU","lung parenchyma, skin, body hair, wei qi barrier"},
        {"PC","pericardium, chest fascia, deep anterior arm"},
    };
    private static readonly Dictionary<string, string> Neuroscience = new()
    {
        {"BL","HPA axis, hippocampal neurogenesis, spinal proprioception, fear circuits"},
        {"KD","adrenal-cortisol axis, bone piezoelectricity, hippocampal volume, willpower circuits"},
        {"GB","cerebellum feedforward planning, basal ganglia timing, lateral vestibular"},
        {"LR","basal ganglia motor sequencing, anterior cingulate, decision under uncertainty"},
        {"ST","enteric nervous system, vagal afferents, insular cortex body map"},
        {"SP","interoception, insular cortex, gut-brain axis, vagal tone"},
        {"SI","dorsal attention network, somatosensory discrimination, scapular motor control"},
        {"HT","heart rate variability, limbic integration, prefrontal-amygdala coherence"},
        {"TB","autonomic balance, social engagement system, polyvagal ventral vagal"},
        {"LI","somatosensory cortex arm map, mucosal immunity, respiratory rhythm"},
        {"LU","respiratory pacemaker, hippocampal theta entrainment, vagal parasympathetic"},
        {"PC","prefrontal-limbic coherence, cardiac interoception, relational attunement"},
    };
    private static readonly Dictionary<string, string> Nutrition = new()
    {
        {"BL","black sesame, bone broth, miso, kidney beans, walnuts"},
        {"KD","black foods, bone broth, seaweed, chestnuts, lamb kidney"},
        {"GB","sour foods, mung beans, leafy greens, lemon, vinegar"},
        {"LR","bitter greens, beets, turmeric, milk thistle, brassicas"},
        {"ST","yellow foods, millet, squash, sweet potato, congee"},
        {"SP","root vegetables, millet, legumes, moderate sweet, cooked warm foods"},
        {"SI","bitter foods, red berries, oats, whole grains, leafy reds"},
        {"HT","red foods, hawthorn berry, jujube, longan, bitter greens"},
        {"TB","warming foods, ginger, scallion, moderate spice, bone broth"},
        {"LI","pungent foods, daikon, pear, white fungus, rice"},
        {"LU","white pungent foods, pear, daikon, lotus root, white mushroom"},
        {"PC","hawthorn, rose hip, cacao, red wine moderate, heart-nourishing broths"},
    };

    private static readonly Dictionary<string, string> Acupoints = new()
    {
        {"BL","BL23 Shenshu (lumbar), BL40 Weizhong (knee), BL60 Kunlun (ankle)"},
        {"KD","KD1 Yongquan (sole), KD3 Taixi (ankle), KD7 Fuliu (lower leg)"},
        {"GB","GB34 Yanglingquan (knee), GB41 Zulinqi (foot), GB21 Jianjing (shoulder)"},
        {"LR","LR3 Taichong (foot), LR8 Ququan (knee), LR14 Qimen (chest)"},
        {"ST","ST36 Zusanli (knee), ST40 Fenglong (leg), ST44 Neiting (foot)"},
        {"SP","SP6 Sanyinjiao (ankle), SP9 Yinlingquan (knee), SP21 Dabao (chest)"},
        {"SI","SI3 Houxi (hand), SI8 Xiaohai (elbow), SI11 Tianzong (scapula)"},
        {"HT","HT7 Shenmen (wrist), HT3 Shaohai (elbow), HT1 Jiquan (axilla)"},
        {"TB","TB5 Waiguan (forearm), TB14 Jianliao (shoulder), TB23 Sizhukong (temple)"},
        {"LI","LI4 Hegu (hand), LI11 Quchi (elbow), LI15 Jianyu (shoulder)"},
        {"LU","LU7 Lieque (wrist), LU5 Chize (elbow), LU1 Zhongfu (chest)"},
        {"PC","PC6 Neiguan (wrist), PC3 Quze (elbow), PC1 Tianchi (chest)"},
    };
    private static readonly Dictionary<string, string> QigongForm = new()
    {
        {"BL","spinal wave, standing post, gathering downward, swimming dragon"},
        {"KD","kidney nourishing, rooting, wave spine, turtle breathing"},
        {"GB","lateral stretch, wood chopping, bamboo in wind, lateral silk reeling"},
        {"LR","tendon stretching, Yi Jin Jing, eye gazing, spring liver flush"},
        {"ST","bear grounding, earth stamping, abdominal breathing, harvest gathering"},
        {"SP","slow circular arm, grounding step, holding ball, Earth silk reeling"},
        {"SI","shoulder blade retraction, heart-small intestine pair, fire breathing"},
        {"HT","heart opening, arms expanding upward, golden retrieval, joy cultivation"},
        {"TB","three burner regulation, lifting sky, shaking tree, full body wave"},
        {"LI","metal drawing bow, grief release, lung clearing, large intestine flush"},
        {"LU","lung expansion, drawing bow, autumn harvest, skin breathing awareness"},
        {"PC","pericardium protection, heart wrapping, inner smile, relational opening"},
    };

    // Ko cycle: key -> what it controls
    private static readonly Dictionary<string, string[]> KoControls = new()
    {
        {"Wood", new[]{"Earth"}},
        {"Earth", new[]{"Water"}},
        {"Water", new[]{"Fire"}},
        {"Fire", new[]{"Metal"}},
        {"Metal", new[]{"Wood"}},
    };

    // Sheng cycle: key -> what it nourishes
    private static readonly Dictionary<string, string[]> ShengNourishes = new()
    {
        {"Wood", new[]{"Fire"}},
        {"Fire", new[]{"Earth"}},
        {"Earth", new[]{"Metal"}},
        {"Metal", new[]{"Water"}},
        {"Water", new[]{"Wood"}},
    };
    // Horary clock: hour (0-23) -> peak channel
    private static readonly string[] HoraryClock = {
        "GB","GB","LR","LR","LU","LU","LI","LI",
        "ST","ST","SP","SP","HT","HT","SI","SI",
        "BL","BL","KD","KD","PC","PC","TB","TB"
    };

    private static readonly Dictionary<string, string> ChannelElement = new()
    {
        {"BL","Water"},{"KD","Water"},
        {"GB","Wood"}, {"LR","Wood"},
        {"ST","Earth"},{"SP","Earth"},
        {"SI","Fire"}, {"HT","Fire"},
        {"TB","Fire"}, {"PC","Fire"},
        {"LI","Metal"},{"LU","Metal"},
    };
    public override void _Ready()
    {
        var resolver = GetTree().Root.FindChild("KappaAtlasResolver", true, false);
        if (resolver != null)
            resolver.Connect("VolResolved", new Callable(this, nameof(OnVolResolved)));
        else
            GD.PrintErr("[TCMContextResolver] KappaAtlasResolver not found");
        GD.Print("[TCMContextResolver] Ready");
    }

    private void OnVolResolved(int volId, int l1, int l2, int l3,
        string channel, string element, int jointIdx)
    {
        string tissue = Tissue.ContainsKey(channel) ? Tissue[channel] : "unknown";
        string neuro = Neuroscience.ContainsKey(channel) ? Neuroscience[channel] : "unknown";
        string nutrition = Nutrition.ContainsKey(channel) ? Nutrition[channel] : "unknown";
        string acu = Acupoints.ContainsKey(channel) ? Acupoints[channel] : "unknown";
        string qigong = QigongForm.ContainsKey(channel) ? QigongForm[channel] : "unknown";
        string horary = GetHoraryStatus(channel);
        string koSheng = GetKoShengStatus(element);
        EmitSignal(SignalName.TCMContextResolved,
            channel, element, tissue, neuro, horary, nutrition, acu, koSheng, qigong);
    }
    private string GetHoraryStatus(string channel)
    {
        int hour = DateTime.Now.Hour;
        string peak = HoraryClock[hour];
        int peakHour = Array.IndexOf(HoraryClock, channel);
        if (peak == channel)
            return channel + " PEAK hour (" + hour + ":00-" + (hour+1) + ":00)";
        int diff = Math.Abs(hour - peakHour);
        if (diff <= 2 || diff >= 22)
            return channel + " building toward peak (peak @ " + peakHour + ":00)";
        return channel + " subdued -- " + peak + " peak now";
    }

    private string GetKoShengStatus(string element)
    {
        string controls = "";
        string nourishes = "";
        string controlledBy = "";
        string nourishment = "";
        if (KoControls.ContainsKey(element))
            controls = element + " controls " + string.Join(",", KoControls[element]);
        if (ShengNourishes.ContainsKey(element))
            nourishes = element + " nourishes " + string.Join(",", ShengNourishes[element]);
        foreach (var kv in KoControls)
            if (Array.IndexOf(kv.Value, element) >= 0)
                controlledBy = kv.Key + " controls " + element;
        foreach (var kv in ShengNourishes)
            if (Array.IndexOf(kv.Value, element) >= 0)
                nourishment = kv.Key + " nourishes " + element;
        return controls + " | " + nourishes + " | " + controlledBy + " | " + nourishment;
    }
}

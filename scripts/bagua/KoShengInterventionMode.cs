// KoShengInterventionMode.cs -- Guardian Interface
// Monitors element dominance across frames.
// When one element holds 10+ consecutive frames, emits intervention recommendations.
// Ko cycle: direct restraint. Sheng mother: root nourishment.
//
using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class KoShengInterventionMode : Node
{
    [Signal] public delegate void InterventionRecommendedEventHandler(
        string dominantElement, string dominantChannel,
        string koChannel, string motherChannel,
        string koRationale, string motherRationale,
        int streakCount);

    private const int STREAK_THRESHOLD = 10;

    private string _currentElement = "";
    private string _currentChannel = "";
    private int _streakCount = 0;
    private bool _interventionActive = false;

    // Ko cycle: dominant element -> which element directly restrains it
    private static readonly Dictionary<string, string> KoRestrains = new()
    {
        {"Wood",  "Metal"},
        {"Fire",  "Water"},
        {"Earth", "Wood"},
        {"Metal", "Fire"},
        {"Water", "Earth"},
    };

    // Sheng mother: dominant element -> its mother (nourishing source)
    // Nourishing the mother calms the child (indirect)
    private static readonly Dictionary<string, string> ShengMother = new()
    {
        {"Wood",  "Water"},
        {"Fire",  "Wood"},
        {"Earth", "Fire"},
        {"Metal", "Earth"},
        {"Water", "Metal"},
    };
    // Primary channel to activate for each element
    private static readonly Dictionary<string, string> ElementChannel = new()
    {
        {"Wood",  "GB/LR -- Yi Jin Jing, lateral stretch, tendon release"},
        {"Fire",  "HT/PC -- heart opening, arms expanding, joy cultivation"},
        {"Earth", "SP/ST -- bear grounding, slow circular, abdominal breath"},
        {"Metal", "LU/LI -- drawing bow, grief release, lung expansion"},
        {"Water", "KD/BL -- spinal wave, standing post, gathering downward"},
    };

    // Ko rationale: why this restraint is indicated
    private static readonly Dictionary<string, string> KoRationale = new()
    {
        {"Wood",  "Metal chops Wood -- LU/LI practice draws qi inward, restrains lateral expansion, calms tendon hypertonicity and agitation"},
        {"Fire",  "Water controls Fire -- KD/BL practice roots downward, cools excess activation, steadies HRV and limbic excitation"},
        {"Earth", "Wood controls Earth -- GB/LR lateral movement breaks rumination loop, moves stagnant muscle qi, sharpens proprioception"},
        {"Metal", "Fire controls Metal -- HT/PC heart opening dissolves grief rigidity, warms wei qi boundary, restores relational tone"},
        {"Water", "Earth controls Water -- SP/ST grounding practice builds center, counters fear paralysis, restores adrenal rhythm"},
    };

    // Mother rationale: why nourishing the mother helps
    private static readonly Dictionary<string, string> MotherRationale = new()
    {
        {"Wood",  "Water nourishes Wood -- KD/BL practice roots the excess, nourishes bone marrow and adrenal reserve beneath tendon drive"},
        {"Fire",  "Wood nourishes Fire -- GB/LR gentle stretch feeds Heart without forcing calm, restores rhythmic flow"},
        {"Earth", "Fire nourishes Earth -- HT/PC warmth and joy dissolve worry loop, activates gut-brain vagal coherence"},
        {"Metal", "Earth nourishes Metal -- SP/ST grounding provides substrate for lung expansion, supports wei qi through digestion"},
        {"Water", "Metal nourishes Water -- LU/LI deep breath fills kidney qi, respiratory rhythm stabilizes HPA axis"},
    };
    public override void _Ready()
    {
        var tcm = GetTree().Root.FindChild("TCMContextResolver", true, false);
        if (tcm != null)
            tcm.Connect("TCMContextResolved", new Callable(this, nameof(OnTCMContext)));
        else
            GD.PrintErr("[KoShengInterventionMode] TCMContextResolver not found");
        GD.Print("[KoShengInterventionMode] Ready -- threshold=" + STREAK_THRESHOLD + " frames");
    }
    private void OnTCMContext(string channel, string element, string tissue,
        string neuroscience, string horary, string nutrition,
        string acupoints, string koSheng, string qigongForm)
    {
        if (element == _currentElement)
        {
            _streakCount++;
        }
        else
        {
            _currentElement = element;
            _currentChannel = channel;
            _streakCount = 1;
            _interventionActive = false;
        }
        if (_streakCount >= STREAK_THRESHOLD && !_interventionActive)
        {
            _interventionActive = true;
            string koEl = KoRestrains.ContainsKey(element) ? KoRestrains[element] : "";
            string moEl = ShengMother.ContainsKey(element) ? ShengMother[element] : "";
            string koCh = ElementChannel.ContainsKey(koEl) ? ElementChannel[koEl] : "";
            string moCh = ElementChannel.ContainsKey(moEl) ? ElementChannel[moEl] : "";
            string koRat = KoRationale.ContainsKey(element) ? KoRationale[element] : "";
            string moRat = MotherRationale.ContainsKey(element) ? MotherRationale[element] : "";
            GD.Print("[KoShengInterventionMode] " + element + " dominant x" + _streakCount
                + " -- Ko: " + koEl + " | Mother: " + moEl);
            EmitSignal(SignalName.InterventionRecommended,
                element, channel, koCh, moCh, koRat, moRat, _streakCount);
        }
    }
}

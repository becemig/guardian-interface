// ContentRegistry.cs — Guardian Interface · Trinity Protocol
// Static registry: loads all content JSON into C# objects at startup.
// Call ContentRegistry.Load() once. Then query freely from any Wing.

using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class ContentRegistry
{
    public static IReadOnlyList<AcupointEntry>         Acupoints      { get; private set; } = Array.Empty<AcupointEntry>();
    public static IReadOnlyList<HerbEntry>             Herbs          { get; private set; } = Array.Empty<HerbEntry>();
    public static IReadOnlyList<PatternEntry>          Patterns       { get; private set; } = Array.Empty<PatternEntry>();
    public static IReadOnlyList<SymptomEntry>          Symptoms       { get; private set; } = Array.Empty<SymptomEntry>();
    public static IReadOnlyList<FormulaEntry>          Formulas       { get; private set; } = Array.Empty<FormulaEntry>();
    public static IReadOnlyList<RedFlagEntry>          RedFlags       { get; private set; } = Array.Empty<RedFlagEntry>();
    public static IReadOnlyList<MovementModalityEntry> Movement       { get; private set; } = Array.Empty<MovementModalityEntry>();
    public static IReadOnlyList<FoeVignetteEntry>      FoeVignettes   { get; private set; } = Array.Empty<FoeVignetteEntry>();
    public static IReadOnlyList<TomeEntry>             Tomes          { get; private set; } = Array.Empty<TomeEntry>();
    public static IReadOnlyList<ForbiddenTomeEntry>    ForbiddenTomes { get; private set; } = Array.Empty<ForbiddenTomeEntry>();

    private static Dictionary<string, AcupointEntry>    _acupointById = new();
    private static Dictionary<string, HerbEntry>        _herbById     = new();
    private static Dictionary<string, PatternEntry>     _patternById  = new();
    private static Dictionary<string, SymptomEntry>     _symptomById  = new();
    private static Dictionary<string, RedFlagEntry>     _redFlagById  = new();
    private static Dictionary<string, FoeVignetteEntry> _foeById      = new();
    private static Dictionary<string, TomeEntry>        _tomeById     = new();

    public static bool IsLoaded { get; private set; } = false;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static void Load()
    {
        if (IsLoaded) return;
        Acupoints      = LoadList<AcupointEntry>("res://data/content/acupoints.json");
        Herbs          = LoadList<HerbEntry>("res://data/content/herbs.json");
        Patterns       = LoadList<PatternEntry>("res://data/content/patterns.json");
        Symptoms       = LoadList<SymptomEntry>("res://data/content/symptoms.json");
        Formulas       = LoadList<FormulaEntry>("res://data/content/formulas.json");
        RedFlags       = LoadList<RedFlagEntry>("res://data/content/red_flags.json");
        Movement       = LoadList<MovementModalityEntry>("res://data/content/movement.json");
        FoeVignettes   = LoadList<FoeVignetteEntry>("res://data/content/foe_vignettes.json");
        Tomes          = LoadList<TomeEntry>("res://data/content/tomes.json");
        ForbiddenTomes = LoadList<ForbiddenTomeEntry>("res://data/content/forbidden_tomes.json");
        foreach (var a in Acupoints)    _acupointById[a.Id] = a;
        foreach (var h in Herbs)        _herbById[h.Id]     = h;
        foreach (var p in Patterns)     _patternById[p.Id]  = p;
        foreach (var s in Symptoms)     _symptomById[s.Id]  = s;
        foreach (var r in RedFlags)     _redFlagById[r.Id]  = r;
        foreach (var f in FoeVignettes) _foeById[f.Id]      = f;
        foreach (var t in Tomes)        _tomeById[t.Id]     = t;
        IsLoaded = true;
        GD.Print($"[ContentRegistry] {Acupoints.Count} acupoints, {Herbs.Count} herbs, {Patterns.Count} patterns, {Symptoms.Count} symptoms, {FoeVignettes.Count} foes");
    }

    public static AcupointEntry    GetAcupoint(string id) => _acupointById.GetValueOrDefault(id);
    public static HerbEntry        GetHerb(string id)     => _herbById.GetValueOrDefault(id);
    public static PatternEntry     GetPattern(string id)  => _patternById.GetValueOrDefault(id);
    public static SymptomEntry     GetSymptom(string id)  => _symptomById.GetValueOrDefault(id);
    public static RedFlagEntry     GetRedFlag(string id)  => _redFlagById.GetValueOrDefault(id);
    public static FoeVignetteEntry GetFoe(string id)      => _foeById.GetValueOrDefault(id);
    public static TomeEntry        GetTome(string id)     => _tomeById.GetValueOrDefault(id);

    public static System.Collections.Generic.IEnumerable<AcupointEntry> SelfSafeAcupoints()
    {
        foreach (var a in Acupoints) if (a.SelfSafe) yield return a;
    }

    private static System.Collections.Generic.List<T> LoadList<T>(string resPath)
    {
        if (FileAccess.FileExists(resPath) == false)
        {
            GD.PrintErr($"[ContentRegistry] Missing: {resPath}");
            return new System.Collections.Generic.List<T>();
        }
        using var f = FileAccess.Open(resPath, FileAccess.ModeFlags.Read);
        var json = f.GetAsText();
        return JsonSerializer.Deserialize<System.Collections.Generic.List<T>>(json, _json) ?? new System.Collections.Generic.List<T>();
    }
}

// DATA MODELS
public class AcupointEntry
{
    [JsonPropertyName("id")]                  public string Id                   { get; set; } = "";
    [JsonPropertyName("pinyin")]              public string Pinyin               { get; set; } = "";
    [JsonPropertyName("characters")]          public string Characters           { get; set; } = "";
    [JsonPropertyName("english_translation")] public string EnglishTranslation   { get; set; } = "";
    [JsonPropertyName("channel")]             public string Channel              { get; set; } = "";
    [JsonPropertyName("point_number")]        public string PointNumber          { get; set; } = "";
    [JsonPropertyName("eastern_location")]    public string EasternLocation      { get; set; } = "";
    [JsonPropertyName("western_anatomy")]     public string WesternAnatomy       { get; set; } = "";
    [JsonPropertyName("modern_research_notes")] public string ModernResearchNotes { get; set; } = "";
    [JsonPropertyName("evidence_layer")]      public string EvidenceLayer        { get; set; } = "";
    [JsonPropertyName("method_note")]         public string MethodNote           { get; set; } = "";
    [JsonPropertyName("self_safe")]           public bool   SelfSafe             { get; set; }
    [JsonPropertyName("self_application_technique")] public string SelfApplicationTechnique { get; set; } = "";
    [JsonPropertyName("classical_functions")] public List<string> ClassicalFunctions  { get; set; } = new();
    [JsonPropertyName("contraindications")]   public List<string> Contraindications   { get; set; } = new();
    [JsonPropertyName("common_combinations")] public List<string> CommonCombinations  { get; set; } = new();
    [JsonPropertyName("indications_by_pattern")] public List<string> IndicationsByPattern { get; set; } = new();
}
public class FiveElementSignature
{
    [JsonPropertyName("element")] public string Element { get; set; } = "";
    [JsonPropertyName("action")]  public string Action  { get; set; } = "";
}
public class HerbEntry
{
    [JsonPropertyName("id")]            public string Id            { get; set; } = "";
    [JsonPropertyName("pinyin")]        public string Pinyin        { get; set; } = "";
    [JsonPropertyName("characters")]    public string Characters    { get; set; } = "";
    [JsonPropertyName("english_name")]  public string EnglishName   { get; set; } = "";
    [JsonPropertyName("latin_name")]    public string LatinName     { get; set; } = "";
    [JsonPropertyName("nature")]        public string Nature        { get; set; } = "";
    [JsonPropertyName("evidence_layer")] public string EvidenceLayer { get; set; } = "";
    [JsonPropertyName("self_safe")]     public bool SelfSafe        { get; set; }
    [JsonPropertyName("taste")]                public List<string> Taste              { get; set; } = new();
    [JsonPropertyName("channels_entered")]     public List<string> ChannelsEntered    { get; set; } = new();
    [JsonPropertyName("classical_functions")]  public List<string> ClassicalFunctions { get; set; } = new();
    [JsonPropertyName("contraindications")]    public List<string> Contraindications  { get; set; } = new();
    [JsonPropertyName("common_formulas")]      public List<string> CommonFormulas     { get; set; } = new();
    [JsonPropertyName("modern_research_notes")] public string ModernResearchNotes     { get; set; } = "";
}
public class PatternEntry
{
    [JsonPropertyName("id")]             public string Id            { get; set; } = "";
    [JsonPropertyName("name")]           public string Name          { get; set; } = "";
    [JsonPropertyName("category")]       public string Category      { get; set; } = "";
    [JsonPropertyName("organ_system")]   public string OrganSystem   { get; set; } = "";
    [JsonPropertyName("description")]    public string Description   { get; set; } = "";
    [JsonPropertyName("evidence_layer")] public string EvidenceLayer { get; set; } = "";
    [JsonPropertyName("tongue")]         public string Tongue        { get; set; } = "";
    [JsonPropertyName("pulse")]          public string Pulse         { get; set; } = "";
    [JsonPropertyName("wu_xing")]        public string WuXing        { get; set; } = "";
    [JsonPropertyName("key_symptoms")]   public List<string> KeySymptoms { get; set; } = new();
    [JsonPropertyName("acupoints")]      public List<string> Acupoints   { get; set; } = new();
    [JsonPropertyName("herbs")]          public List<string> Herbs       { get; set; } = new();
}
public class SymptomEntry
{
    [JsonPropertyName("id")]          public string Id          { get; set; } = "";
    [JsonPropertyName("label")]       public string Label       { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("is_red_flag")] public bool IsRedFlag     { get; set; }
    [JsonPropertyName("modifier_notes")] public string ModifierNotes { get; set; } = "";
    [JsonPropertyName("patterns_commonly_seen_in")] public List<string> PatternsCommonlySeenIn { get; set; } = new();
}
public class FormulaEntry
{
    [JsonPropertyName("id")]               public string Id              { get; set; } = "";
    [JsonPropertyName("pinyin")]           public string Pinyin          { get; set; } = "";
    [JsonPropertyName("english_name")]     public string EnglishName     { get; set; } = "";
    [JsonPropertyName("description")]      public string Description     { get; set; } = "";
    [JsonPropertyName("evidence_layer")]   public string EvidenceLayer   { get; set; } = "";
    [JsonPropertyName("chief_herbs")]      public List<string> ChiefHerbs        { get; set; } = new();
    [JsonPropertyName("patterns_addressed")] public List<string> PatternsAddressed { get; set; } = new();
}
public class RedFlagEntry
{
    [JsonPropertyName("id")]                 public string Id               { get; set; } = "";
    [JsonPropertyName("label")]              public string Label            { get; set; } = "";
    [JsonPropertyName("description")]        public string Description      { get; set; } = "";
    [JsonPropertyName("recommended_action")] public string RecommendedAction { get; set; } = "";
}
public class MovementModalityEntry
{
    [JsonPropertyName("id")]             public string Id            { get; set; } = "";
    [JsonPropertyName("name")]           public string Name          { get; set; } = "";
    [JsonPropertyName("description")]    public string Description   { get; set; } = "";
    [JsonPropertyName("evidence_layer")] public string EvidenceLayer { get; set; } = "";
    [JsonPropertyName("patterns")]       public List<string> Patterns { get; set; } = new();
}
public class FoeVignetteEntry
{
    [JsonPropertyName("id")]        public string Id        { get; set; } = "";
    [JsonPropertyName("name")]      public string Name      { get; set; } = "";
    [JsonPropertyName("blurb")]     public string Blurb     { get; set; } = "";
    [JsonPropertyName("correct")]   public string Correct   { get; set; } = "";
    [JsonPropertyName("rationale")] public string Rationale { get; set; } = "";
    [JsonPropertyName("element")]   public string Element   { get; set; } = "";
}
public class TomeEntry
{
    [JsonPropertyName("id")]      public string Id      { get; set; } = "";
    [JsonPropertyName("title")]   public string Title   { get; set; } = "";
    [JsonPropertyName("author")]  public string Author  { get; set; } = "";
    [JsonPropertyName("snippet")] public string Snippet { get; set; } = "";
    [JsonPropertyName("citation")] public string Citation { get; set; } = "";
}
public class ForbiddenTomeEntry
{
    [JsonPropertyName("id")]      public string Id      { get; set; } = "";
    [JsonPropertyName("title")]   public string Title   { get; set; } = "";
    [JsonPropertyName("framing")] public string Framing { get; set; } = "";
    [JsonPropertyName("note")]    public string Note    { get; set; } = "";
}

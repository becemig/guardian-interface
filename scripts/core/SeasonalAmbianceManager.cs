using Godot;
public partial class SeasonalAmbianceManager : Node {
    [Export] public NodePath WorldEnvPath="WorldEnvironment";
    [Export] public float TransitionSpeed=1.2f;
    private WorldEnvironment _env;
    private float _t=1f,_cohMod=0.6f;
    private struct Pal{public Color Amb,Sky,Fog,Ui;public float Energy,Str,FogD;}
    private static readonly Pal[] P=new Pal[]{
        new Pal{Amb=new Color(0.08f,0.10f,0.22f),Sky=new Color(0.05f,0.08f,0.20f),Fog=new Color(0.06f,0.09f,0.18f),Ui=new Color(0.17f,0.55f,0.88f),Energy=0.15f,Str=0.3f,FogD=0.04f},
        new Pal{Amb=new Color(0.18f,0.42f,0.18f),Sky=new Color(0.52f,0.78f,0.40f),Fog=new Color(0.30f,0.55f,0.28f),Ui=new Color(0.25f,0.82f,0.35f),Energy=0.55f,Str=0.65f,FogD=0.015f},
        new Pal{Amb=new Color(0.95f,0.72f,0.20f),Sky=new Color(0.98f,0.60f,0.15f),Fog=new Color(0.85f,0.55f,0.18f),Ui=new Color(0.98f,0.78f,0.10f),Energy=1.0f,Str=1.0f,FogD=0.005f},
        new Pal{Amb=new Color(0.72f,0.52f,0.18f),Sky=new Color(0.80f,0.62f,0.30f),Fog=new Color(0.65f,0.48f,0.22f),Ui=new Color(0.88f,0.65f,0.22f),Energy=0.5f,Str=0.75f,FogD=0.02f},
        new Pal{Amb=new Color(0.78f,0.80f,0.85f),Sky=new Color(0.65f,0.72f,0.80f),Fog=new Color(0.70f,0.75f,0.80f),Ui=new Color(0.75f,0.88f,0.95f),Energy=0.35f,Str=0.55f,FogD=0.025f}};
    private Pal _cur,_tgt;
    public override void _Ready(){
        _env=GetNodeOrNull<WorldEnvironment>(WorldEnvPath);
        _cur=P[(int)WuXingPhase.Earth];_tgt=_cur;
        UniversalDockingBus.Instance.Subscribe(BusEvent.PhaseChanged,_OnPhase);
        UniversalDockingBus.Instance.Subscribe(BusEvent.CoherenceChanged,_OnCoh);
        GD.Print("[SeasonalAmbianceManager] Ready");}
    public override void _ExitTree(){
        UniversalDockingBus.Instance.Unsubscribe(BusEvent.PhaseChanged,_OnPhase);
        UniversalDockingBus.Instance.Unsubscribe(BusEvent.CoherenceChanged,_OnCoh);}
    public override void _Process(double delta){
        if(_t>=1f)return;
        _t=Mathf.Min(_t+(float)delta/TransitionSpeed,1f);
        _Apply(_Lerp(_cur,_tgt,Mathf.SmoothStep(0f,1f,_t)),_cohMod);}
    public void _OnPhase(object d){
        if(d is not PhaseState s)return;
        _cur=_Lerp(_cur,_tgt,_t);_tgt=P[(int)s.Phase];_t=0f;
        UniversalDockingBus.Instance.Publish(BusEvent.UiAccentChanged,_tgt.Ui);}
    public void _OnCoh(object d){
        if(d is CoherencePayload c)_cohMod=0.5f+(c.Score*0.7f);}
    private void _Apply(Pal p,float cm){
        if(_env==null)return;
        var e=_env.Environment;if(e==null)return;
        e.AmbientLightColor=p.Amb;e.AmbientLightEnergy=p.Str*cm;
        e.FogEnabled=true;e.FogLightColor=p.Fog;e.FogDensity=p.FogD/cm;
        if(e.Sky?.SkyMaterial is ProceduralSkyMaterial sky){
            sky.SkyHorizonColor=p.Sky;sky.GroundHorizonColor=p.Fog;
            sky.SkyEnergyMultiplier=p.Energy*cm*1.5f;}}
    private static Pal _Lerp(Pal a,Pal b,float t)=>new Pal{
        Amb=a.Amb.Lerp(b.Amb,t),Sky=a.Sky.Lerp(b.Sky,t),
        Fog=a.Fog.Lerp(b.Fog,t),Ui=a.Ui.Lerp(b.Ui,t),
        Energy=Mathf.Lerp(a.Energy,b.Energy,t),
        Str=Mathf.Lerp(a.Str,b.Str,t),
        FogD=Mathf.Lerp(a.FogD,b.FogD,t)};
}

using Godot;
public enum WuXingPhase { Water, Wood, Fire, Earth, Metal }
public enum PhaseCoherence { Harmonic, Neutral, Discordant }
public class PhaseState {
    public WuXingPhase Phase { get; set; }
    public PhaseCoherence Coherence { get; set; }
    public float SensorHz { get; set; }
    public float CoherenceScore { get; set; }
    public string Season => Phase switch {
        WuXingPhase.Water=>"Winter",WuXingPhase.Wood=>"Spring",
        WuXingPhase.Fire=>"Summer",WuXingPhase.Earth=>"Late Summer",
        WuXingPhase.Metal=>"Autumn",_=>"Unknown"};
    public string Organ => Phase switch {
        WuXingPhase.Water=>"Kidney/Bladder",WuXingPhase.Wood=>"Liver/GB",
        WuXingPhase.Fire=>"Heart/SI",WuXingPhase.Earth=>"Spleen/ST",
        WuXingPhase.Metal=>"Lung/LI",_=>"Unknown"};
}
public class TelemetryPayload {
    public float SensorHz{get;set;} public float Coherence{get;set;}
    public string Modality{get;set;}="";
}
public class CoherencePayload {
    public float Score{get;set;} public PhaseCoherence State{get;set;}
    public CoherencePayload(float s,PhaseCoherence c){Score=s;State=c;}
}
public partial class PhaseGovernor : Node {
    public static PhaseGovernor Instance{get;private set;}
    public PhaseState CurrentPhase{get;private set;}=new PhaseState{
        Phase=WuXingPhase.Earth,Coherence=PhaseCoherence.Neutral,
        SensorHz=300f,CoherenceScore=0.6f};
    [Export] public float PhaseHoldSeconds=2.0f;
    private WuXingPhase _candidate=WuXingPhase.Earth;
    private float _holdTimer,_smoothHz=300f,_smoothCoh=0.6f;
    public override void _Ready(){
        Instance=this;
        UniversalDockingBus.Instance.Subscribe(BusEvent.TelemetryDataPushed,_OnTelemetry);
        UniversalDockingBus.Instance.Subscribe(BusEvent.SensorSignalReceived,_OnSensor);
        GD.Print("[PhaseGovernor] Ready");}
    public override void _ExitTree(){
        UniversalDockingBus.Instance.Unsubscribe(BusEvent.TelemetryDataPushed,_OnTelemetry);
        UniversalDockingBus.Instance.Unsubscribe(BusEvent.SensorSignalReceived,_OnSensor);}
    public void _OnTelemetry(object d){if(d is TelemetryPayload t)_Update(t.SensorHz,t.Coherence);}
    public void _OnSensor(object d){if(d is string volId)_Update(100f,_smoothCoh);}
    public override void _Process(double delta){
        if(_candidate!=CurrentPhase.Phase){
            _holdTimer+=(float)delta;
            if(_holdTimer>=PhaseHoldSeconds)_Commit(_candidate,_smoothCoh);}
        else _holdTimer=0f;}
    private void _Update(float hz,float coh){
        _smoothHz=Mathf.Lerp(_smoothHz,hz,0.08f);
        _smoothCoh=Mathf.Lerp(_smoothCoh,coh,0.15f);
        var p=_ToPhase(_smoothHz);
        if(p!=_candidate){_candidate=p;_holdTimer=0f;}
        var c=_ToCoh(_smoothCoh);
        if(c!=CurrentPhase.Coherence){
            CurrentPhase.Coherence=c;CurrentPhase.CoherenceScore=_smoothCoh;
            UniversalDockingBus.Instance.Publish(BusEvent.CoherenceChanged,new CoherencePayload(_smoothCoh,c));}}
    private void _Commit(WuXingPhase p,float coh){
        CurrentPhase=new PhaseState{Phase=p,Coherence=_ToCoh(coh),SensorHz=_smoothHz,CoherenceScore=coh};
        GD.Print($"[PhaseGovernor] {p} | {CurrentPhase.Season} | {CurrentPhase.Coherence}");
        UniversalDockingBus.Instance.Publish(BusEvent.PhaseChanged,CurrentPhase);}
    public void ForcePhase(WuXingPhase p,float coh=0.7f){_candidate=p;_holdTimer=PhaseHoldSeconds;_Commit(p,coh);}
    private static WuXingPhase _ToPhase(float hz)=>hz switch{
        <80f=>WuXingPhase.Water,<160f=>WuXingPhase.Wood,
        <280f=>WuXingPhase.Fire,<380f=>WuXingPhase.Earth,_=>WuXingPhase.Metal};
    private static PhaseCoherence _ToCoh(float s)=>s switch{
        >0.75f=>PhaseCoherence.Harmonic,>0.40f=>PhaseCoherence.Neutral,
        _=>PhaseCoherence.Discordant};
    public WuXingPhase Phase=>CurrentPhase.Phase;
    public float CoherenceScore=>CurrentPhase.CoherenceScore;
}

using Godot;
public partial class CalibrationHUD : Control
{
    public override void _Ready() {
        GetNode<Button>("BtnRest").Pressed += () => GetNode<SensorCalibrator>("/root/SensorCalibrator").CurrentState = SensorCalibrator.CalibrationState.MeasuringRest;
        GetNode<Button>("BtnMax").Pressed += () => GetNode<SensorCalibrator>("/root/SensorCalibrator").CurrentState = SensorCalibrator.CalibrationState.MeasuringMax;
    }
}

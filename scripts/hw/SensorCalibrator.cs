using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SensorCalibrator : Node
{
    public enum CalibrationState { Uncalibrated, MeasuringRest, MeasuringMax, Complete }
    public CalibrationState CurrentState = CalibrationState.Uncalibrated;
    private float _rawRestValue = 0.0f;
    private float _rawMaxValue = 4095.0f;
    private List<float> _buffer = new List<float>();

    public float ProcessSignal(float raw)
    {
        if (CurrentState == CalibrationState.MeasuringRest) {
            _buffer.Add(raw);
            if (_buffer.Count > 50) { _rawRestValue = _buffer.Average(); CurrentState = CalibrationState.Uncalibrated; }
        }
        else if (CurrentState == CalibrationState.MeasuringMax) {
            _buffer.Add(raw);
            if (_buffer.Count > 50) { _rawMaxValue = _buffer.Average(); CurrentState = CalibrationState.Complete; }
        }
        return CurrentState == CalibrationState.Complete ? Mathf.Clamp((raw - _rawRestValue) / (_rawMaxValue - _rawRestValue), 0, 1) : raw / 4095.0f;
    }
}

using Godot;
using System;

namespace NeuroModelingDemo;

[Tool]
public partial class ContinuousTimeRnnNode : Node
{
    [Export] public int InputSize { get; set; } = 2;
    [Export] public int HiddenSize { get; set; } = 2;
    [Export] public int OutputSize { get; set; } = 1;
    [Export] public double Tau { get; set; } = 0.5;
    [Export] public string IntegrationMethod { get; set; } = "rk4";

    private ContinuousTimeRnnSystem? _system;

    public override void _Ready()
    {
        var config = new ContinuousTimeRnnConfig(
            InputSize,
            HiddenSize,
            OutputSize,
            Tau,
            IntegrationMethod,
            seed: 0
        );

        if (InputSize != 2 || HiddenSize != 2 || OutputSize != 1)
        {
            GD.PushWarning(
                "[ContinuousTimeRnnNode] This prototype currently " +
                "uses the fixed 2-2-1 synthetic parity fixture."
            );
            return;
        }

        _system = new ContinuousTimeRnnSystem(
            config,
            wIn: new double[,]
            {
                { 0.40, -0.20 },
                { 0.10, 0.30 }
            },
            wRec: new double[,]
            {
                { 0.15, -0.05 },
                { 0.08, 0.12 }
            },
            bHidden: new double[] { 0.0, 0.0 },
            wOut: new double[,]
            {
                { 0.25, -0.35 }
            },
            bOut: new double[] { 0.0 },
            initialState: new double[] { 0.0, 0.0 }
        );

        RnnParityHarness.Verify();
    }

    public double[] Step(double[] input, double dt)
    {
        if (_system is null)
        {
            throw new InvalidOperationException(
                "RNN system is not initialized."
            );
        }

        return _system.Step(input, dt);
    }

    public void ResetState()
    {
        _system?.ResetState();
    }

    public double[] GetState()
    {
        return _system?.State ?? Array.Empty<double>();
    }

    public double GetMeanState()
    {
        double[] state = _system?.State ?? Array.Empty<double>();

        if (state.Length == 0)
            return 0.0;

        double total = 0.0;

        foreach (double value in state)
            total += value;

        return total / state.Length;
    }

    public double GetStateNorm()
    {
        return _system?.StateNorm() ?? 0.0;
    }
}

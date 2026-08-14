using Godot;
using System;

namespace NeuroModelingDemo;

[Tool]
public partial class ContinuousTimeRnnNode : Node
{
    private ContinuousTimeRnnSystem? _system;

    public override void _Ready()
    {
        (RnnParityFixture fixture, _) = RnnParityFixtureLoader.Load();

        var config = new ContinuousTimeRnnConfig(
            fixture.InputSize,
            fixture.HiddenSize,
            fixture.OutputSize,
            fixture.Tau,
            fixture.IntegrationMethod,
            seed: 0
        );

        _system = new ContinuousTimeRnnSystem(
            config,
            RnnParityFixtureLoader.ToMatrix(fixture.WIn, "w_in"),
            RnnParityFixtureLoader.ToMatrix(fixture.WRec, "w_rec"),
            fixture.BHidden,
            RnnParityFixtureLoader.ToMatrix(fixture.WOut, "w_out"),
            fixture.BOut,
            fixture.InitialState
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
        {
            return 0.0;
        }

        double total = 0.0;

        foreach (double value in state)
        {
            total += value;
        }

        return total / state.Length;
    }

    public double GetStateNorm()
    {
        return _system?.StateNorm() ?? 0.0;
    }
}

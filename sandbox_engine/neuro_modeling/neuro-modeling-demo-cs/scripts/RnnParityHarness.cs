using Godot;
using System;

namespace NeuroModelingDemo;

public static class RnnParityHarness
{
    public static void Verify()
    {
        (RnnParityFixture fixture, RnnParityExpected expected) =
            RnnParityFixtureLoader.Load();

        var config = new ContinuousTimeRnnConfig(
            fixture.InputSize,
            fixture.HiddenSize,
            fixture.OutputSize,
            fixture.Tau,
            fixture.IntegrationMethod,
            seed: 0
        );

        var system = new ContinuousTimeRnnSystem(
            config,
            RnnParityFixtureLoader.ToMatrix(fixture.WIn, "w_in"),
            RnnParityFixtureLoader.ToMatrix(fixture.WRec, "w_rec"),
            fixture.BHidden,
            RnnParityFixtureLoader.ToMatrix(fixture.WOut, "w_out"),
            fixture.BOut,
            fixture.InitialState
        );

        for (int index = 0; index < fixture.Steps.Length; index++)
        {
            RnnParityStep step = fixture.Steps[index];
            RnnParityExpectedStep expectedStep = expected.Steps[index];

            VerifyStep(
                system,
                step.Input,
                step.Dt,
                expectedStep.State,
                expectedStep.Output,
                index + 1,
                expected.Tolerance
            );
        }

        GD.Print(
            "[RnnParityHarness] PASS: C# RK4 fixture matches " +
            "Python reference outputs."
        );
    }

    private static void VerifyStep(
        ContinuousTimeRnnSystem system,
        double[] input,
        double dt,
        double[] expectedState,
        double[] expectedOutput,
        int stepNumber,
        double tolerance)
    {
        double[] actualOutput = system.Step(input, dt);

        AssertClose(
            system.State,
            expectedState,
            $"state at step {stepNumber}",
            tolerance
        );

        AssertClose(
            actualOutput,
            expectedOutput,
            $"output at step {stepNumber}",
            tolerance
        );
    }

    private static void AssertClose(
        double[] actual,
        double[] expected,
        string label,
        double tolerance)
    {
        if (actual.Length != expected.Length)
        {
            throw new InvalidOperationException(
                $"Parity mismatch for {label}: length differs."
            );
        }

        for (int index = 0; index < actual.Length; index++)
        {
            if (Math.Abs(actual[index] - expected[index]) > tolerance)
            {
                throw new InvalidOperationException(
                    $"Parity mismatch for {label}[{index}]: " +
                    $"expected {expected[index]:R}, " +
                    $"received {actual[index]:R}."
                );
            }
        }
    }
}

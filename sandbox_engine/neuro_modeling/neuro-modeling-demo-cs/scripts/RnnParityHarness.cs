using Godot;
using System;

namespace NeuroModelingDemo;

public static class RnnParityHarness
{
    private const double Tolerance = 1e-12;

    public static void Verify()
    {
        var config = new ContinuousTimeRnnConfig(
            inputSize: 2,
            hiddenSize: 2,
            outputSize: 1,
            tau: 0.5,
            integrationMethod: "rk4",
            seed: 0
        );

        var system = new ContinuousTimeRnnSystem(
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

        VerifyStep(
            system,
            input: new double[] { 1.0, -0.5 },
            dt: 0.02,
            expectedState: new double[]
            {
                0.019665750447084254,
                -0.001933937905994222
            },
            expectedOutput: new double[]
            {
                0.005592681335820335
            },
            stepNumber: 1
        );

        VerifyStep(
            system,
            input: new double[] { 0.25, 0.75 },
            dt: 0.01,
            expectedState: new double[]
            {
                0.018342117801678135,
                0.003086189878743904
            },
            expectedOutput: new double[]
            {
                0.0035048522498333087
            },
            stepNumber: 2
        );

        VerifyStep(
            system,
            input: new double[] { 0.0, 0.0 },
            dt: 0.03,
            expectedState: new double[]
            {
                0.017421190994724663,
                0.003011025251471711
            },
            expectedOutput: new double[]
            {
                0.003301001541094121
            },
            stepNumber: 3
        );

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
        int stepNumber)
    {
        double[] actualOutput = system.Step(input, dt);

        AssertClose(
            system.State,
            expectedState,
            $"state at step {stepNumber}"
        );

        AssertClose(
            actualOutput,
            expectedOutput,
            $"output at step {stepNumber}"
        );
    }

    private static void AssertClose(
        double[] actual,
        double[] expected,
        string label)
    {
        if (actual.Length != expected.Length)
        {
            throw new InvalidOperationException(
                $"Parity mismatch for {label}: length differs."
            );
        }

        for (int index = 0; index < actual.Length; index++)
        {
            if (Math.Abs(actual[index] - expected[index]) > Tolerance)
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

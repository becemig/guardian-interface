using System;

namespace NeuroModelingDemo;

public sealed class ContinuousTimeRnnConfig
{
    public int InputSize { get; }
    public int HiddenSize { get; }
    public int OutputSize { get; }
    public double Tau { get; }
    public string IntegrationMethod { get; }
    public int Seed { get; }

    public ContinuousTimeRnnConfig(
        int inputSize,
        int hiddenSize,
        int outputSize,
        double tau = 1.0,
        string integrationMethod = "euler",
        int seed = 42)
    {
        if (inputSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(inputSize),
                "Input size must be greater than zero."
            );

        if (hiddenSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(hiddenSize),
                "Hidden size must be greater than zero."
            );

        if (outputSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(outputSize),
                "Output size must be greater than zero."
            );

        if (!double.IsFinite(tau) || tau <= 0.0)
            throw new ArgumentOutOfRangeException(
                nameof(tau),
                "Tau must be finite and greater than zero."
            );

        if (integrationMethod is not ("euler" or "rk4"))
            throw new ArgumentException(
                "Integration method must be 'euler' or 'rk4'.",
                nameof(integrationMethod)
            );

        InputSize = inputSize;
        HiddenSize = hiddenSize;
        OutputSize = outputSize;
        Tau = tau;
        IntegrationMethod = integrationMethod;
        Seed = seed;
    }
}

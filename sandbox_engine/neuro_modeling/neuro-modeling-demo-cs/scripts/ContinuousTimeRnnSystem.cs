using System;

namespace NeuroModelingDemo;

public sealed class ContinuousTimeRnnSystem
{
    private readonly ContinuousTimeRnnConfig _config;
    private readonly double[,] _wIn;
    private readonly double[,] _wRec;
    private readonly double[] _bHidden;
    private readonly double[,] _wOut;
    private readonly double[] _bOut;
    private double[] _state = Array.Empty<double>();

    public int InputSize => _config.InputSize;
    public int HiddenSize => _config.HiddenSize;
    public int OutputSize => _config.OutputSize;

    public double[] State => (double[])_state.Clone();

    public ContinuousTimeRnnSystem(
        ContinuousTimeRnnConfig config,
        double[,] wIn,
        double[,] wRec,
        double[] bHidden,
        double[,] wOut,
        double[] bOut,
        double[]? initialState = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));

        ValidateMatrix(
            wIn,
            config.HiddenSize,
            config.InputSize,
            nameof(wIn)
        );
        ValidateMatrix(
            wRec,
            config.HiddenSize,
            config.HiddenSize,
            nameof(wRec)
        );
        ValidateVector(
            bHidden,
            config.HiddenSize,
            nameof(bHidden)
        );
        ValidateMatrix(
            wOut,
            config.OutputSize,
            config.HiddenSize,
            nameof(wOut)
        );
        ValidateVector(
            bOut,
            config.OutputSize,
            nameof(bOut)
        );

        _wIn = (double[,])wIn.Clone();
        _wRec = (double[,])wRec.Clone();
        _bHidden = (double[])bHidden.Clone();
        _wOut = (double[,])wOut.Clone();
        _bOut = (double[])bOut.Clone();

        ResetState(initialState);
    }

    public void ResetState(double[]? state = null)
    {
        if (state is null)
        {
            _state = new double[HiddenSize];
            return;
        }

        ValidateVector(state, HiddenSize, nameof(state));
        _state = (double[])state.Clone();
    }

    public double[] Step(double[] input, double dt)
    {
        if (!double.IsFinite(dt) || dt <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dt),
                "Timestep must be finite and greater than zero."
            );
        }

        ValidateVector(input, InputSize, nameof(input));

        double[] nextState = _config.IntegrationMethod == "rk4"
            ? IntegrateRk4(_state, input, dt)
            : IntegrateEuler(_state, input, dt);

        EnsureFinite(nextState, "RNN state became non-finite.");
        _state = nextState;

        return Output();
    }

    public double[] Output()
    {
        double[] activated = Activate(_state);
        double[] output = new double[OutputSize];

        for (int row = 0; row < OutputSize; row++)
        {
            double total = _bOut[row];

            for (int column = 0; column < HiddenSize; column++)
                total += _wOut[row, column] * activated[column];

            output[row] = total;
        }

        EnsureFinite(output, "RNN output became non-finite.");
        return output;
    }

    public double StateNorm()
    {
        double sumSquares = 0.0;

        foreach (double value in _state)
            sumSquares += value * value;

        return Math.Sqrt(sumSquares);
    }

    private double[] Derivative(double[] state, double[] input)
    {
        double[] activated = Activate(state);
        double[] derivative = new double[HiddenSize];

        for (int row = 0; row < HiddenSize; row++)
        {
            double total = -state[row] + _bHidden[row];

            for (int column = 0; column < HiddenSize; column++)
                total += _wRec[row, column] * activated[column];

            for (int column = 0; column < InputSize; column++)
                total += _wIn[row, column] * input[column];

            derivative[row] = total / _config.Tau;
        }

        return derivative;
    }

    private double[] IntegrateEuler(
        double[] state,
        double[] input,
        double dt)
    {
        double[] derivative = Derivative(state, input);
        double[] result = new double[HiddenSize];

        for (int index = 0; index < HiddenSize; index++)
            result[index] = state[index] + dt * derivative[index];

        return result;
    }

    private double[] IntegrateRk4(
        double[] state,
        double[] input,
        double dt)
    {
        double[] k1 = Derivative(state, input);
        double[] k2 = Derivative(
            AddScaled(state, k1, 0.5 * dt),
            input
        );
        double[] k3 = Derivative(
            AddScaled(state, k2, 0.5 * dt),
            input
        );
        double[] k4 = Derivative(
            AddScaled(state, k3, dt),
            input
        );

        double[] result = new double[HiddenSize];

        for (int index = 0; index < HiddenSize; index++)
        {
            result[index] = state[index] + (dt / 6.0) *
                (k1[index] + 2.0 * k2[index] +
                 2.0 * k3[index] + k4[index]);
        }

        return result;
    }

    private static double[] AddScaled(
        double[] source,
        double[] delta,
        double scale)
    {
        double[] result = new double[source.Length];

        for (int index = 0; index < source.Length; index++)
            result[index] = source[index] + scale * delta[index];

        return result;
    }

    private static double[] Activate(double[] values)
    {
        double[] result = new double[values.Length];

        for (int index = 0; index < values.Length; index++)
            result[index] = Math.Tanh(values[index]);

        return result;
    }

    private static void ValidateMatrix(
        double[,] matrix,
        int rows,
        int columns,
        string name)
    {
        if (matrix is null ||
            matrix.GetLength(0) != rows ||
            matrix.GetLength(1) != columns)
        {
            throw new ArgumentException(
                $"Expected {name} shape [{rows}, {columns}].",
                name
            );
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (!double.IsFinite(matrix[row, column]))
                {
                    throw new ArgumentException(
                        $"{name} contains a non-finite value.",
                        name
                    );
                }
            }
        }
    }

    private static void ValidateVector(
        double[] values,
        int expectedLength,
        string name)
    {
        if (values is null || values.Length != expectedLength)
        {
            throw new ArgumentException(
                $"Expected {name} length {expectedLength}.",
                name
            );
        }

        EnsureFinite(values, $"{name} contains a non-finite value.");
    }

    private static void EnsureFinite(
        double[] values,
        string message)
    {
        foreach (double value in values)
        {
            if (!double.IsFinite(value))
                throw new InvalidOperationException(message);
        }
    }
}

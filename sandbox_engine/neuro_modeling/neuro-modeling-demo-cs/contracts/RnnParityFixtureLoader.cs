using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeuroModelingDemo;

public sealed class RnnParityFixture
{
    [JsonPropertyName("input_size")]
    public int InputSize { get; init; }

    [JsonPropertyName("hidden_size")]
    public int HiddenSize { get; init; }

    [JsonPropertyName("output_size")]
    public int OutputSize { get; init; }

    [JsonPropertyName("tau")]
    public double Tau { get; init; }

    [JsonPropertyName("integration_method")]
    public string IntegrationMethod { get; init; } = "rk4";

    [JsonPropertyName("initial_state")]
    public double[] InitialState { get; init; } = Array.Empty<double>();

    [JsonPropertyName("w_in")]
    public double[][] WIn { get; init; } = Array.Empty<double[]>();

    [JsonPropertyName("w_rec")]
    public double[][] WRec { get; init; } = Array.Empty<double[]>();

    [JsonPropertyName("b_hidden")]
    public double[] BHidden { get; init; } = Array.Empty<double>();

    [JsonPropertyName("w_out")]
    public double[][] WOut { get; init; } = Array.Empty<double[]>();

    [JsonPropertyName("b_out")]
    public double[] BOut { get; init; } = Array.Empty<double>();

    [JsonPropertyName("steps")]
    public RnnParityStep[] Steps { get; init; } = Array.Empty<RnnParityStep>();
}

public sealed class RnnParityStep
{
    [JsonPropertyName("input")]
    public double[] Input { get; init; } = Array.Empty<double>();

    [JsonPropertyName("dt")]
    public double Dt { get; init; }
}

public sealed class RnnParityExpected
{
    [JsonPropertyName("tolerance")]
    public double Tolerance { get; init; }

    [JsonPropertyName("expected")]
    public RnnParityExpectedStep[] Steps { get; init; } =
        Array.Empty<RnnParityExpectedStep>();
}

public sealed class RnnParityExpectedStep
{
    [JsonPropertyName("state")]
    public double[] State { get; init; } = Array.Empty<double>();

    [JsonPropertyName("output")]
    public double[] Output { get; init; } = Array.Empty<double>();
}

public static class RnnParityFixtureLoader
{
    private const string FixturePath =
        "res://contracts/rnn_parity_fixture.json";

    private const string ExpectedPath =
        "res://contracts/rnn_parity_expected.json";

    public static (RnnParityFixture Fixture, RnnParityExpected Expected) Load()
    {
        RnnParityFixture fixture = Deserialize<RnnParityFixture>(FixturePath);
        RnnParityExpected expected = Deserialize<RnnParityExpected>(ExpectedPath);

        Validate(fixture, expected);

        return (fixture, expected);
    }

    public static double[,] ToMatrix(double[][] rows, string label)
    {
        if (rows.Length == 0 || rows[0].Length == 0)
        {
            throw new InvalidOperationException(
                $"RNN parity fixture matrix '{label}' must not be empty."
            );
        }

        int columnCount = rows[0].Length;
        var matrix = new double[rows.Length, columnCount];

        for (int row = 0; row < rows.Length; row++)
        {
            if (rows[row].Length != columnCount)
            {
                throw new InvalidOperationException(
                    $"RNN parity fixture matrix '{label}' is not rectangular."
                );
            }

            for (int column = 0; column < columnCount; column++)
            {
                matrix[row, column] = rows[row][column];
            }
        }

        return matrix;
    }

    private static T Deserialize<T>(string path)
    {
        string json = FileAccess.GetFileAsString(path);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException(
                $"Unable to read required RNN parity contract: {path}."
            );
        }

        T? result = JsonSerializer.Deserialize<T>(json);

        return result ?? throw new InvalidOperationException(
            $"Unable to deserialize RNN parity contract: {path}."
        );
    }

    private static void Validate(
        RnnParityFixture fixture,
        RnnParityExpected expected)
    {
        if (fixture.InputSize <= 0 ||
            fixture.HiddenSize <= 0 ||
            fixture.OutputSize <= 0)
        {
            throw new InvalidOperationException(
                "RNN parity fixture dimensions must be positive."
            );
        }

        if (fixture.Tau <= 0.0)
        {
            throw new InvalidOperationException(
                "RNN parity fixture tau must be positive."
            );
        }

        if (!string.Equals(
                fixture.IntegrationMethod,
                "rk4",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "RNN parity fixture must use RK4 integration."
            );
        }

        if (fixture.Steps.Length != expected.Steps.Length)
        {
            throw new InvalidOperationException(
                "RNN parity fixture and expected step counts differ."
            );
        }

        if (fixture.InitialState.Length != fixture.HiddenSize ||
            fixture.BHidden.Length != fixture.HiddenSize ||
            fixture.BOut.Length != fixture.OutputSize)
        {
            throw new InvalidOperationException(
                "RNN parity fixture vector dimensions are invalid."
            );
        }

        ValidateMatrix(fixture.WIn, fixture.HiddenSize, fixture.InputSize, "w_in");
        ValidateMatrix(fixture.WRec, fixture.HiddenSize, fixture.HiddenSize, "w_rec");
        ValidateMatrix(fixture.WOut, fixture.OutputSize, fixture.HiddenSize, "w_out");
    }

    private static void ValidateMatrix(
        double[][] matrix,
        int rowCount,
        int columnCount,
        string label)
    {
        if (matrix.Length != rowCount)
        {
            throw new InvalidOperationException(
                $"RNN parity fixture '{label}' row count is invalid."
            );
        }

        foreach (double[] row in matrix)
        {
            if (row.Length != columnCount)
            {
                throw new InvalidOperationException(
                    $"RNN parity fixture '{label}' column count is invalid."
                );
            }
        }
    }
}

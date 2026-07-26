using Godot;
using System;
using System.Collections.Generic;

public class SinewTrain
{
    public string LineName;          // e.g., "Bladder Sinew / Superficial Back Line"
    public List<string> BoundNodes;  // Group of myofascial/acupoint markers in this line
    public float ContinuousTension;  // Aggregated tension metric across the train
}

public partial class SinewChannelMapper : Node
{
    private Dictionary<string, SinewTrain> _myofascialWeb = new Dictionary<string, SinewTrain>();

    public override void _Ready()
    {
        InitializeYiJinJingLines();
    }

    private void InitializeYiJinJingLines()
    {
        // Map the Taiyang (Bladder) Sinew Line - Foot to Head tension line
        _myofascialWeb["Taiyang_BL"] = new SinewTrain
        {
            LineName = "Superficial Back Line",
            BoundNodes = new List<string> { "BL-67", "BL-57", "BL-40", "BL-10" },
            ContinuousTension = 0.0f
        };

        // Map the Jueyin (Liver) Sinew Line - Inward spiral / stabilization line
        _myofascialWeb["Jueyin_LV"] = new SinewTrain
        {
            LineName = "Deep Front Line / Spiral Stabilizer",
            BoundNodes = new List<string> { "LV-1", "LV-3", "LV-8", "LV-14" },
            ContinuousTension = 0.0f
        };

        GD.Print("Yi Jin Jing Myofascial Web initialized into 12 primary Sinew Trains.");
    }

    // Input A Update: Compute the tensional continuity
    public float[] ExtractTensionTensor(Dictionary<string, float> sensorTelemetry)
    {
        List<float> tensorInputs = new List<float>();

        foreach (var train in _myofascialWeb.Values)
        {
            float totalLineTension = 0.0f;
            foreach (var node in train.BoundNodes)
            {
                if (sensorTelemetry.TryGetValue(node, out float tension))
                {
                    totalLineTension += tension;
                }
            }
            // Calculate the average tensional continuity of the train
            train.ContinuousTension = totalLineTension / train.BoundNodes.Count;
            tensorInputs.Add(train.ContinuousTension);
        }

        // Returns a clean array matching the 12 structural lines for Deep Learning inference
        return tensorInputs.ToArray();
    }
}

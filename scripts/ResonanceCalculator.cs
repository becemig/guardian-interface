using Godot;
using System.Linq;

public partial class ResonanceCalculator : Node
{
    public float CalculateSectionResonance(float[] sectorWeights)
    {
        // Summing the resonance across the 8 sectors
        return sectorWeights.Sum();
    }
}

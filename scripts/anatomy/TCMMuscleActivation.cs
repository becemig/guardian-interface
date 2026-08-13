using Godot;

public partial class TCMMuscleActivation : MeshInstance3D
{
    [Export] public string MuscleId { get; set; } = "";
    [Export] public string Element { get; set; } = "Wood";
    [Export(PropertyHint.Range, "0,1,0.01")]
    public float Activation { get; private set; }

    private ShaderMaterial _material;

    public override void _Ready()
    {
        _material = GetActiveMaterial(0) as ShaderMaterial;

        if (_material == null)
        {
            GD.PushWarning(
                $"[TCMMuscleActivation] {Name} needs a ShaderMaterial on surface 0."
            );
            return;
        }

        ApplyVisuals();
    }

    public void SetSyntheticActivation(float value, double elapsedSeconds)
    {
        Activation = Mathf.Clamp(value, 0.0f, 1.0f);

        if (_material == null)
            return;

        _material.SetShaderParameter("activation", Activation);
        _material.SetShaderParameter(
            "pulse_phase",
            (float)(elapsedSeconds % Mathf.Tau + Mathf.Tau) % Mathf.Tau
        );
    }

    private void ApplyVisuals()
    {
        if (_material == null)
            return;

        _material.SetShaderParameter("element_color", ElementColor(Element));
        _material.SetShaderParameter("activation", Activation);
    }

    private static Color ElementColor(string element)
    {
        return element.ToLowerInvariant() switch
        {
            "fire" => new Color("#E14B4B"),
            "earth" => new Color("#D9A625"),
            "metal" => new Color("#DCE6F0"),
            "water" => new Color("#1F4E8C"),
            _ => new Color("#24B86A")
        };
    }
}

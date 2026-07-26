using Godot;

public partial class NodePopulator : Node
{
    public void Populate(Godot.Collections.Dictionary nodeData)
    {
        // Use ContainsKey instead of Contains
        if (nodeData.ContainsKey("label"))
        {
            var label = GetNodeOrNull<Label3D>("Label3D");
            if (label != null) label.Text = nodeData["label"].ToString();
        }

        if (nodeData.ContainsKey("element"))
        {
            var mesh = GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
            if (mesh != null)
            {
                var material = new StandardMaterial3D();
                string element = nodeData["element"].ToString();
                material.AlbedoColor = element switch {
                    "Earth" => Colors.Orange, // Amber is not a standard Godot color
                    "Water" => Colors.Blue,
                    "Fire" => Colors.Red,
                    _ => Colors.Gray
                };
                mesh.SetSurfaceOverrideMaterial(0, material);
            }
        }
    }
}

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WorldCompiler : Node
{
    private const float AttractorThreshold = 15.0f;
    private const float FoldingRadius = 2.0f;
    private const float GravityPull = 0.3f;

    public Dictionary<string, Vector3> NodePositions = new Dictionary<string, Vector3>();

    public override void _Ready()
    {
        var academy = GetNode<AcademyManager>("/root/AcademyManager");
        CompileLayout(academy.Nodes);
    }

    private void CompileLayout(IEnumerable<Node> nodes)
    {
        var nodeData = new List<NodeProxy>();
        foreach (var node in nodes)
        {
            var n = (Godot.Collections.Dictionary)node.Get("Data");
            
            float domainCount = n.ContainsKey("domain_count") ? (float)n["domain_count"] : 1f;
            float avgWeight = n.ContainsKey("avg_weight") ? (float)n["avg_weight"] : 0.5f;
            float enthalpy = 0f;
            
            if (n.ContainsKey("energy_state"))
            {
                var es = (Godot.Collections.Dictionary)n["energy_state"];
                enthalpy = es.ContainsKey("enthalpy") ? (float)es["enthalpy"] : 0f;
            }
            
            float mass = domainCount * avgWeight * enthalpy * -1f;
            nodeData.Add(new NodeProxy { Id = node.Name, Mass = mass, Data = n });
        }

        var attractors = nodeData.Where(n => n.Mass > AttractorThreshold).ToList();
        var rng = new Random();

        foreach (var a in attractors)
        {
            a.Position = new Vector3((float)(rng.NextDouble() * 20 - 10), (float)(rng.NextDouble() * 20 - 10), (float)(rng.NextDouble() * 20 - 10));
        }

        foreach (var n in nodeData)
        {
            string fold = GetFoldState(n.Data);
            if (fold == "native" || fold == "intermediate")
            {
                string similarId = GetFirstSimilar(n.Data);
                if (string.IsNullOrEmpty(similarId) || !NodePositions.ContainsKey(similarId))
                    n.Position = new Vector3((float)(rng.NextDouble() * 40 - 20), (float)(rng.NextDouble() * 40 - 20), (float)(rng.NextDouble() * 40 - 20));
                else
                    n.Position = NodePositions[similarId] + new Vector3((float)(rng.NextDouble() * FoldingRadius * 2 - FoldingRadius), (float)(rng.NextDouble() * FoldingRadius * 2 - FoldingRadius), (float)(rng.NextDouble() * FoldingRadius * 2 - FoldingRadius));
                
                NodePositions[n.Id] = n.Position;
            }
        }
    }

    private string GetFoldState(Godot.Collections.Dictionary n)
    {
        if (!n.ContainsKey("energy_state")) return "native";
        var es = (Godot.Collections.Dictionary)n["energy_state"];
        return es.ContainsKey("fold_state") ? es["fold_state"].ToString() : "native";
    }

    private string GetFirstSimilar(Godot.Collections.Dictionary n)
    {
        if (!n.ContainsKey("branching_vectors")) return string.Empty;
        var bv = (Godot.Collections.Dictionary)n["branching_vectors"];
        if (!bv.ContainsKey("similar")) return string.Empty;
        var arr = (Godot.Collections.Array)bv["similar"];
        return arr.Count > 0 ? arr[0].ToString() : string.Empty;
    }

    private class NodeProxy
    {
        public string Id = string.Empty;
        public float Mass;
        public Vector3 Position;
        public Godot.Collections.Dictionary Data = new Godot.Collections.Dictionary();
    }
}

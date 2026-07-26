using Godot;
using System.Collections.Generic;

public interface IKnowledgeNode
{
    string NodeId { get; }
    void Execute(float resonanceLevel);
    Dictionary<string, float> GetBranchingVectors();
}

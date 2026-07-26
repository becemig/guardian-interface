using Godot;
using System;

public partial class GuardianCore : Node
{
    public int GetSectionIndex(int degree) => (degree - 1) / 45;

    public override void _Ready()
    {
        GD.Print("GuardianCore: Octal-Vector Mapping Online.");
        GD.Print("System initialized for 8-Section Alchemical Taxonomy.");
    }

    public void ProcessDegree(int degree)
    {
        int section = GetSectionIndex(degree);
        GD.Print($"Mapping Degree {degree} to Section {section}: Alchemical Domain Active.");
    }
}

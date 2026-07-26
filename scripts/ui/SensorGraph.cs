using Godot;
using System.Collections.Generic;

public partial class SensorGraph : Line2D
{
    private Queue<float> _dataPoints = new Queue<float>();
    [Export] public int MaxPoints = 200;

    public void AddDataPoint(float value)
    {
        _dataPoints.Enqueue(value);
        if (_dataPoints.Count > MaxPoints) _dataPoints.Dequeue();

        // Map data to screen coordinates
        Points = new Vector2[_dataPoints.Count];
        for (int i = 0; i < _dataPoints.Count; i++)
        {
            float x = i * (GetViewportRect().Size.X / MaxPoints);
            float y = GetViewportRect().Size.Y - (_dataPoints.ToArray()[i] * GetViewportRect().Size.Y);
            Points[i] = new Vector2(x, y);
        }
    }
}

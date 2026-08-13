using Godot;
using System;
using System.Collections.Generic;

public partial class LoadDynamicsPlot : Control
{
    private const double WindowSeconds = 12.0;

    private readonly List<LoadSample> _samples = new();
    private readonly List<CueMarker> _markers = new();

    private sealed class LoadSample
    {
        public double Time;
        public float Left;
        public float Right;
    }

    private sealed class CueMarker
    {
        public double Time;
        public string Cue = "";
    }

    public LoadDynamicsPlot()
    {
        CustomMinimumSize = new Vector2(0, 84);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public void Reset()
    {
        _samples.Clear();
        _markers.Clear();
        QueueRedraw();
    }

    public void AddSample(double elapsedSeconds, float leftLoad, float rightLoad)
    {
        _samples.Add(new LoadSample
        {
            Time = elapsedSeconds,
            Left = leftLoad,
            Right = rightLoad
        });

        TrimOldItems(elapsedSeconds);
        QueueRedraw();
    }

    public void AddCueMarker(double elapsedSeconds, string cue)
    {
        _markers.Add(new CueMarker
        {
            Time = elapsedSeconds,
            Cue = cue
        });

        TrimOldItems(elapsedSeconds);
        QueueRedraw();
    }

    private void TrimOldItems(double latestTime)
    {
        double cutoff = latestTime - WindowSeconds;

        while (_samples.Count > 0 && _samples[0].Time < cutoff)
            _samples.RemoveAt(0);

        while (_markers.Count > 0 && _markers[0].Time < cutoff)
            _markers.RemoveAt(0);
    }

    public override void _Draw()
    {
        Rect2 bounds = new Rect2(Vector2.Zero, Size);

        DrawRect(bounds, new Color(0.025f, 0.045f, 0.075f, 0.92f), true);
        DrawRect(bounds, new Color(0.12f, 0.55f, 0.72f, 0.65f), false, 1.0f);

        float leftInset = 32.0f;
        float rightInset = 8.0f;
        float topInset = 22.0f;
        float bottomInset = 16.0f;

        Rect2 plot = new Rect2(
            leftInset,
            topInset,
            Mathf.Max(1.0f, Size.X - leftInset - rightInset),
            Mathf.Max(1.0f, Size.Y - topInset - bottomInset)
        );

        Font font = ThemeDB.FallbackFont;
        Color muted = new Color(0.52f, 0.66f, 0.73f, 0.90f);
        Color grid = new Color(0.18f, 0.34f, 0.42f, 0.78f);
        Color baseline = new Color(0.83f, 0.65f, 0.25f, 0.85f);

        DrawString(
            font,
            new Vector2(8, 14),
            "SIMULATED LOAD DYNAMICS  •  FIXED 0–100%  •  12 s WINDOW",
            HorizontalAlignment.Left,
            -1,
            10,
            muted
        );

        for (int percent = 0; percent <= 100; percent += 50)
        {
            float y = MapLoad(percent, plot);

            DrawLine(
                new Vector2(plot.Position.X, y),
                new Vector2(plot.End.X, y),
                percent == 50 ? baseline : grid,
                percent == 50 ? 1.4f : 1.0f
            );

            DrawString(
                font,
                new Vector2(3, y + 4),
                $"{percent}",
                HorizontalAlignment.Left,
                25,
                9,
                muted
            );
        }

        if (_samples.Count == 0)
        {
            DrawString(
                font,
                new Vector2(plot.GetCenter().X - 62, plot.GetCenter().Y + 4),
                "Awaiting synthetic samples",
                HorizontalAlignment.Left,
                -1,
                10,
                muted
            );
            return;
        }

        double latestTime = _samples[^1].Time;
        double rightTime = Math.Max(WindowSeconds, latestTime);
        double leftTime = rightTime - WindowSeconds;

        foreach (CueMarker marker in _markers)
        {
            if (marker.Time < leftTime || marker.Time > rightTime)
                continue;

            float x = MapTime(marker.Time, leftTime, rightTime, plot);
            Color markerColor = CueColor(marker.Cue);

            DrawLine(
                new Vector2(x, plot.Position.Y),
                new Vector2(x, plot.End.Y),
                new Color(markerColor.R, markerColor.G, markerColor.B, 0.58f),
                1.0f
            );

            DrawString(
                font,
                new Vector2(x + 2, plot.Position.Y + 10),
                CueAbbreviation(marker.Cue),
                HorizontalAlignment.Left,
                -1,
                9,
                markerColor
            );
        }

        for (int i = 1; i < _samples.Count; i++)
        {
            LoadSample previous = _samples[i - 1];
            LoadSample current = _samples[i];

            if (current.Time < leftTime)
                continue;

            Vector2 previousLeft = new Vector2(
                MapTime(previous.Time, leftTime, rightTime, plot),
                MapLoad(previous.Left, plot)
            );

            Vector2 currentLeft = new Vector2(
                MapTime(current.Time, leftTime, rightTime, plot),
                MapLoad(current.Left, plot)
            );

            Vector2 previousRight = new Vector2(
                MapTime(previous.Time, leftTime, rightTime, plot),
                MapLoad(previous.Right, plot)
            );

            Vector2 currentRight = new Vector2(
                MapTime(current.Time, leftTime, rightTime, plot),
                MapLoad(current.Right, plot)
            );

            DrawLine(
                previousRight,
                currentRight,
                new Color(0.93f, 0.42f, 0.47f, 0.72f),
                1.4f,
                true
            );

            DrawLine(
                previousLeft,
                currentLeft,
                new Color(0.12f, 0.86f, 0.96f, 1.0f),
                2.0f,
                true
            );
        }

        DrawString(
            font,
            new Vector2(plot.Position.X, Size.Y - 3),
            "L",
            HorizontalAlignment.Left,
            -1,
            9,
            new Color(0.12f, 0.86f, 0.96f, 1.0f)
        );

        DrawString(
            font,
            new Vector2(plot.Position.X + 14, Size.Y - 3),
            "= left   R",
            HorizontalAlignment.Left,
            -1,
            9,
            muted
        );

        DrawString(
            font,
            new Vector2(plot.Position.X + 67, Size.Y - 3),
            "= right",
            HorizontalAlignment.Left,
            -1,
            9,
            new Color(0.93f, 0.42f, 0.47f, 0.90f)
        );
    }

    private static float MapLoad(float load, Rect2 plot)
    {
        float normalized = Mathf.Clamp(load, 0.0f, 100.0f) / 100.0f;
        return plot.End.Y - normalized * plot.Size.Y;
    }

    private static float MapTime(
        double time,
        double leftTime,
        double rightTime,
        Rect2 plot)
    {
        double normalized = Mathf.Clamp(
            (float)((time - leftTime) / (rightTime - leftTime)),
            0.0f,
            1.0f
        );

        return plot.Position.X + (float)normalized * plot.Size.X;
    }

    private static Color CueColor(string cue)
    {
        return cue switch
        {
            "SHIFT LEFT" => new Color(0.12f, 0.86f, 0.96f, 1.0f),
            "SHIFT RIGHT" => new Color(0.93f, 0.42f, 0.47f, 1.0f),
            _ => new Color(0.94f, 0.72f, 0.25f, 1.0f)
        };
    }

    private static string CueAbbreviation(string cue)
    {
        return cue switch
        {
            "SHIFT LEFT" => "L",
            "SHIFT RIGHT" => "R",
            _ => "C"
        };
    }
}

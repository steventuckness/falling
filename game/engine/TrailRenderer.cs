using Godot;
using System;
using System.Collections.Generic;

public class TrailRenderer : Node2D {
    List<Vector2> points = new List<Vector2>();

    [Export]
    public int MaxPoints = 30;

    [Export]
    public Color StartingColor = new Color(1.0f, 0, 0);

    [Export]
    public Color EndingColor = new Color(0, 0, 0, 0);

    [Export]
    public Vector2 Size = new Vector2(10.0f, 10.0f);

    [Export]
    public bool IsEnabled {
        get { return _isEnabled; }
        set {
            _isEnabled = value;
            if (!value) {
                this.points = new List<Vector2>();
                this.Update();
            }
        }
    }

    private bool _isEnabled = true;

    public override void _Process(float delta) {
        if (!IsEnabled) return;
        this.points.Add(this.GlobalPosition);
        this.Update();
    }

    private void TrimTail() {
        if (this.points.Count > this.MaxPoints) {
            this.points.RemoveRange(0, this.points.Count - this.MaxPoints);
        }
    }

    public override void _Draw() {
        TrimTail();
        for (int i = 0; i<this.points.Count; i++) {
            float t = (float) i / (float) this.points.Count;
            Color color = EndingColor.LinearInterpolate(StartingColor, t);
            var pos = CenterVector(this.ToLocal(this.points[i]), Size);
            this.DrawRect(new Rect2(pos, Size), color, true);
        }
    }

    private Vector2 CenterVector(Vector2 vector, Vector2 size) =>
        new Vector2(vector.x - (size.x / 2), vector.y - (size.y / 2));
}

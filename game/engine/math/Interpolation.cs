using Godot;
using System;

public static class Interpolation {
    public static float Repeat(float t, float length) {
        return t - Mathf.Floor(t / length) * length;
    }

    public static float PingPong(float t, float length) {
        t = Interpolation.Repeat(t, length * 2f);
        return length - Mathf.Abs(t - length);
    }
}

using System;
using Godot;

public class PlayerColor {
    public enum Value {
        Red,
        Green,
        Blue
    }

    public static Color ToColor(Value v) {
        switch (v) {
            case Value.Red:
                return new Color(1.0f, 0.0f, 0.0f);
            case Value.Green:
                return new Color(0.0f, 1.0f, 0.0f);
            case Value.Blue:
                return new Color(0.0f, 0.0f, 1.0f);
        }
        return new Color(1.0f, 1.0f, 1.0f);
    }
}

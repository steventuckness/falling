using Godot;
using System;

public class LevelSelectCard : Node2D {
    private Label GetLabel() => GetNode("Label") as Label;
    private Sprite Selection() => GetNode("Selection") as Sprite;

    public static LevelSelectCard Instantiate() =>
        ((PackedScene)ResourceLoader.Load("res://ui/LevelSelectCard.tscn"))
        .Instance() as LevelSelectCard;

    public String Text {
        get {
            return GetLabel().GetText();
        }
        set {
            GetLabel().SetText(value);
        }
    }

    public bool IsSelected {
        get {
            return Selection().IsVisible();
        }
        set {
            Selection().SetVisible(value);
        }
    }

    public override void _Ready() {

    }

}

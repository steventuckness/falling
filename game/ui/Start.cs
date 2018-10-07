using Godot;
using System;

public class Start : Control
{
    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey && @event.IsPressed()) {
            this.GetTree().ChangeScene("res://ui/LevelSelect.tscn");
        }   
    }
}

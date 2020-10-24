using Godot;
using System;

public class Start : Control
{
	public override void _Input(InputEvent @event) {	
		if (@event.IsActionPressed("ui_accept")) {	
			this.GetTree().ChangeScene("res://ui/LevelSelect.tscn");
		}
	}
}

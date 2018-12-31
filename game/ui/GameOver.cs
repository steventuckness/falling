using Godot;
using System;

public class GameOver : Control
{
    private VBoxContainer vBoxContainer;
    
    private Godot.Collections.Array  buttons;

    public override void _Ready()
    {
        this.vBoxContainer = this.GetNode("VBoxContainer") as VBoxContainer;
        this.buttons = new Godot.Collections.Array(); 

        var children = this.vBoxContainer.GetChildren();
        for (int i = 0; i < children.Count; i++) {
            if (children[i] is Button) {
                this.buttons.Add(children[i]);
            }
        }

        if (buttons.Count > 0) {
            ((Button)this.buttons[0]).GrabFocus();
        }
    }

    public void _on_LevelSelectButton_pressed() {
        this.GetTree().ChangeScene("res://../ui/LevelSelect.tscn");
    }

    public void _on_QuitButton_pressed() {
        this.GetTree().Quit();
    }
}

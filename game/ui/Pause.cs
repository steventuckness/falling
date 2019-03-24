using Godot;
using System;

public class Pause : Control
{
    private VBoxContainer vBoxContainer;
    
    private Godot.Collections.Array buttons;
    
    public override void _Ready() {
        Visible = false;

        this.vBoxContainer = this.GetNode("VBoxContainer") as VBoxContainer;
        this.buttons = this.vBoxContainer.GetChildren();
    }

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("key_pause")) {
            this.TogglePause();
        }
    }

    public void _on_ResumeButton_pressed() {
        this.DisablePause();
    }

    public void _on_RestartButton_pressed() {
        this.DisablePause();
        this.GetTree().ReloadCurrentScene();
    }

    public void _on_LevelSelectButton_pressed() {
        this.DisablePause();
        this.GetTree().ChangeScene("res://../ui/LevelSelect.tscn");
    }

    public void _on_QuitButton_pressed() {
        this.GetTree().Quit();
    }

    private void TogglePause() {
       if (GetTree().Paused == true) {
           this.DisablePause();
       } else {
           this.EnablePause();
       }
    }

    private void DisablePause() {
        GetTree().Paused = false;
        Visible = false;
    }

    private void EnablePause() {
       GetTree().Paused = true;
       Visible = true;
       ((Button)this.buttons[0]).GrabFocus();
    }
}

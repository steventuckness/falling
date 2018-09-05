using Godot;
using System;
using System.Collections.Generic;

public class FinishOverlay : Node2D
{
   public static String SIGNAL_GO_TO_NEXT_LEVEL = "FinishOverlay::nextLevel";
   
   private PopupDialog popupDialog;

   private Label popupDialogLabel;

    public override void _Ready()
    {
        this.AddUserSignal(FinishOverlay.SIGNAL_GO_TO_NEXT_LEVEL);

        this.popupDialog = (PopupDialog)GetNode("CanvasLayer").GetNode("PopupDialog");
        this.popupDialogLabel = (Label)this.popupDialog.GetNode("Label");
    }

    public override void _Process(float delta)
    {
        if ((Input.IsActionJustPressed("ui_accept") || 
            Input.IsActionJustPressed("ui_cancel")) && this.popupDialog.Visible) {
                NextLevel();
        }
    }

    public void ShowOverlay(long timeElapsed, int clonesCreated) { 
        Godot.Dictionary datimeTimeElapsed = OS.GetDatetimeFromUnixTime((int)timeElapsed / 1000);

        object hours;
        object minutes;
        object seconds;
        int milliseconds;

        datimeTimeElapsed.TryGetValue("hour", out hours);
        datimeTimeElapsed.TryGetValue("minute", out minutes);
        datimeTimeElapsed.TryGetValue("second", out seconds);
        milliseconds = (int)timeElapsed % 1000; // get last 3 digits for more precision

        this.popupDialogLabel.SetText($"Finished the level in {hours}:{minutes}:{seconds}:{milliseconds}");
        ((Label)this.popupDialog.GetNode("ClonesTaken")).SetText($"{clonesCreated} clones were used.");
        this.popupDialog.ShowModal(true);
    }

    public void NextLevel() {
        GetTree().SetPause(false);
        this.popupDialog.ShowModal(false);
        this.EmitSignal(FinishOverlay.SIGNAL_GO_TO_NEXT_LEVEL);
    }
}

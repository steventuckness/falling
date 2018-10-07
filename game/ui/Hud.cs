using Godot;
using System;

public class Hud : Control
{
    private PlayerNode player;
    private Recorder.FrameRecorder<PlayerRecorderFrame> cloneRecorder;

    private ColorRect colorRectFront;

    private const int HEIGHT = 3;

    public override void _Ready()
    {
        this.player = (PlayerNode)this.GetNode("../../Player");
        this.colorRectFront = (ColorRect)this.GetNode("ColorRectFront");
        this.colorRectFront.Visible = false;
        this.colorRectFront.RectSize = new Vector2(0, 0);
    }

    public override void _Process(float delta)
    {         
        if (cloneRecorder == null) {
            this.cloneRecorder = this.player.CloneRecorder;
        }
        Update();
    }

    public override void _Draw() {
        if (this.cloneRecorder != null && this.cloneRecorder.IsRecording) {
            float percentage = this.cloneRecorder.GetRecordCapacityUsedPerentage();

            this.colorRectFront.Visible = true;
            this.colorRectFront.Color = PlayerColor.ToColor(this.player.GetColor());
            float width = this.GetViewport().Size.x * (1 - percentage);
            this.colorRectFront.RectSize = new Vector2(width, HEIGHT);
        }   
    }
}

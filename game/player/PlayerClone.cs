using System;
using System.Collections.Generic;
using Godot;

public class PlayerClone : Player {
    public Recorder.Recording<PlayerRecorderFrame> recording;
    public Color color;
    private Recorder.FramePlayer<PlayerRecorderFrame> movementPlayer;

    public override void _Ready() {
        this.menuEnabled = false;
        base._Ready();
        var sprite = (Sprite)this.GetNode("Sprite");
        sprite.SetModulate(color);
        this.movementPlayer = new Recorder.FramePlayer<PlayerRecorderFrame>(
            this.recording,
            (state) => state.ApplyOnPlayer(this)
        );
        this.movementPlayer.StartPlayback();
    }

    public override void _Process(float delta) {
        this.movementPlayer.Process(delta);
    }

    public override void _PhysicsProcess(float delta) {

    }

    public override bool IsActionPressed(string key) {
        return false;
    }

    public override bool IsActionJustPressed(string key) {
        return false;
    }

    public override bool IsActionJustReleased(string key) {
        return false;
    }
}

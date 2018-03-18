
using Godot;
using System;

public class PlayerNode : Entity {

    public Player implementation {
        get { return _implementation; }
        set {
            _implementation = value;
            _implementation.node = this;
        }
    }

    public bool paused {
        get { return _implementation.paused; }
        set {
            _implementation.paused = value;
        }
    }

    public Vector2 velocity {
        get { return _implementation.velocity; }
        set {
            _implementation.velocity = value;
        }
    }

    private Player _implementation;

    public void CollideNoop() {
        return;
    }

    public void CollideVertical() {
        this._implementation.velocity.y = 0;
    }

    public override void _PhysicsProcess(float delta) {
        implementation._PhysicsProcess(delta);
    }

    public override void _Process(float delta) {
        implementation._Process(delta);
    }

    public override void _Ready() {
        base._Ready();
        if (implementation == null) {
            implementation = new Player();
        }
        implementation._Ready();
    }

    public void Respawn() {
        implementation.Respawn();
    }

    public bool IsFalling() =>
        implementation.IsFalling();

    public bool IsJumping() =>
        implementation.IsJumping();

    public bool IsDead() =>
        implementation.IsDead();

    public bool IsOnFloor() =>
        implementation.IsOnFloor();

    public void RecordingStarted() => implementation.RecordingStarted();

    public void RecordingStopped() => implementation.RecordingStopped();

    public Recording GetRecording() => implementation.GetRecording();
}

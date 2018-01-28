using Godot;
using System;

public class Player : KinematicBody2D {
    // STATE
    private StateMachine<Player> sm = new StateMachine<Player>();
    public Idle    stateIdle = new Idle();
    public Gliding stateGliding = new Gliding();
    public Falling stateFalling = new Falling();
    public Walking stateWalking = new Walking();

    // Physics /////////////////////////////////////////////////////////////////
    public Vector2 velocity = new Vector2(0, 0);
    public float gravity = 10.0f;

    // Ground
    [Export]
    public float groundFriction         = 100.0f; // px / sec
    [Export]
    public float groundAcceleration     = 100.0f; // px / sec
    [Export]
    public float groundMaxSpeed         = 400.0f; // px / sec

    public enum ANIMATION {
        WALKING
    };

    public override void _Ready() {
        this.sm.Init(this.stateIdle);
        this.playAnimation(ANIMATION.WALKING);
    }

    public void playAnimation(ANIMATION animation) {
        AnimationPlayer player = (AnimationPlayer) this.GetNode("animation");
        switch (animation) {
            case ANIMATION.WALKING:
                player.Play("walking");
                break;
        }
    }

    public override void _PhysicsProcess(float delta) {
        this.sm.Update(delta, this);
    }
}

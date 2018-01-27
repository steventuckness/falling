using Godot;
using System;

public class Player : Node2D {
    // STATE
    private StateMachine<Player> sm = new StateMachine<Player>();
    public Idle    stateIdle = new Idle();
    public Gliding stateGliding = new Gliding();
    public Falling stateFalling = new Falling();

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
}

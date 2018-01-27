using Godot;
using System;

public class Player : Node2D {
    // STATE
    private StateMachine<Player> sm = new StateMachine<Player>();
    public Idle    stateIdle = new Idle();
    public Gliding stateGliding = new Gliding();
    public Falling stateFalling = new Falling();

    public override void _Ready() {
        this.sm.Init(this.stateIdle);
    }

    public override void _PhysicsProcess(float delta) {
        this.sm.Update(delta, this);
    }
}

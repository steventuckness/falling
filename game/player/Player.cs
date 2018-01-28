using Godot;
using System;

public class Player : KinematicBody2D {
    // State ///////////////////////////////////////////////////////////////////
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

    // Collision ///////////////////////////////////////////////////////////////
    public PlayerCollision collision; // Mostly for storing pre-move collision checks

    public enum Direction {
        Left,
        Right
    }    

    public enum Animation {
        Walking
    };

    public override void _Ready() {
        this.collision = new PlayerCollision(this);
        this.sm.Init(this.stateIdle);
        this.playAnimation(Animation.Walking);

        // Create custom collision shape
        RectangleShape2D r = new RectangleShape2D();
        r.SetExtents(new Vector2(8 - 0.001f, 16 - 0.001f));

        CollisionShape2D c = (CollisionShape2D) this.GetNode("CollisionShape2D");
        GD.Print(this.GetSafeMargin());
        c.SetShape(r);

         this.SetSafeMargin(0.001f);
    }

    public void playAnimation(Animation animation) {
        AnimationPlayer player = (AnimationPlayer) this.GetNode("animation");
        switch (animation) {
            case Animation.Walking:
                player.Play("walking");
                break;
        }
    }

    public override void _PhysicsProcess(float delta) {
        this.collision.Update();
        this.sm.Update(delta, this);
    }

    public void applyGravity(float delta, float terminalVelocity) {
        if (this.velocity.y < terminalVelocity) {
            this.velocity = new Vector2(this.velocity.x, this.velocity.y + (delta * this.gravity));
        }
    }
}

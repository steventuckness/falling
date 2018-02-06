using Godot;
using System;

public class Player : KinematicBody2D {
    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Player> sm = new StateMachine<Player>();
    public Idle     stateIdle       = new Idle();
    public Gliding  stateGliding    = new Gliding();
    public Falling  stateFalling    = new Falling();
    public Walking  stateWalking    = new Walking();
    public Jumping  stateJumping    = new Jumping();
    public Dead     stateDead       = new Dead();
    public Respawn  stateRespawn    = new Respawn();

    // Physics /////////////////////////////////////////////////////////////////
    public Vector2 velocity = new Vector2(0, 0);

    [Export]
    public float gravity = 200.0f;

    public Direction direction = Direction.Right;
    private Animation animation;

    [Export]
    public float  jumpSpeed = 120f; // px / sec

    // Ground
    [Export]
    public float groundFriction         = 200.0f; // px / sec
    [Export]
    public float groundAcceleration     = 150.0f; // px / sec
    [Export]
    public float groundMaxSpeed         = 400.0f; // px / sec

    // Collision /////////////////////////////////////////////////////////////// 
    public PlayerCollision collision; // Mostly for storing pre-move collision checks 
    private float skinWidth = 0.001f; 

    // Falling
    [Export]
    public float fallingMaxSpeed = 50.0f;

    [Export]
    public float impactDeadSpeed = 30.0f;
	
    // Gliding ////////////////////////////////////////////////////////////////
    [Export]
    public float glideGravity = 5.0f;


    [Export]
    public float glideBumpAcceleration = 20.0f;

    [Export]
    public float glideTopBumpVelocity = 100.0f;

    [Export]
    public Vector2 glideMaxSpeed = new Vector2(10.0f, 10.0f);

    [Export]
    public float glideHorizontalAcceleration = 3.0f;

    // MISC ////////////////////////////////////////////////////////////////////
    public Vector2 carry = new Vector2(0, 0);
    [Export]
    public float respawnTime           = 2.0f;  // Seconds

    public enum Direction {
        Left,
        Right
    }    

    public enum Animation {
        Walking,
        Gliding,
        Jumping,
        Falling,
        Idle
    };

    public override void _Ready() {
        this.AddUserSignal("playerDied");
        
        this.collision = new PlayerCollision(this); 
        this.sm.Init(this.stateIdle);
        this.PlayAnimation(Animation.Walking);
    }

    public bool IsDead() {
        return this.sm.GetCurrentState() == this.stateDead;
    }

    public bool CanBeKilled() {
        return this.sm.GetCurrentState() != this.stateRespawn && !this.IsDead();
    }

    public void Kill() {
        // Player cannot be killed while respawning or is already dead
        if(!this.CanBeKilled()) {
            return;
        }
        this.sm.TransitionState(this.stateDead);
    }

    public void PlayAnimation(Animation animation) {
        AnimationPlayer player = (AnimationPlayer) this.GetNode("animation");
        string directionStr = (direction == Direction.Left ? "left" : "right");
        this.animation = animation;
        switch (animation) {
            case Animation.Walking:
                player.Play("walking_" + directionStr);
                break;
            case Animation.Gliding:
                player.Play("gliding_" + directionStr);
                break;
            case Animation.Jumping:
                player.Play("jumping_" + directionStr);
                break;
            case Animation.Falling:
                player.Play("falling_" + directionStr);
                break;
            case Animation.Idle:
                player.Play("idle_" + directionStr);
                break;
        }
    }

    public override void _PhysicsProcess(float delta) {
        // this.collision.Update();
        this.sm.Update(delta, this);
    }

 	public Vector2 Move(float slopeStop) { 
        this.velocity = this.MoveAndSlide(this.velocity, new Vector2(0, -1), slopeStop, 4, 1.06f); 
        return this.velocity; 
	}

    public void ApplyGravity(float delta, float terminalVelocity) {
        this.velocity = Acceleration.ApplyTerminalY(terminalVelocity, this.gravity, delta, this.velocity);
    }

    public void DetectDirectionChange() {
        if (Input.IsActionPressed("key_left") && this.direction != Direction.Left) {
            GD.Print("Direction: Left");
            this.direction = Direction.Left;
            this.PlayAnimation(this.animation);
        }
        if (Input.IsActionPressed("key_right") && this.direction != Direction.Right) {
            GD.Print("Direction: Right");
            this.direction = Direction.Right;
            this.PlayAnimation(this.animation);
        }
    }

    public int GetDirectionMultiplier() {
        return this.direction == Direction.Left ? -1 : 1;
    }
}

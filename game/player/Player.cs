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
    public Knockback stateKnockback = new Knockback();
    public bool paused = false;

    // Signals /////////////////////////////////////////////////////////////////
    public static String SIGNAL_DIED = "Player::died";
    public static String SIGNAL_RESPAWN = "Player::respawn";

    // Physics /////////////////////////////////////////////////////////////////
    public Vector2 velocity = new Vector2(0, 0);

    private float groundImpactSpeed = 0.0f; // px / sec

    [Export]
    public float gravity = 200.0f;

    public Direction direction = Direction.Right;
    private Animation animation;

    // Air
    [Export]
    public float  jumpHeight            = 56f;
    [Export]
    public float airAcceleration        = 100f;
    [Export]
    public float airFriction            = 50f;
    [Export]
    public float airMaxSpeed            = 200f;

    // Ground
    [Export]
    public float groundFriction         = 200.0f;
    [Export]
    public float groundAcceleration     = 150.0f;
    [Export]
    public float groundMaxSpeed         = 400.0f;
    [Export]
    public float groundSprintMaxSpeed   = 500.0f;

    // Collision /////////////////////////////////////////////////////////////// 
    public PlayerCollision collision; // Mostly for storing pre-move collision checks

    // Falling
    [Export]
    public float fallingMaxSpeed = 50.0f;

    [Export]
    public float impactDeadSpeed = 30.0f;
	
    // Gliding ////////////////////////////////////////////////////////////////
    [Export]
    public float glideMaxYSpeed = 20.0f;


    [Export]
    public float glideDrag = 30.0f;

    [Export]
    public float glideLift = 9.0f;


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
        this.AddUserSignal(Player.SIGNAL_DIED);
        
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

    public void Respawn() {
        this.sm.TransitionState(this.stateRespawn);
    }

    public void PlayAnimation(Animation animation) {
        AnimationPlayer player = (AnimationPlayer) this.GetNode("Animation");
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
        if(this.paused) {
            return;
        }
        // this.collision.Update();
        this.sm.Update(delta, this);
    }

 	public Vector2 Move(float slopeStop) { 
        this.velocity = this.MoveAndSlide(this.velocity, new Vector2(0, -1), slopeStop, 4, 1.06f); 
        return this.velocity; 
	}

    public void ApplyGravity(float delta) {
        this.velocity = Acceleration.ApplyTerminalY(this.fallingMaxSpeed, this.gravity, delta, this.velocity);
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

    public int GetInputDirection() {
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;

        return isRight - isLeft;
    }

    public void AirControl(float delta) {
        int dir = this.GetInputDirection();
        // Air friction
        if(this.velocity.x != 0) {
            this.velocity = Acceleration.ApproachX(
                0f, this.airFriction, delta, this.velocity
            );
        }

        // Air control
        this.velocity = Acceleration.ApplyTerminalX(
            float.MaxValue,
            this.airAcceleration * dir,
            delta,
            this.velocity
        );
    }

    public State<Player> DetectDeathByFalling() {
        bool isGrounded = this.IsOnFloor();

        if (!isGrounded) {
            groundImpactSpeed = this.velocity.y;
        }

        if (groundImpactSpeed >= this.impactDeadSpeed && isGrounded) {
            return this.stateDead; 
        }

        if (isGrounded) {
            return this.stateIdle;
        }
        return null;
    }

    public int GetDirectionMultiplier() {
        return this.direction == Direction.Left ? -1 : 1;
    }
}



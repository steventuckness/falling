using Godot;
using System;

public class Player : KinematicBody2D {
    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Player> sm = new StateMachine<Player>();
    public Idle    stateIdle = new Idle();
    public Gliding stateGliding = new Gliding();
    public Falling stateFalling = new Falling();
    public Walking stateWalking = new Walking();
    public Jumping stateJumping = new Jumping();

    // Physics /////////////////////////////////////////////////////////////////
    public Vector2 velocity = new Vector2(0, 0);
    public float gravity = 10.0f;
    public Direction direction = Direction.Right;
    private Animation animation;

    [Export]
    public float  jumpSpeed = 200f; // px / sec

    // Ground
    [Export]
    public float groundFriction         = 100.0f; // px / sec
    [Export]
    public float groundAcceleration     = 100.0f; // px / sec
    [Export]
    public float groundMaxSpeed         = 400.0f; // px / sec

    // Collision /////////////////////////////////////////////////////////////// 
    public PlayerCollision collision; // Mostly for storing pre-move collision checks 
    private float skinWidth = 0.001f; 

    // Falling
    [Export]
    public float fallingMaxSpeed = 20.0f;
	
    // MISC //////////////////////////////////////////////////////////////////// 
    public Vector2 carry = new Vector2(0, 0); 

    // Gliding ////////////////////////////////////////////////////////////////
    [Export]
    public float glideGravity = 5.0f;

    [Export]
    // How much of a "jolt" the bump will create
    public float glideBumpFactor = -10.0f;

    [Export]
    // How long will the bump acceleration will remain
    public float glideBumpTime = 1.0f;

    [Export]
    public Vector2 glideMaxSpeed = new Vector2(10.0f, 10.0f);

    [Export]
    public float glideHorizontalAcceleration = 3.0f;

    public enum Direction {
        Left,
        Right
    }    

    public enum Animation {
        Walking,
        Gliding
    };

    public override void _Ready() {
		this.collision = new PlayerCollision(this); 
        this.sm.Init(this.stateIdle);
        this.PlayAnimation(Animation.Walking);
		
		// Create custom collision shape 
        RectangleShape2D r = new RectangleShape2D(); 
        r.SetExtents(new Vector2(8 - this.skinWidth, 16 - this.skinWidth)); 
 
        CollisionShape2D c = (CollisionShape2D) this.GetNode("CollisionShape2D"); 
        c.SetShape(r); 
 
        this.SetSafeMargin(this.skinWidth); 
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

    public void applyGravity(float delta, float terminalVelocity) {
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
}

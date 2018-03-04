using Godot;
using System;

public class Player {

    public PlayerNode node;

    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Player> sm = new StateMachine<Player>();
    public Idle stateIdle = new Idle();
    public Falling stateFalling = new Falling();
    public Walking stateWalking = new Walking();
    public Jumping stateJumping = new Jumping();
    public Dead stateDead = new Dead();
    public Respawn stateRespawn = new Respawn();
    public Knockback stateKnockback = new Knockback();
    public bool paused = false;

    // Signals /////////////////////////////////////////////////////////////////
    public static String SIGNAL_DIED = "Player::died";
    public static String SIGNAL_RESPAWN = "Player::respawn";

    // Physics /////////////////////////////////////////////////////////////////
    public Vector2 velocity = new Vector2(0, 0);

    private float groundImpactSpeed = 0.0f; // px / sec

    public float gravity = 900.0f;

    public Direction direction = Direction.Right;
    private Animation animation;

    // Air
    public float jumpHeight = 56f;
    public float airAcceleration = 400f;
    public float airFriction = 50f;
    public float airMaxSpeed = 200f;
    public float airMult = 0.65f;

    // Ground
    public float groundFriction = 2000.0f;
    public float groundAcceleration = 1000.0f;
    public float groundMaxSpeed = 90.0f;
    public float groundSprintMaxSpeed = (90.0f * 1.25f);

    // Collision /////////////////////////////////////////////////////////////// 
    public PlayerCollision collision; // Mostly for storing pre-move collision checks

    // Falling
    public float fallingMaxSpeed = 160.0f;

    // Gliding ////////////////////////////////////////////////////////////////
    public float glideMaxYSpeed = 20.0f;
    public float glideDrag = 30.0f;
    public float glideLift = 9.0f;
    public int glideAgainWaitTime = 300; // milliseconds

    // MISC ////////////////////////////////////////////////////////////////////
    public Vector2 carry = new Vector2(0, 0);
    public float respawnTime = 2.0f;  // Seconds

    // UI //////////////////////////////////////////////////////////////////////
    public CloneMenu cloneMenu;

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

    public void _Ready() {
        this.node.AddUserSignal(Player.SIGNAL_DIED);
        
        this.collision = new PlayerCollision(this); 
        this.sm.Init(this.stateIdle);
        this.PlayAnimation(Animation.Walking);

        this.cloneMenu = (CloneMenu) this.node.GetNode("Node2D/Menu");
        this.cloneMenu.SetOptions(CloneOptions.OptionsFrom(
            new CloneOptions.ECloneOption[] {
                CloneOptions.ECloneOption.RED,
                CloneOptions.ECloneOption.GREEN,
                CloneOptions.ECloneOption.BLUE
            }
        ));
        this.cloneMenu.Hide();
    }

    public bool IsDead() {
        return this.sm.GetCurrentState() == this.stateDead;
    }

    public bool CanBeKilled() {
        return this.sm.GetCurrentState() != this.stateRespawn && !this.IsDead();
    }

    public void Kill() {
        // Player cannot be killed while respawning or is already dead
        if (!this.CanBeKilled()) {
            return;
        }
        this.sm.TransitionState(this.stateDead);
    }

    public void Respawn() {
        this.sm.TransitionState(this.stateRespawn);
    }

    public bool IsJumping() {
        return (this.sm.GetCurrentState() == this.stateJumping);
    }

    public bool IsFalling() {
        return (this.sm.GetCurrentState() == this.stateFalling);
    }

    public void PlayAnimation(Animation animation) {
        // AnimationPlayer player = (AnimationPlayer) this.GetNode("Animation");
        // string directionStr = (direction == Direction.Left ? "left" : "right");
        // this.animation = animation;
        // switch (animation) {
        //     case Animation.Walking:
        //         player.Play("walking_" + directionStr);
        //         break;
        //     case Animation.Gliding:
        //         player.Play("gliding_" + directionStr);
        //         break;
        //     case Animation.Jumping:
        //         player.Play("jumping_" + directionStr);
        //         break;
        //     case Animation.Falling:
        //         player.Play("falling_" + directionStr);
        //         break;
        //     case Animation.Idle:
        //         player.Play("idle_" + directionStr);
        //         break;
        // }
    }

    public void _PhysicsProcess(float delta) {
        if(this.paused) {
            return;
        }
        if (Input.IsActionPressed("key_up")) {
            this.cloneMenu.Show();
        }
        else {
            this.cloneMenu.Hide();
        }

        this.sm.Update(delta, this);
    }

 	public Vector2 Move(float slopeStop) { 
        this.velocity = this.node.MoveAndSlide(this.velocity, new Vector2(0, -1), slopeStop, 4, 1.06f); 
        return this.velocity; 
	}

    public void ApplyGravity(float delta) {
        // this.velocity = Acceleration.ApplyTerminalY(this.fallingMaxSpeed, this.gravity, delta, this.velocity);
        this.velocity = Acceleration.ApproachY(
            this.fallingMaxSpeed,
            this.gravity,
            delta,
            this.velocity
        );
    }

    public void DetectDirectionChange() {
        if (this.IsActionPressed("key_left") && this.direction != Direction.Left) {
            GD.Print("Direction: Left");
            this.direction = Direction.Left;
            this.PlayAnimation(this.animation);
        }
        if (this.IsActionPressed("key_right") && this.direction != Direction.Right) {
            GD.Print("Direction: Right");
            this.direction = Direction.Right;
            this.PlayAnimation(this.animation);
        }
    }

    public int GetInputDirection() {
        int isLeft = this.IsActionPressed("key_left") ? 1 : 0;
        int isRight = this.IsActionPressed("key_right") ? 1 : 0;

        return isRight - isLeft;
    }

    public void AirControl(float delta) {
        int dir = this.GetInputDirection();
        // Air friction
        if (this.velocity.x != 0 && dir == 0) {
            this.velocity = Acceleration.ApproachX(
                0f, this.airFriction, delta, this.velocity
            );
        }
        // Air control
        else if (dir != 0) {
            this.velocity = Acceleration.ApproachX(
                dir * this.groundMaxSpeed,
                this.groundAcceleration * this.airMult,
                delta,
                this.velocity
            );
        }
    }

    public void GroundControl(float delta) {
        int dir = this.GetInputDirection();

        // Ground friction
        if(dir == 0 && Mathf.Abs(this.velocity.x) > 0) {
            this.velocity = Acceleration.ApproachX(
                0, this.groundFriction, delta, this.velocity
            );
        } 
        // Ground accel
        else if(dir != 0) {
            bool isSprinting = this.IsActionPressed("key_sprint");
            float approachSpeed = dir * (isSprinting ? this.groundSprintMaxSpeed : this.groundMaxSpeed);
            this.velocity = Acceleration.ApproachX(
                approachSpeed, this.groundAcceleration, delta, this.velocity
            );
        }
    }

    public int GetDirectionMultiplier() {
        return this.direction == Direction.Left ? -1 : 1;
    }

    public virtual bool IsActionPressed(string key) {
        return Input.IsActionPressed(key);
    }

    public virtual bool IsActionJustPressed(string key) {
        return Input.IsActionJustPressed(key);
    }

    public virtual bool IsActionJustReleased(string key) {
        return Input.IsActionJustReleased(key);
    }

    // Methods to conform to the KinematicBody interface. Add any extra methods
    // here.

    public void Hide() => this.node.Hide();

    public void EmitSignal(string signal) => this.node.EmitSignal(signal);

    public bool IsOnFloor() => this.node.IsOnFloor();

    public Node GetNode(NodePath path) => this.node.GetNode(path);

    public void SetRotation(float radians) => this.node.SetRotation(radians);

    public Vector2 MoveAndSlide(Vector2 velocity) => this.node.MoveAndSlide(velocity);

    public bool IsOnWall() => this.node.IsOnWall();

    public void Show() => this.node.Show();

    public Vector2 GetGlobalPosition() => this.node.GetGlobalPosition();

    public void SetGlobalPosition(Vector2 position) => this.node.SetGlobalPosition(position);

    public int GetSlideCount() => this.node.GetSlideCount();

    public KinematicCollision2D GetSlideCollision(int slideIdx) => this.node.GetSlideCollision(slideIdx);

    public float GetSafeMargin() => this.node.GetSafeMargin();

    public World2D GetWorld2d() => this.node.GetWorld2d();
}



using Godot;
using System;
using Recorder;
using System.Collections.Generic;

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
    public bool menuEnabled = true;

    private bool wasRecordingDuringDeath = false;

    // Signals /////////////////////////////////////////////////////////////////
    public static String SIGNAL_DIED = "Player::died";
    public static String SIGNAL_RESPAWN = "Player::respawn";
    public static String SIGNAL_CREATE_CLONE = "Player::createClone";

    // Physics /////////////////////////////////////////////////////////////////
    public Vector2 velocity = new Vector2(0, 0);

    public float gravity = 900.0f;

    public Direction direction = Direction.Right;
    public Animation CurrentAnimation { get { return animation; } }
    private Animation animation;

    // Air
    public float jumpHeight = 56f;
    public float airAcceleration = 400f;
    public float airFriction = 50f;
    public float airMaxSpeed = 200f;
    public float airMult = 0.65f;
    public const float WALL_JUMP_GRACE_TIME = 0.3f;
    public const float WALL_CLING_TIME = 0.25f;
    public float wallJumpGraceTime = 0;
    public float wallClingTimer = 0;
    public bool isClinging = false;
    public bool canCling = false;

    // Ground
    public float groundFriction = 2000.0f;
    public float groundAcceleration = 1000.0f;
    public float groundMaxSpeed = 90.0f;
    public float groundSprintMaxSpeed = (90.0f * 1.25f);

    // Collision /////////////////////////////////////////////////////////////// 
    public CollisionEngine collision {
        get {
            return this.node.collision;
        }
    }
    public struct CollisionData {
        public bool top;
        public bool right;
        public bool bottom;
        public bool left;
    }
    public CollisionData collisionData = new CollisionData();

    // Falling
    public float fallingMaxSpeed = 160.0f;
    public bool fallThroughPlatform = false;

    // Gliding ////////////////////////////////////////////////////////////////
    public float glideMaxYSpeed = 20.0f;
    public float glideDrag = 30.0f;
    public float glideLift = 9.0f;
    public int glideAgainWaitTime = 300; // milliseconds

    // MISC ////////////////////////////////////////////////////////////////////
    public Vector2 carry = new Vector2(0, 0);
    public float respawnTime = 2.0f;  // Seconds
    private Recorder.FrameRecorder<PlayerRecorderFrame> cloneRecorder;

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

    private Recording<PlayerRecorderFrame> lastRecording;

    public virtual void _Ready() {
        this.AddUserSignal(Player.SIGNAL_DIED);
        this.AddUserSignal(Player.SIGNAL_CREATE_CLONE);

        this.sm.Init(this.stateIdle);
        this.PlayAnimation(Animation.Walking);
        this.cloneRecorder = new Recorder.FrameRecorder<PlayerRecorderFrame>(
            () => PlayerRecorderFrame.FromPlayer(this),
            this.RecordingStarted,
            this.RecordingStopped
        );

        if (menuEnabled) {
            this.cloneMenu = (CloneMenu)this.node.GetNode("CloneMenu/Menu");
            this.cloneMenu.Hide();
        }
    }

    protected virtual void RecordingStarted() {
        GD.Print("Recording started!");
        var trail = ((TrailRenderer)node.GetNode("trail"));
        trail.StartingColor = this.GetColor();
        trail.IsEnabled = true;
    }

    protected virtual void RecordingStopped(Recorder.Recording<PlayerRecorderFrame> recording) {
        this.lastRecording = recording;
        this.EmitSignal(SIGNAL_CREATE_CLONE);
        ((TrailRenderer)node.GetNode("trail")).IsEnabled = false;
        GD.Print("Recording stopped!");
    }

    public Color GetColor() {
        var sprite = (Sprite)this.node.GetNode("Sprite");
        return sprite.GetModulate();
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

        this.wasRecordingDuringDeath = this.cloneRecorder.IsRecording;
        
        if (this.cloneRecorder.IsRecording) {
            this.cloneRecorder.StopRecording();
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

    public virtual void _PhysicsProcess(float delta) {
        if (this.paused) {
            return;
        }
        if (menuEnabled) {
            if (Input.IsActionPressed("key_up") && this.cloneMenu.Visible == false) {
                this.cloneMenu.Show();
            }
            else if (Input.IsActionJustReleased("key_up") && this.cloneMenu.Visible) {
                this.cloneMenu.Hide();
                var sprite = (Sprite)this.node.GetNode("Sprite");
                sprite.SetModulate(this.cloneMenu.GetSelectedColor());
            }
        }
        // Convenience data for solid collision checks
        this.collisionData.top = this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(0, -1));
        this.collisionData.right = this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(1, 0));
        this.collisionData.bottom = this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(0, 1));
        this.collisionData.left = this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(-1, 0));

        this.sm.Update(delta, this);
    }

    public virtual void _Process(float delta) {
        this.cloneRecorder.Process(delta);
    }

    public void MoveX(float x, Entity.OnCollide onCollide) {
        this.node.MoveX(x, onCollide);
    }

    public void MoveY(float y, Entity.OnCollide onCollide) {
        this.node.MoveY(y, onCollide);
    }

    public Vector2 Move() {
        GD.Print("Player::Move()");
        this.node.MoveX(this.velocity.x, () => {
            this.velocity.x = 0;
        });
        this.node.MoveY(this.velocity.y, () => {
            this.velocity.y = 0;
        });
        return this.velocity;
    }

    public void ApplyGravity(float delta) {
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
        this.UpdateCling(delta);
        int dir = this.GetInputDirection();
        // Air friction
        if (this.velocity.x != 0 && dir == 0) {
            this.velocity = Acceleration.ApproachX(
                0f, this.airFriction, delta, this.velocity
            );
        }
        // Air control
        else if (dir != 0 && !this.IsClinging()) {
            this.velocity = Acceleration.ApproachX(
                dir * this.groundMaxSpeed,
                this.groundAcceleration * this.airMult,
                delta,
                this.velocity
            );
        }
    }
    public void UpdateWallJump(float delta) {
        bool cLeft = this.collisionData.left;
        bool cRight = this.collisionData.right;

        if (!cLeft && !cRight) {
            this.wallJumpGraceTime = Mathf.Max(0, this.wallJumpGraceTime - delta);
        }
        else {
            this.wallJumpGraceTime = WALL_JUMP_GRACE_TIME;
        }
    }
    public void UpdateCling(float delta) {
        int dir = this.GetInputDirection();
        bool cLeft = this.collisionData.left;
        bool cRight = this.collisionData.right;
        
        // Back in the air
        if(!cLeft && !cRight) {
            this.canCling = true;
            this.isClinging = false;
        }

        // Pressing out from a wall
        if (this.canCling && (dir == -1 && cRight || dir == 1 && cLeft)) {
            this.isClinging = true;
            this.wallClingTimer = WALL_CLING_TIME;
            this.canCling = false;
        }
        if(this.isClinging) {
            this.wallClingTimer = Mathf.Max(0, this.wallClingTimer - delta);
            if(this.wallClingTimer <= 0) {
                this.isClinging = false;
            }
        }
    }
    public bool IsClinging() {
        return this.isClinging;
    }
    public bool CanWallJump() {
        int dir = this.GetInputDirection();
        bool cLeft = this.collisionData.left;
        bool cRight = this.collisionData.right;
        bool pushingOff = (dir == -1) ? (!cLeft) : (dir == 1) ? (!cRight) : false;
        return this.wallJumpGraceTime > 0 && (dir != 0) && pushingOff;
    }
    public void ResetWallJump() {
        this.wallJumpGraceTime = 0;
    }
    public Vector2 WallJump(int dir) => new Vector2(50 * dir, -Mathf.Sqrt(2 * this.gravity * this.jumpHeight));

    public void GroundControl(float delta) {
        int dir = this.GetInputDirection();

        // Ground friction
        if (dir == 0 && Mathf.Abs(this.velocity.x) > 0) {
            this.velocity = Acceleration.ApproachX(
                0, this.groundFriction, delta, this.velocity
            );
        }
        // Ground accel
        else if (dir != 0) {
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

    public virtual Recorder.Recording<PlayerRecorderFrame> GetRecording() =>
        this.lastRecording;

    // Methods to conform to the KinematicBody interface. Add any extra methods
    // here.

    public bool IsOnFloor() {
        return this.node.collision.CollideCheck<Solid>(
            this.GetPosition() + new Vector2(0, 1)
        ) || this.node.collision.CollideCheck<PlayerNode>(
            this.GetPosition() + new Vector2(0, 1)
        );
    }

    public virtual bool IsRidingClone(PlayerClone clone) {
        return this.collision.CollideFirst<PlayerNode>(this.GetPosition() + new Vector2(0, 1)) == clone.node;
    }

    public void Hide() => this.node.Hide();

    public void EmitSignal(string signal) => this.node.EmitSignal(signal);

    public void EmitSignal(string signal, params object[] args) => this.node.EmitSignal(signal, args);

    public Node GetNode(NodePath path) => this.node.GetNode(path);

    public void SetRotation(float radians) => this.node.SetRotation(radians);

    public void Show() => this.node.Show();

    public Vector2 GetGlobalPosition() => this.node.GetGlobalPosition();

    public void SetGlobalPosition(Vector2 position) => this.node.SetGlobalPosition(position);

    public World2D GetWorld2d() => this.node.GetWorld2d();

    public void AddUserSignal(string signal) => this.node.AddUserSignal(signal);

    public Vector2 GetPosition() => this.node.GetPosition();

    public void SetPosition(Vector2 position) => this.node.SetPosition(position);

    public void SetCloneOptions(CloneOptions.ECloneOption[] cloneOptions) {
        this.cloneMenu.SetOptions(CloneOptions.OptionsFrom(cloneOptions));
    }

    public List<CloneOptions.CloneOption> GetCloneOptions() {
        return this.cloneMenu.GetOptions();
    }

    public bool WasRecordingDuringDeath {
        get {
            return this.wasRecordingDuringDeath;
        }
    }
}



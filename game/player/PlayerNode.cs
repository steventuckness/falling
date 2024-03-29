
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    private List<PlayerNode> DisableCloneCollisionIfOverlapping() {
        /*
         * We use this method to disable collision against any clones that
         * we are overlapping at the start of a frame. This is possible when
         * the clone spawns directly on top of the player and can cause the
         * player to be stuck.
         * 
         * Be sure to re-enable collision against clones after movement!
         */
        List<PlayerNode> overlaps = this.collision.CollideAll<PlayerNode>(this.GetPosition());
        overlaps.ForEach((PlayerNode overlap) => {
            overlap.IsCollidable = false;
        });
        return overlaps;
    }

    private List<PlayerNode> ReEnableCloneCollision(List<PlayerNode> overlaps) {
        overlaps.ForEach((PlayerNode overlap) => {
            overlap.IsCollidable = true;
        });
        return overlaps;
    }

    private List<Platform> OneWayPlatforms() {
        return this.scene.GetManager()
            .GetEntitiesBy<Platform>()
            .Where((p) => p.isOneWay)
            .ToList();
    }

    private List<Platform> DisableOneWayPlatforms(int dirY, List<Platform> platforms) {
        platforms.ForEach((p) => {
            // If we are already overlapping the platform or we are moving up, disable the platform collision
            if (p.collider.Collides(this.collider) || dirY < 0 || this.implementation.fallThroughPlatform) {
                p.collider.IsCollidable = false;
            }
        });

        return platforms;
    }

    public override void MoveX(float x, OnCollide onCollide) {
        Vector2 pos = this.GetPosition();
        int moveX = this.remainders.UpdateX(x);
        int dirX = (int)Mathf.Sign(moveX);
        bool collided = false;
        if (x == 0 || moveX == 0) {
            return;
        }
        List<PlayerNode> overlaps = this.DisableCloneCollisionIfOverlapping();
        List<Platform> oneWay = this.OneWayPlatforms();
        oneWay.ForEach((p) => {
            if (p.collider.Collides(this.collider)) {
                p.collider.IsCollidable = false;
            }
        });

        for (int i = 0; i < Mathf.Abs(moveX); i++) {
            Vector2 check = this.GetPosition() + new Vector2(dirX, 0);

            if (
                !this.collision.CollideCheck<Solid>(check) &&
                !this.collision.CollideCheck<PlayerNode>(check)
            ) {
                this.SetPosition(this.GetPosition() + new Vector2(dirX, 0));
            }
            else {
                collided = true;
            }
        }

        this.ReEnableCloneCollision(overlaps);
        oneWay.ForEach((p) => {
            p.collider.IsCollidable = true;
        });
        if (collided && onCollide != null) {
            onCollide();
        }
    }

    public override void MoveY(float y, OnCollide onCollide) {
        Vector2 pos = this.GetPosition();
        int moveY = this.remainders.UpdateY(y);
        int dirY = (int)Mathf.Sign(moveY);
        bool collided = false;
        if (y == 0 || moveY == 0) {
            return;
        }
        List<PlayerNode> overlaps = this.DisableCloneCollisionIfOverlapping();
        List<Platform> oneWay = this.DisableOneWayPlatforms(dirY, this.OneWayPlatforms());

        for (int i = 0; i < Mathf.Abs(moveY); i++) {
            Vector2 check = this.GetPosition() + new Vector2(0, dirY);

            if (
                !this.collision.CollideCheck<Solid>(check) &&
                !this.collision.CollideCheck<PlayerNode>(check)) {
                this.SetPosition(this.GetPosition() + new Vector2(0, dirY));
            }
            else {
                collided = true;
            }
        }

        this.ReEnableCloneCollision(overlaps);
        oneWay.ForEach((p) => {
            p.collider.IsCollidable = true;
        });
        this.implementation.fallThroughPlatform = false;

        if (collided && onCollide != null) {
            onCollide();
        }
    }

    public static PlayerNode GetPlayerNodeFromChild(Godot.Object child) {
        if (child == null) {
            return null;
        }
        if (child is PlayerNode) {
            return (PlayerNode)child;
        }
        if (child is Node2D) {
            return GetPlayerNodeFromChild(((Node2D)child).GetParent());
        }
        return null;
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

    public bool IsRidingClone(PlayerClone clone) =>
        implementation.IsRidingClone(clone);

    public void Kill() => implementation.Kill();

    public Recorder.Recording<PlayerRecorderFrame> GetRecording() => implementation.GetRecording();

    public void InstantiateClone(PlayerNode player) {
        if (implementation != null) {
            return;
        }

        implementation = new PlayerClone();
    }

    public void SetColor(PlayerColor.Value color) {
        this.implementation.SetColor(color);
    }

    public PlayerColor.Value GetColor() {
        return this.implementation.GetColor();
    }

    public bool WasRecordingDuringDeath {
        get {
            return this.implementation.WasRecordingDuringDeath;
        }
    }

    public Recorder.FrameRecorder<PlayerRecorderFrame> CloneRecorder {
        get => this.implementation.CloneRecorder;
    }
}

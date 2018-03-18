using Godot;
using System;
using System.Collections.Generic;

public class Entity : Node2D {
    public CollisionEngine collision;
    public Vector2 remainders;
    public Collider collider;
    public delegate void OnCollide();
    public Scene scene;

    public virtual bool IsRiding(Solid s) {
        return this.collision.CollideFirst<Solid>(
            this.GetPosition() + new Vector2(0, 1)
        ) == s;
    }

    public override void _Ready() {
        this.collider = (Collider)this.GetNode("Collider");
        this.remainders = new Vector2();

        Scene scene = (Scene)this.GetTree().GetRoot().GetNode("MainScene/Scene");
        this.scene = scene;
        this.scene.GetManager().Add(this);
        this.collision = new CollisionEngine(this, scene);
    }

    public override void _ExitTree() {
        this.scene.GetManager().Remove(this);
    }

    public virtual Collider GetCollider() => this.collider;

    public int CalcMoveX(float x) {
        this.remainders.x += x;
        int moveX = (int)Mathf.Round(this.remainders.x);
        if (moveX == 0) {
            return moveX;
        }
        this.remainders.x -= moveX;

        return moveX;
    }

    public int CalcMoveY(float y) {
        this.remainders.y += y;
        int moveY = (int)Mathf.Round(this.remainders.y);
        if (moveY == 0) {
            return moveY;
        }
        this.remainders.y -= moveY;

        return moveY;
    }

    public virtual void Move(Vector2 move, OnCollide onCollidH, OnCollide onCollideV) {
        this.MoveX(move.x, onCollidH);
        this.MoveY(move.y, onCollideV);
    }

    public virtual void MoveX(float x, OnCollide onCollide) {
        Vector2 pos = this.GetPosition();
        int moveX = this.CalcMoveX(x);
        int dirX = (int)Mathf.Sign(moveX);
        bool collided = false;
        if (x == 0 || moveX == 0) {
            return;
        }
        for(int i = 0; i < Mathf.Abs(moveX); i++) {
            if(!this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(dirX, 0))) {
                this.SetPosition(this.GetPosition() + new Vector2(dirX, 0));
            } else {
                collided = true;
            }
        }
        if(collided) {
            onCollide();
        }
    }

    public virtual void MoveY(float y, OnCollide onCollide) {
        Vector2 pos = this.GetPosition();
        int moveY = this.CalcMoveY(y);
        int dirY = (int)Mathf.Sign(moveY);
        bool collided = false;
        if (y == 0 || moveY == 0) {
            return;
        }
        for(int i = 0; i < Mathf.Abs(moveY); i++) {
            if(!this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(0, dirY))) {
                this.SetPosition(this.GetPosition() + new Vector2(0, dirY));
            } else {
                collided = true;
            }
        }

        if (collided) {
            onCollide();
        }
    }
}

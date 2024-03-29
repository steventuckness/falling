using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Entity : Node2D {
    public CollisionEngine collision;
    public Collider collider;
    public delegate void OnCollide();
    public Scene scene;
    public SubPixelFloat remainders;

    public bool IsCollidable {
        get {
            return this.collider.IsCollidable;
        }
        set {
            this.collider.IsCollidable = value;
        }
    }
    public T GetComponent<T>() where T : Component =>
        this.GetChildren().OfType<T>().FirstOrDefault(c => c is T);

    public virtual bool IsRiding(Solid s) {
        return this.collision.CollideFirst<Solid>(
            this.GetPosition() + new Vector2(0, 1)
        ) == s;
    }

    public override void _Ready() {
        this.collider = (Collider)this.GetNode("Collider");
        this.remainders = new SubPixelFloat();

        Scene scene = (Scene)this.GetTree().GetRoot().GetNode("Level/Scene");
        this.scene = scene;
        this.scene.GetManager().Add(this);
        this.collision = new CollisionEngine(this, scene);
    }

    public override void _ExitTree() {
        this.scene.GetManager().Remove(this);
    }

    public virtual Collider GetCollider() => this.collider;

    public virtual void Move(Vector2 move, OnCollide onCollidH, OnCollide onCollideV) {
        this.MoveX(move.x, onCollidH);
        this.MoveY(move.y, onCollideV);
    }

    public virtual void MoveX(float x, OnCollide onCollide) {
        Vector2 pos = this.GetPosition();
        int moveX = this.remainders.UpdateX(x);
        int dirX = (int)Mathf.Sign(moveX);
        bool collided = false;
        if (x == 0 || moveX == 0) {
            return;
        }
        for (int i = 0; i < Mathf.Abs(moveX); i++) {
            Vector2 check = this.GetPosition() + new Vector2(dirX, 0);

            if (!this.collision.CollideCheck<Solid>(check)) {
                this.SetPosition(this.GetPosition() + new Vector2(dirX, 0));
            }
            else {
                collided = true;
            }
        }
        if (collided && onCollide != null) {
            onCollide();
        }
    }

    public virtual void MoveY(float y, OnCollide onCollide) {
        Vector2 pos = this.GetPosition();
        int moveY = this.remainders.UpdateY(y);
        int dirY = (int)Mathf.Sign(moveY);
        bool collided = false;
        if (y == 0 || moveY == 0) {
            return;
        }
        for (int i = 0; i < Mathf.Abs(moveY); i++) {
            if (!this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector2(0, dirY))) {
                this.SetPosition(this.GetPosition() + new Vector2(0, dirY));
            }
            else {
                collided = true;
            }
        }

        if (collided && onCollide != null) {
            onCollide();
        }
    }
}

using Godot;
using System;
using Rect;

public class Collider : Node2D {
    public delegate Node2D FindOwner();
    private FindOwner findOwner;

    public override void _Ready() {
        this.findOwner = () => (Node2D) this.GetParent();
    }

    public void SetFindOwner(FindOwner f) {
        this.findOwner = f;
    }

    public virtual float Left() => 0.0f;
    public virtual float Right() => 0.0f;
    public virtual float Top() => 0.0f;
    public virtual float Bottom() => 0.0f;

    public bool Collides(Collider other) {
        if(other is BoxCollider) {
            return this.Collides(other as BoxCollider);
        } else if(other is GridCollider) {
            return this.Collides(other as GridCollider);
        }
        return false;
    }

    public virtual bool Collides(BoxCollider other) => false;
    public virtual bool Collides(GridCollider other) => false;

    public Node2D GetNodeOwner() => this.findOwner();

    public CollisionShape2D GetCollisionShape() =>
        (CollisionShape2D)this.GetNode("Shape");
}
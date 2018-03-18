using Godot;
using System;
using Rect;

public class BoxCollider : Collider {
    public RectangleShape2D GetRectangleShape() =>
        (RectangleShape2D)this.GetCollisionShape().GetShape();

    public Rect2 GetRect(Vector2 position) =>
        new Rect2(
            position - this.GetRectangleShape().GetExtents(),
            this.GetRectangleShape().GetExtents() * 2
        );

    public Rect2 GetRect() =>
        new Rect2(
            this.GetGlobalPosition() - this.GetRectangleShape().GetExtents(),
            this.GetRectangleShape().GetExtents() * 2
        );

    public override float Left() => this.GetRect().Left();
    public override float Right() => this.GetRect().Right();
    public override float Top() => this.GetRect().Top();
    public override float Bottom() => this.GetRect().Bottom();

    public override bool Collides(BoxCollider other) {
        Rect2 otherRect = other.GetRect();
        Rect2 thisRect = this.GetRect();
        return thisRect.Overlaps(otherRect);
    }

    public override bool Collides(GridCollider other) {
        // TODO: Handle collision within the tile grid
        return false;
    }
}

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
        return thisRect.Overlaps(otherRect) || otherRect.Overlaps(thisRect);
    }

    public override bool Collides(GridCollider other) {
        TileMap map = other.GetMap();
        Rect2 r = this.GetRect();
        // TODO: Remove this. This corrects a WEIRD issue where
        // accessing tiles at world coordinates seems to be OFF
        // by one unit on the right and bottom edges. So we will temporarily
        // and hackily grow the rect inwards to account for this.
        r = r.GrowIndividual(0, 0, -1, -1);
        Vector2[] points = r.Points();
        bool hit = false;
        foreach(Vector2 p in points) {
            int cell = map.GetCellv(map.WorldToMap(p));
            if(cell > -1) {
                hit = true;
                break;
            }
        }

        // if(hit) {
        //     GD.Print("Hit tile!");
        // } else {
        //     GD.Print("No hit");
        // }
        
        return hit;
    }
}

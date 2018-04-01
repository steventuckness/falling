using Godot;
using System;

public class DoorBase : Solid, IDoor {
    private bool isOpen = false;

    public bool CanOpen() {
        return !this.IsOpen();
    }

    public void Close() {
        this.isOpen = false;
        this.IsCollidable = true;
        this.Update();
    }

    public bool IsOpen() {
        return this.isOpen;
    }

    public void Open() {
        this.isOpen = true;
        this.IsCollidable = false;
        this.Update();
    }

    public override void _Ready() {
        base._Ready();
        this.isOpen = false;
    } 

    public override void _Draw() {
         
        Color c = this.IsOpen() ? new Color(0, 256, 0, 1) : new Color(256, 0, 0, 1);
        RectangleShape2D shape = (RectangleShape2D)((CollisionShape2D)this.collider.GetCollisionShape()).GetShape();
        Vector2 extents = shape.GetExtents();

        this.DrawRect(
            new Rect2(new Vector2(-extents.x, -extents.y), extents * 2), c
        ); 
    }
}

using Godot;
using System;

public class DoorBase : StaticBody2D, IDoor {
    private CollisionShape2D shape;
    private RectangleShape2D rectangle;

    private bool isOpen = false;

    private int originalCollisionMask;
    private int originalCollisionLayer;

    public bool CanOpen() {
        return !this.IsOpen();
    }

    public void Close() {
        this.isOpen = false;
        this.SetCollisionMask(this.originalCollisionMask);
        this.SetCollisionLayer(this.originalCollisionLayer);
        this.Update();
    }

    public bool IsOpen() {
        return this.isOpen;
    }

    public void Open() {
        this.isOpen = true;
        this.SetCollisionMask(0);
        this.SetCollisionLayer(0);
        this.Update();
    }

    public override void _Ready() {
        this.originalCollisionMask = this.GetCollisionMask();
        this.originalCollisionLayer = this.GetCollisionLayer();

        this.isOpen = false;
        this.shape = (CollisionShape2D) this.GetNode("CollisionShape2D");
        this.rectangle = (RectangleShape2D) this.shape.GetShape();
    }

    public override void _Draw() {
        Color c = this.IsOpen() ? new Color(0, 256, 0, 1) : new Color(256, 0, 0, 1);
        Vector2 extents = this.rectangle.GetExtents();

        this.DrawRect(
            new Rect2(new Vector2(-extents.x, -extents.y), extents * 2), c
        );
    }
}

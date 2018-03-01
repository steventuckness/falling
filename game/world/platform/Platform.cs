using Godot;
using System;

public class Platform : Node2D
{
    [Export]
    public float movementSpeed = 45; // pixels / per second
    
    private PathFollow2D follow;
    
    public override void _Ready()
    {
        if (GetParent() is PathFollow2D) {
            this.follow = (PathFollow2D)GetParent();   
        }   
    }

    public override void _Process(float delta)
    {
        if (this.follow != null) {
            this.follow.SetOffset(this.follow.GetOffset() + this.movementSpeed * delta);
        }
    }
}

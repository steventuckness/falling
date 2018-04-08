using Godot;
using System;

public class Component : Node2D {
    protected Entity entity;
    public bool isEnabled = true;
    
    public override void _EnterTree() {
        this.entity = (Entity) this.GetParent();
    }
    public override void _ExitTree() {
    }
}

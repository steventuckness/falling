using Godot;
using System;

public class Spike : Node2D
{
    private Area2D area;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        this.area = (Area2D) this.GetNode("Area2D");
    }

    public override void _PhysicsProcess(float delta) {
        object[] bodies = this.area.GetOverlappingBodies();
        // Query world to see if we have an overlap with someone.
        foreach(object body in bodies) {
            if(body is Player) {
                Player p = (Player) body;
                p.Kill();
            }
        }
    }
}

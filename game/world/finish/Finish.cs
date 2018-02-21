using Godot;
using System;

public class Finish : Node2D
{
    public static String SIGNAL_REACHED_FINISH = "Finish::playerReachedFinish";

    private Area2D area;
    
    public override void _Ready()
    {
        this.AddUserSignal(Finish.SIGNAL_REACHED_FINISH);
        
        this.area = (Area2D) this.GetNode("Area2D");
    }

     public override void _PhysicsProcess(float delta) {
        object[] bodies = this.area.GetOverlappingBodies();
        
        foreach(object body in bodies) {
            if(body is Player) {
                EmitSignal(Finish.SIGNAL_REACHED_FINISH);
            }
        }
    }
}

using Godot;
using System;

public class Finish : Node2D {
    public static String SIGNAL_REACHED_FINISH = "Finish::playerReachedFinish";

    private Area2D area;

    public override void _Ready() {
        this.AddUserSignal(Finish.SIGNAL_REACHED_FINISH);
        this.area = (Area2D)this.GetNode("Area2D");
    }

    private void _on_Area2D_area_entered(Godot.Object area) {
        PlayerNode player = PlayerNode.GetPlayerNodeFromChild(area);
        if (player != null) {
            EmitSignal(Finish.SIGNAL_REACHED_FINISH);
        }
    }
}

using Godot;
using System;

public class DeathArea : Area2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        this.Connect("area_entered", this, "onAreaEntered");
        
    }

    private void onAreaEntered(Godot.Object body)
    {
        PlayerNode player = PlayerNode.GetPlayerNodeFromChild(body);
        if (player != null) {
            player.Kill();
        }
    }
}




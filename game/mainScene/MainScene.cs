using Godot;
using System;

public class MainScene : Node2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        OS.SetWindowSize(new Vector2(1024, 768));
    }

   public override void _Process(float delta)
   {
       if(Input.IsActionPressed("key_restart")) {
           this.GetTree().ReloadCurrentScene();
       }
   }
}

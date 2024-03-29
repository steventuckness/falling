using Godot;
using System;

public class MainScene : Node2D {
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready() {
        // Called every time the node is added to the scene.
        // Initialization here
        OS.SetWindowSize(new Vector2(1024, 768));
    }

    public override void _Process(float delta) {
        // Configure custom signal the only way possible in C# right now.
        // https://github.com/godotengine/godot/issues/11956. (3.1 milestone) 
        if (GetNode("Player").HasUserSignal("playerDied") &&
         !GetNode("Player").IsConnected("playerDied", this, "died")) {
            GetNode("Player").Connect("playerDied", this, "died");
        }
    }

    public void died() {
        GD.Print("MainScene->playerDied signal recieved!");
    }
}

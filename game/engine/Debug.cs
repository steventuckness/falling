using Godot;
using System;

public class Debug : Node2D {
    private Act act;
    public Vector2 speed = new Vector2(500, 500);

    public override void _Ready() {
    }

    public Debug SetAct(Act act) {
        this.act = act;
        return this;
    }

    public override void _PhysicsProcess(float delta) {
        int xDir = (Input.IsActionPressed("key_right") ? 1 : 0) - (Input.IsActionPressed("key_left") ? 1 : 0);
        int yDir = (Input.IsActionPressed("key_down") ? 1 : 0) - (Input.IsActionPressed("key_up") ? 1 : 0);
        Vector2 dir = new Vector2(xDir, yDir);

        // Move the player around for testing purposes.
        if(Input.IsActionJustPressed("key_jump")) {
            this.act.GetPlayer().SetPosition(this.GetPosition());
        }

        this.SetPosition(
            this.GetPosition() + (dir * this.speed * delta)
        );
    }
}

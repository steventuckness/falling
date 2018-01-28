using System;
using Godot;

public class Walking : State<Player> {
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Walking:OnEnter()");
        GD.Print(owner.groundAcceleration);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Walking:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        Vector2 v = player.velocity;
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        a = a * dir;

        // If no input, we'll transition to the idle state
        if(dir == 0) {
            return player.stateIdle;
        }

        v = v + a;
        v[0] = Math.Min(v[0], player.groundMaxSpeed);
        player.velocity = v;

        player.MoveAndCollide(player.velocity * delta);
        return null;
    }
}

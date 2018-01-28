using System;
using Godot;

public class Idle : State<Player> {
    public override void OnEnter(float delta, Player player) {
        GD.Print("Idle:OnEnter()");
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Idle:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        Vector2 v = player.velocity;
        int vDir = Math.Sign(v[0]);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;        

        // Ground friction
        Vector2 f = new Vector2(-vDir * player.groundFriction * delta, 0);

        if(dir != 0) {
            return player.stateWalking;
        }
        // Apply friction
        v += f;

        // Switched directions, then stop
        if(Math.Sign(v[0]) != vDir) {
            v *= new Vector2(0, 1);
        }

        player.velocity = v;

        // Integrate
        player.MoveAndCollide(player.velocity * delta);
        return null;
    }
}

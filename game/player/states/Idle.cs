using System;
using Godot;

public class Idle : State<Player> {
    public override String GetName() { return "Idle"; }

    public override void OnEnter(float delta, Player player) {
        player.velocity = new Vector2(0, 0);
        player.PlayAnimation(Player.Animation.Idle);
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        bool isGrounded = player.IsOnFloor();
        Vector2 v = player.velocity;
        int vDir = Math.Sign(v[0]);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int isJump = Input.IsActionJustPressed("key_jump") ? 1 : 0;
        int dir = isRight - isLeft;

        if (!isGrounded) {
            return player.stateFalling;
        }

        if (isJump != 0) {
            return player.stateJumping;
        }

        // Ground friction
        Vector2 f = new Vector2(-vDir * player.groundFriction * delta, 0);

        if (dir != 0) {
            return player.stateWalking;
        }
        // Apply friction
        v += f;

        // Switched directions, then stop
        if (Math.Sign(v[0]) != vDir) {
            v *= new Vector2(0, 1);
        }

        player.velocity = v;
        player.ApplyGravity(delta, 200f);
        player.Move(25f);
        return null;
    }
}

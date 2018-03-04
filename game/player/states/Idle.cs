using System;
using Godot;

public class Idle : State<Player> {
    public override String GetName() { return "Idle"; }

    public override void OnEnter(float delta, Player player) {
        // player.velocity = new Vector2(0, 0);
        player.PlayAnimation(Player.Animation.Idle);
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        bool isGrounded = player.IsOnFloor();
        int isLeft = player.IsActionPressed("key_left") ? 1 : 0;
        int isRight = player.IsActionPressed("key_right") ? 1 : 0;
        int isJump = player.IsActionJustPressed("key_jump") ? 1 : 0;
        int dir = isRight - isLeft;

        if (!isGrounded) {
            return player.stateFalling;
        }
        if (isJump != 0) {
            return player.stateJumping;
        }
        if (dir != 0) {
            return player.stateWalking;
        }

        player.GroundControl(delta);
        player.ApplyGravity(delta);
        player.Move(0f);
        return null;
    }
}

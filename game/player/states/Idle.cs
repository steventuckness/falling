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
        bool isDown = player.IsActionPressed("key_down");
        int isLeft = player.IsActionPressed("key_left") ? 1 : 0;
        int isRight = player.IsActionPressed("key_right") ? 1 : 0;
        int isJump = player.IsActionJustPressed("key_jump") ? 1 : 0;
        int dir = isRight - isLeft;

        if (!isGrounded) {
            return player.stateFalling;
        }
        if (isJump != 0 && !isDown) {
            return player.stateJumping;
        }
        if (dir != 0) {
            return player.stateWalking;
        }
        // Allow fallthrough on platform?
        if (isJump != 0 && isDown && isGrounded) {
            player.fallThroughPlatform = true;
        }
        player.GroundControl(delta);
        player.ApplyGravity(delta);
        GD.Print("Vy: ", player.velocity.y);
        player.MoveX(player.velocity.x * delta, () => {
            player.velocity.x = 0;
        });
        player.MoveY(player.velocity.y * delta, () => {
            player.velocity.y = 0;
        });
        return null;
    }
}

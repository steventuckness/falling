using Godot;
using System;

public class Jumping : State<Player> {
    public override void OnEnter(float delta, Player player) {
        player.velocity = new Vector2(player.velocity.x, -Mathf.Sqrt(2 * player.gravity * player.jumpHeight));
        player.PlayAnimation(Player.Animation.Jumping);
        player.ResetWallJump();
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        int dir = player.GetInputDirection();
        if (player.velocity.y >= 0) {
            return player.stateFalling;
        }

        player.DetectDirectionChange();

        // Slow down the jump (variable jump height)
        if(player.IsActionJustReleased("key_jump")) {
            player.velocity.y *= 0.5f;
        }
        player.UpdateWallJump(delta);
        player.AirControl(delta);
        player.ApplyGravity(delta);

        if(player.CanWallJump() && Input.IsActionJustPressed("key_jump")) {
            GD.Print("JUMP!");
            player.ResetWallJump();
            GD.Print(player.wallJumpGraceTime);
            player.velocity = player.WallJump(dir);
        }

        player.MoveX(player.velocity.x * delta, () => {
            player.velocity.x = 0;
        });
        player.MoveY(player.velocity.y * delta, () => {
            player.velocity.y = 0;
        });

        return null;
    }
}

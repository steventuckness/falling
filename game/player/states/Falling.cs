using System;
using Godot;

public class Falling : State<Player> {
    private bool heldJumpWhileEntering = false;

    public override void OnEnter(float delta, Player player) {
        player.PlayAnimation(Player.Animation.Falling);
        this.heldJumpWhileEntering = player.IsActionPressed("key_jump");
        player.ResetWallJump();
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        int dir = player.GetInputDirection();
        player.DetectDirectionChange();
        if (player.IsOnFloor()) {
            return player.stateIdle;
        }
        player.UpdateWallJump(delta);
        player.AirControl(delta);
        player.ApplyGravity(delta);

        // Wall jump?
        if(player.CanWallJump() && Input.IsActionJustPressed("key_jump")) {
            player.ResetWallJump();
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

using System;
using Godot;

public class Falling : State<Player> {
    private bool heldJumpWhileEntering = false;
    private long timeWhenGlideLastPressed = 0; // milliseconds


    public override void OnEnter(float delta, Player owner) {
        owner.PlayAnimation(Player.Animation.Falling);
        this.heldJumpWhileEntering = owner.IsActionPressed("key_jump");
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        player.DetectDirectionChange();
        if (player.IsOnFloor()) {
            return player.stateIdle;
        }
        player.AirControl(delta);
        player.ApplyGravity(delta);
        player.MoveX(player.velocity.x * delta, () => {
            player.velocity.x = 0;
        });
        player.MoveY(player.velocity.y * delta, () => {
            player.velocity.y = 0;
        });
        return null;
    }
}

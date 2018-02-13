using Godot;
using System;

public class Jumping : State<Player> {
    public override void OnEnter(float delta, Player player) {
        player.velocity = new Vector2(player.velocity.x, -Mathf.Sqrt(2 * player.gravity * player.jumpHeight));
        player.PlayAnimation(Player.Animation.Jumping);
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        int dir = player.GetInputDirection();
        if (player.velocity.y >= 0) {
            return player.stateFalling;
        }
        if(Input.IsActionJustPressed("key_jump")) {
            return player.stateGliding;
        }

        player.DetectDirectionChange();

        // Slow down the jump (variable jump height)
        if(Input.IsActionJustReleased("key_jump")) {
            player.velocity.y *= 0.5f;
        }
        player.AirControl(delta);
        player.ApplyGravity(delta);
        player.Move(0f);

        return null;
    }
}

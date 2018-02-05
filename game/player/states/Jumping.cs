using Godot;
using System;

public class Jumping : State<Player>
{
    public override void OnEnter(float delta, Player player) {
        GD.Print("Jumping:OnEnter()");
        player.velocity = new Vector2(player.velocity.x, player.velocity.y - player.jumpSpeed);
        player.PlayAnimation(Player.Animation.Jumping);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Jumping:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {

        if (player.velocity.y >= 0) {
            return player.stateFalling;
        }

        player.DetectDirectionChange();
        // allow slightly higher jump
        /* if (Input.IsActionPressed("key_jump")) {
            // slow down gravity effect
            player.velocity.y -= 3.5f;
        } */
 
        // Allow to move left and right while jumping
        player.velocity = Acceleration.ApplyTerminalX(
            player.groundMaxSpeed, 
            player.groundAcceleration * player.GetDirectionMultiplier(), 
            delta, 
            player.velocity
        );
        player.ApplyGravity(delta, 1000.0f);
        player.Move(0f);

        return null;
    }
}

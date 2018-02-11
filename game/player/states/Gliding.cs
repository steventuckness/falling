using System;
using Godot;

public class Gliding : State<Player> {

    bool isBumping = false;
    float startYVelocity = 0.0f;

    public override void OnEnter(float delta, Player owner) {
        isBumping = true;
        startYVelocity = owner.velocity.y;
        owner.PlayAnimation(Player.Animation.Gliding);
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        if(!Input.IsActionPressed("key_jump")) {
            player.velocity = new Vector2(0,0);
            return player.stateFalling;
        }
        player.DetectDirectionChange();
        this.CalculateVerticalVelocity(delta, player);
        this.CalculateHorizontalVelocity(delta, player);
        player.MoveAndSlide(player.velocity);

        if (player.IsOnFloor()) {
            return player.stateIdle;
        }
        return null;
    }

    private void CalculateHorizontalVelocity(float delta, Player player) {
        player.velocity = 
            Acceleration.ApplyTerminalX(
                player.glideMaxSpeed.x,
                player.glideHorizontalAcceleration * player.GetDirectionMultiplier(),
                delta,
                player.velocity
            );
    }

    private void CalculateVerticalVelocity(float delta, Player player) {
        if (isBumping) {
            GD.Print("bumping!");
            player.velocity = Acceleration.ApplyY(player.glideBumpAcceleration * startYVelocity, delta, player.velocity);
            if (Math.Abs(player.velocity.y) > player.glideTopBumpVelocity) {
                isBumping = false;
            } 
        } else {
            player.velocity = GetGlideVelocityWithGravity(player, delta);
        }
        
    }

    private Vector2 GetGlideVelocityWithGravity(Player player, float delta) {
        return Acceleration.ApplyTerminalY(
                    player.glideMaxSpeed.y, 
                    player.glideGravity, 
                    delta, 
                    player.velocity
                );
    }
}

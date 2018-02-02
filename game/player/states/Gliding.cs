using System;
using Godot;

public class Gliding : State<Player> {

    bool isBumping = false;
    float bumpTime = 0.0f;
    float bumpAcceleration = 0.0f;

    public override void OnEnter(float delta, Player owner) {
        GD.Print("Gliding:OnEnter()");
        isBumping = true;
        bumpTime = 0.0f;
        bumpAcceleration = owner.glideBumpFactor * owner.velocity.y;
        owner.PlayAnimation(Player.Animation.Gliding);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Gliding:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        if(!Input.IsActionPressed("key_jump")) {
            player.velocity = new Vector2(0,0);
            return player.stateFalling;
        }
        player.DetectDirectionChange();
        this.CalculateVerticalVelocity(delta, player);
        this.CalculateHorizontalVelocity(delta, player);
        KinematicCollision2D collision = player.MoveAndCollide(player.velocity);

        if (collision != null) {
            GD.Print("Wall hit");
            player.velocity = new Vector2(0,0);
            return player.stateFalling;
        }
        return null;
    }

    private void CalculateHorizontalVelocity(float delta, Player player) {
        switch (player.direction) {
            case Player.Direction.Right:
                player.velocity =
                    Acceleration.ApplyTerminalX(
                        player.glideMaxSpeed.x,
                        player.glideHorizontalAcceleration,
                        delta,
                        player.velocity
                    );
                break;
            case Player.Direction.Left:
                player.velocity =
                    Acceleration.ApplyTerminalX(
                        player.glideMaxSpeed.x,
                        player.glideHorizontalAcceleration * -1,
                        delta,
                        player.velocity
                    );
                break;
        }
    }

    private void CalculateVerticalVelocity(float delta, Player player) {
        if (isBumping) {
            player.velocity = 
                Acceleration.ApplyY(bumpAcceleration, delta, player.velocity);
            bumpTime += delta;
            if (bumpTime > player.glideBumpTime) {
                isBumping = false;
            }
        } else {
            player.velocity = 
                Acceleration.ApplyTerminalY(
                    player.glideMaxSpeed.y, 
                    player.glideGravity, 
                    delta, 
                    player.velocity
                );
        }
    }
}

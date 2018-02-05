using System;
using Godot;

public class Falling : State<Player> {
    
    private float groundImpactSpeed = 0.0f; // px / sec
    
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Falling:OnEnter()");
        owner.PlayAnimation(Player.Animation.Falling);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Falling:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {        
        player.DetectDirectionChange();
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        player.velocity = player.velocity + (a * dir);
        player.velocity.x = Math.Min(player.velocity.x, player.groundMaxSpeed);
        player.ApplyGravity(delta, player.fallingMaxSpeed);
        player.Move(25.0f);
        bool isGrounded = player.IsOnFloor();

        if (!isGrounded) {
            groundImpactSpeed = player.velocity.y;
        }

        if (groundImpactSpeed >= player.impactDeadSpeed && isGrounded) {
            return player.stateDead; 
        }

        if (isGrounded) {
            return player.stateIdle;
        }

        if (Input.IsActionPressed("key_jump")) {
            return player.stateGliding;
        }

        return null;
    }
}

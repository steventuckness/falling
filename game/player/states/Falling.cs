using System;
using Godot;

public class Falling : State<Player> {
    
    private float groundImpactSpeed = 0.0f; // px / sec
    private bool heldJumpWhileEntering = false;
    
    public override void OnEnter(float delta, Player owner) {
        owner.PlayAnimation(Player.Animation.Falling);
        this.heldJumpWhileEntering = Input.IsActionPressed("key_jump");
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {        
        player.DetectDirectionChange();
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

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

        if (Input.IsActionJustPressed("key_jump")) {
            return player.stateGliding;
        }
        player.AirControl(delta);
        player.ApplyGravity(delta, player.fallingMaxSpeed);
        player.Move(0f);

        return null;
    }
}

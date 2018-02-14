using System;
using Godot;

public class Falling : State<Player> {
    
    private float groundImpactSpeed = 0.0f; // px / sec
    private bool heldJumpWhileEntering = false;
    private long timeWhenGlideLastPressed = 0; // milliseconds

    
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

        State<Player> landedState = player.DetectDeathByFalling();
        if (landedState != null) {
            return landedState;
        }

        if (Input.IsActionJustPressed("key_jump") && (OS.GetTicksMsec() - this.timeWhenGlideLastPressed >= player.glideAgainWaitTime)) {
            this.timeWhenGlideLastPressed = OS.GetTicksMsec();
            return player.stateGliding;
        }
        player.AirControl(delta);
        player.ApplyGravity(delta);
        player.Move(0f);

        return null;
    }
}

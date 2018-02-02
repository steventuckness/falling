using System;
using Godot;

public class Falling : State<Player> {
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Falling:OnEnter()");
        owner.PlayAnimation(Player.Animation.Walking);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Falling:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        bool isGrounded = player.IsOnFloor();
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        if(isGrounded) {
            return player.stateIdle;
        }

        player.velocity = player.velocity + (a * dir);
        player.velocity.x = Math.Min(player.velocity.x, player.groundMaxSpeed);
        player.applyGravity(delta, 400.0f);
        player.Move(25.0f);

        if (Input.IsActionPressed("key_jump")) {
            return player.stateGliding;
        }
        if (player.velocity.y >= 400.0f) {
            GD.Print("player dead");
        }

        return null;
    }
}

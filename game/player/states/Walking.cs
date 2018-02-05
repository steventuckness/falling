using System;
using Godot;

public class Walking : State<Player> {
    public override void OnEnter(float delta, Player player) {
        GD.Print("Walking:OnEnter()");
        player.velocity *= new Vector2(1, 0);
        player.PlayAnimation(Player.Animation.Walking);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Walking:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        player.DetectDirectionChange();
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        bool isGrounded = player.IsOnFloor();
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        if(dir == 0) {
            return player.stateIdle;
        }
        if(isGrounded && Input.IsActionJustPressed("key_jump")) {
            return player.stateJumping;
        }

        player.velocity = player.velocity + (a * dir);
        player.velocity.x = Math.Min(player.velocity.x, player.groundMaxSpeed);
        player.ApplyGravity(delta, 200f);
        player.Move(0f);
        return null;
    }
}

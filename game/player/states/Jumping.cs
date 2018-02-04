using Godot;
using System;

public class Jumping : State<Player>
{
    public override void OnEnter(float delta, Player player) {
        GD.Print("Jumping:OnEnter()");
        player.velocity = new Vector2(player.velocity.x, player.velocity.y - player.jumpSpeed);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Jumping:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        // TODO: dry up. Copy and pasted alot of code from walking state. Refactor 
        bool isGrounded = player.collision.isOnGround;
        
        Vector2 v = player.velocity;
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);

        if (v.y >= 0) {
            return player.stateFalling;
        }

        int vDir = Math.Sign(v[0]);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        // Apply acceleration
        a = a * dir;
        v += a;
    
        // Allow to move left and right while jumping
        player.velocity.x = Math.Min(v.x, player.groundMaxSpeed);
        player.applyGravity(delta, 200.0f);
        player.Move(0f);

        return null;
    }
}

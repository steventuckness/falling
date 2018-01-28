using System;
using Godot;

public class Walking : State<Player> {
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Walking:OnEnter()");
        GD.Print(owner.groundAcceleration);
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Walking:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        bool isGrounded = player.collision.isOnGround;
        Vector2 v = player.velocity;
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        a = a * dir;

        // If no input, we'll transition to the idle state
        if(dir == 0) {
            return player.stateIdle;
        }
        if(isGrounded && Input.IsActionJustPressed("key_jump")) {
            return player.stateGliding;
        }
        // Apply acceleration
        v = v + a;
        // Max speed
        v[0] = Math.Min(v[0], player.groundMaxSpeed);

        // TODO: Going to need to get a lot smarter about handling the collition
        // For example, we'll need a floor check to see if we need to enter
        // the falling state.
        KinematicCollision2D coll = player.MoveAndCollide(v * delta);

        // TODO: This is very primitive.
        // We should really be checking the collision to see
        // if it's a collision tile
        if(coll != null) {
            // We've gone directly against a wall
            if(Math.Abs(coll.Normal.x) == 1) {
                v *= new Vector2(0, 1);
            }
        }

        player.velocity = v;
        
        return null;
    }
}

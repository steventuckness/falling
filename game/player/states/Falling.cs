using System;
using Godot;

public class Falling : State<Player> {
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Falling:OnEnter()");
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Falling:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        bool isGrounded = player.IsOnFloor();

        if(isGrounded) {
            return player.stateIdle;
        }

        player.applyGravity(delta, 400.0f);
        player.Move(25.0f);

        if (player.velocity.y >= 400.0f) {
            GD.Print("player dead");
        }

        return null;
    }
}

using System;
using Godot;

public class Knockback : State<Player> {
    public override void OnEnter(float delta, Player player) {
        player.PlayAnimation(Player.Animation.Walking);
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        player.ApplyGravity(delta);
        player.Move();
        if(player.IsOnFloor()) {
            return player.stateIdle;
        }
        return null;
    }

}

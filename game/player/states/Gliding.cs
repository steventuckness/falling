using System;
using Godot;

public class Gliding : State<Player> {
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Gliding:OnEnter()");
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Gliding:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        if(!Input.IsActionPressed("key_jump")) {
            return player.stateFalling;
        }
        return null;
    }
}

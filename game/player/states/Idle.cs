using System;
using Godot;

public class Idle : State<Player> {
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Idle:OnEnter()");
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Idle:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        if(Input.IsActionJustPressed("key_jump")) {
            return player.stateGliding;
        }
        return null;
    }
}

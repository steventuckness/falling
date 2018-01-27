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
        return null;
    }
}

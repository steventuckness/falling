using Godot;
using System;

public class Dead : State<Player>
{              
    public override void OnEnter(float delta, Player owner) {
        GD.Print("Dead:OnEnter()");
        owner.EmitSignal("playerDied");
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Dead:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        return null;
    }
}

using Godot;
using System;


public class Dead : State<Player> {
    public override String GetName() { return "Dead"; }
    public override void OnEnter(float delta, Player player) {
        GD.Print("Dead:OnEnter()");
        player.Hide();
        player.EmitSignal("playerDied");
    }

    public override void OnExit(float delta, Player player) {
        GD.Print("Dead:OnExit()");
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        if (timeInState > player.respawnTime) {
            return player.stateRespawn;
        }
        return null;
    }
}

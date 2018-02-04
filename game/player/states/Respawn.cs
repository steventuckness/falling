using Godot;
using System;

public class Respawn : State<Player> {
    public override String GetName() { return "Respawn"; }
    public override void OnEnter(float delta, Player player) {
        GD.Print("Respawn:OnEnter()");
        player.Show();
    }

    public override void OnExit(float delta, Player owner) {
        GD.Print("Respawn:OnExit()");
    }
    
    public override State<Player> Update(float delta, Player player, float timeInState) {
        return null;
    }
}

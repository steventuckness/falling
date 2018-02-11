using Godot;
using System;

public class Respawn : State<Player> {
    public override String GetName() { return "Respawn"; }
    public override void OnEnter(float delta, Player player) {
        player.Show();
    }

    public override void OnExit(float delta, Player owner) {
    }
    
    public override State<Player> Update(float delta, Player player, float timeInState) {
        return null;
    }
}

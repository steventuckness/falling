using Godot;
using System;

public class Act : Node {
    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Act> sm = new StateMachine<Act>();
    private Play statePlay = new Play();
    private ActRespawn stateRespawn = new ActRespawn();

    // Nodes ///////////////////////////////////////////////////////////////////
    private Node2D spawn;
    private Player player;
    private Node2D finish;

    public override void _Ready() {
        // Get nodes
        this.spawn = (Node2D) this.GetNode("Spawn");
        this.player = (Player) this.GetNode("Player");
        this.finish = (Node2D) this.GetNode("Finish");

        // Wire events
        if(this.player.HasUserSignal(Player.SIGNAL_DIED)) {
            this.player.Connect(Player.SIGNAL_DIED, this, "PlayerDied");
        }

        this.sm.Init(this.statePlay);
    }

    public void PlayerDied() {
        GD.Print("Act -> Received signal of player death");
        // TODO: Make the Act actually handle respawn
        this.sm.TransitionState(this.stateRespawn);
    }

    public override void _PhysicsProcess(float delta) {
        this.sm.Update(delta, this);
    }
}

using Godot;
using System;

public class Act : Node {
    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Act> sm = new StateMachine<Act>();
    public Play statePlay = new Play();
    public ActRespawn stateRespawn = new ActRespawn();

    // Nodes ///////////////////////////////////////////////////////////////////
    private Node2D spawn;
    private Player player;
    private Node2D finish;
    private Node2D debug;

    // Exports /////////////////////////////////////////////////////////////////
    [Export]
    public float cameraSpeed = 5f;
    [Export]
    public float respawnTime = 2f;

    public Node2D GetSpawn() {
        return this.spawn;
    }

    public Player GetPlayer() {
        return this.player;
    }

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

    public Node2D CreateDebug() {
        PackedScene scene = (PackedScene) ResourceLoader.Load("res://engine/Debug.tscn");
        Debug debugNode = (Debug) scene.Instance();
        debugNode.SetAct(this);

        return debugNode;
    }

    public void PlayerDied() {
        GD.Print("Act -> Received signal of player death");
        // TODO: Make the Act actually handle respawn
        this.sm.TransitionState(this.stateRespawn);
    }

    public override void _PhysicsProcess(float delta) {
        if(Input.IsActionJustPressed("key_restart")) {
            this.GetTree().ReloadCurrentScene();
        }

        if(Input.IsActionJustPressed("key_debug_toggle")) {
            if(this.debug != null) {
                Camera2D cam = (Camera2D) this.player.GetNode("Camera2D");
                cam.MakeCurrent();
                this.RemoveChild(this.debug);
                this.debug = null;
                this.player.paused = false;
            } else {
                this.debug = this.CreateDebug();
                this.debug.SetPosition(this.player.GetPosition());
                Camera2D cam = (Camera2D) this.debug.GetNode("Camera2D");
                this.AddChild(debug);
                cam.MakeCurrent();
                this.player.paused = true;
            }
        }

        this.sm.Update(delta, this);
    }
    private void FinishEntered(Godot.Object area)
    {
        GD.Print("Finish triggered");
        ((ActManager)this.GetNode("/root/ActManager")).NextAct();
    }
}



using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Act : Node {
    private long startTime;
    private int clonesCreated = 0;
    const String PHASED_OBJECT = "PhasedObject";

    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Act> sm = new StateMachine<Act>();
    public Play statePlay = new Play();
    public ActRespawn stateRespawn = new ActRespawn();

    // Nodes ///////////////////////////////////////////////////////////////////
    private Node2D spawn;
    private PlayerNode player;
    private Node2D finish;
    private FinishOverlay finishOverlay;
    private Node2D debug;
    private Cam cam;


    // Exports /////////////////////////////////////////////////////////////////
    [Export]
    public int colors = 1;

    [Export]
    public float cameraSpeed = 5f;
    [Export]
    public float respawnTime = 2f;

    // Exporting arrays in C# is broken right now: https://github.com/godotengine/godot/issues/17684
    //[Export] 
    //public int[] cloneColors = new int[] { 0 /*red*/, 1 /*green*/ };

    public Node2D GetSpawn() {
        return this.spawn;
    }

    public PlayerNode GetPlayer() {
        return this.player;
    }

    private void ConnectEvents() {
        this.player.Connect(Player.SIGNAL_RECORDING_STOPPED, this, "RecordingStopped");
        this.player.Connect(Player.SIGNAL_RECORDING_STARTED, this, "RecordingStarted");

        if (this.player.HasUserSignal(Player.SIGNAL_DIED)) {
            this.player.Connect(Player.SIGNAL_DIED, this, "PlayerDied");
        }

        if (this.finish.HasUserSignal(Finish.SIGNAL_REACHED_FINISH)) {
            this.finish.Connect(Finish.SIGNAL_REACHED_FINISH, this, "PlayerReachedFinish");
        }

        if (this.finishOverlay.HasUserSignal(FinishOverlay.SIGNAL_GO_TO_NEXT_LEVEL)) {
            this.finishOverlay.Connect(FinishOverlay.SIGNAL_GO_TO_NEXT_LEVEL, this, "NextLevel");
        }

        // Grab all the cam-locks
        object[] camLocks = this.GetNode("CamLocks").GetChildren();
        foreach(CamLock camLock in camLocks) {
            camLock.Connect(CamLock.PLAYER_ENTERED, this, "OnCamLimitEnter");
            camLock.Connect(CamLock.PLAYER_EXITED, this, "OnCamLimitExit");
        }
    }

    public override void _Ready() {
        this.startTime = OS.GetTicksMsec();
        
        // Get nodes
        this.spawn = (Node2D)this.GetNode("Spawn");
        this.player = (PlayerNode)this.GetNode("Player");
        this.finish = (Node2D)this.GetNode("Finish");
        this.finishOverlay = (FinishOverlay)this.GetNode("FinishOverlay");
        this.cam = (Cam)this.GetNode("Cam");

        this.cam.Follow(this.player);
        this.ConnectEvents();
        this.sm.Init(this.statePlay);

        this.ConfigureCloneOptions(this.GetActColors());
        this.GetPlayer().SetColor(this.player.implementation.GetCloneOptions()[0].GetColor()); 
    }

    public void OnCamLimitEnter(CamLock camLock) {
        // Use the camera lock that we entered to adjust
        // the limits of the camera
        this.cam.SetLimits(
            camLock.LockCamera(this.cam)
        );
    }

    public void OnCamLimitExit(CamLock camLock) {
        if(camLock.ShouldUnlockOnExit()) {
            this.cam.UnsetLimits();
        }
    }

    public Node2D CreateDebug() {
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://engine/Debug.tscn");
        Debug debugNode = (Debug)scene.Instance();
        debugNode.SetAct(this);

        return debugNode;
    }

    public void PlayerDied() {
        GD.Print("Act -> Received signal of player death");

        if (player.WasRecordingDuringDeath) {
            GD.Print("player was recording");

            // just respawn immediately for now except don't kill all of the clones and don't delay...
            Node2D spawn = this.GetSpawn();
            Vector2 spawnPosition = spawn.GetPosition();
            this.player.Show();
            this.player.SetPosition(spawnPosition);
            this.player.Respawn();
        } else {
            GD.Print("player was not recording");
            this.KillAllClones();
            
            // TODO: Make the Act actually handle respawn
            this.sm.TransitionState(this.stateRespawn);
        }
    }

    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionJustPressed("key_restart")) {
            this.GetTree().ReloadCurrentScene();
        }

        if (Input.IsActionJustPressed("key_debug_toggle")) {
            if (this.debug != null) {
                Camera2D cam = (Camera2D)this.player.GetNode("Camera2D");
                cam.MakeCurrent();
                this.RemoveChild(this.debug);
                this.debug = null;
                this.player.paused = false;
            }
            else {
                this.debug = this.CreateDebug();
                this.debug.SetPosition(this.player.GetPosition());
                Camera2D cam = (Camera2D)this.debug.GetNode("Camera2D");
                this.AddChild(debug);
                cam.MakeCurrent();
                this.player.paused = true;
            }
        }

        this.sm.Update(delta, this);
    }
    private void PlayerReachedFinish()
    {
        long timeElapsed = OS.GetTicksMsec() - this.startTime;
        this.finish.SetProcess(false);
        this.finishOverlay.ShowOverlay(timeElapsed, clonesCreated);
        GetTree().SetPause(true);
    }

    private void NextLevel() {
         ((ActManager)this.GetNode("/root/ActManager")).NextAct();
    }

    private void RecordingStarted() {
        SetPhasedObjectsVisibility(true);
    }

    private void RecordingStopped() {
        GD.Print("Recording stopped at the act");
        this.SetPhasedObjectsVisibility(false);
        this.CreateClone();
    }

    private void CreateClone() {
        this.clonesCreated++; 
        PlayerNode node = (PlayerNode)
             ((PackedScene)ResourceLoader.Load("res://player/player.tscn"))
                 .Instance();

        // instantiate clone in PlayerNode class to avoid losing the reference 
        node.InstantiateClone(this.GetPlayer());
        ((PlayerClone)node.implementation).recording = player.GetRecording();
        node.SetColor(player.GetColor());
       
        // loop through children and remove children that have the new clone color
        var clones = this.GetClones();
        GD.Print($"clones count : {clones.Length}");

        foreach(var existingClone in clones) {
            if (((PlayerNode)existingClone).GetColor() == node.GetColor()) {
                ((PlayerNode)existingClone).QueueFree();
            }
        }

        node.AddToGroup("clones");
        this.AddChild(node);
        GD.Print("Clone created!");
    }

    private object[] GetClones() {
        return this.GetTree().GetNodesInGroup("clones");
    }

    public void KillAllClones() {
        GD.Print("attempting to kill all clones");
        var clones = this.GetClones();
        foreach(var clone in clones) {
            ((PlayerNode)clone).QueueFree();
        }
    }

    private void ConfigureCloneOptions(int[] cloneColors) {   
        var levelCloneOptions = new CloneOptions.ECloneOption[cloneColors.Length];
    
        for (int i = 0; i < cloneColors.Length; i++) {
            levelCloneOptions[i] = (CloneOptions.ECloneOption)cloneColors[i];
        } 

        this.player.implementation.SetCloneOptions(levelCloneOptions); 
    }

    public void SetPhasedObjectsVisibility(bool isVisible) {
        var nodes = GetTree().GetNodesInGroup(PHASED_OBJECT);
        foreach (Node node in nodes) {
            if (node is Entity) {
                ((Entity)node).collider.IsCollidable = isVisible;
            }
            else if (node is Node2D) {
                ((Node2D)node).SetVisible(isVisible);
            } else {
                GD.Print("WARNING: A node that is not a Node2D was assigned to PhasedObjects.");
            }
        }
    }
    
    private int[] GetActColors() => Enumerable.Range(1, this.color).ToArray();
}



using Godot;
using System;

public class Act : Node {
    // State ///////////////////////////////////////////////////////////////////
    private StateMachine<Act> sm = new StateMachine<Act>();
    public Play statePlay = new Play();
    public ActRespawn stateRespawn = new ActRespawn();

    // Nodes ///////////////////////////////////////////////////////////////////
    private Node2D spawn;
    private PlayerNode player;
    private Node2D finish;
    private Node2D finishOverlay;
    private Node2D debug;
    private Cam cam;

    private long startTime;

    // Exports /////////////////////////////////////////////////////////////////
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
        this.player.Connect(Player.SIGNAL_CREATE_CLONE, this, "CreateClone");

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
        this.finishOverlay = (Node2D)this.GetNode("FinishOverlay");
        this.cam = (Cam)this.GetNode("Cam");

        this.cam.Follow(this.player);
        this.ConnectEvents();
        this.sm.Init(this.statePlay);

        this.ConfigureCloneOptions(this.GetActColors(GetTree().GetCurrentScene().GetName()));
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
        // TODO: Make the Act actually handle respawn
        this.sm.TransitionState(this.stateRespawn);
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
        this.finishOverlay.Call("ShowOverlay", timeElapsed);
        GetTree().SetPause(true);
    }

    private void NextLevel() {
         ((ActManager)this.GetNode("/root/ActManager")).NextAct();
    }

    private void CreateClone() {
        PlayerNode node = (PlayerNode)
             ((PackedScene)ResourceLoader.Load("res://player/player.tscn"))
                 .Instance();
        PlayerClone clone = new PlayerClone();
        clone.recording = this.GetPlayer().GetRecording();
        var playerSprite = (Sprite)this.GetPlayer().GetNode("Sprite");
        clone.color = playerSprite.GetModulate();
        node.implementation = clone;
        this.AddChild(node);
        GD.Print("Clone created!");
    }

    private void ConfigureCloneOptions(int[] cloneColors) {   
        var levelCloneOptions = new CloneOptions.ECloneOption[cloneColors.Length];
    
        for (int i = 0; i < cloneColors.Length; i++) {
            levelCloneOptions[i] = (CloneOptions.ECloneOption)cloneColors[i];
        } 

        this.player.implementation.SetCloneOptions(levelCloneOptions); 
    }
    
    private int[] GetActColors(string sceneName) {
        int[] actColors;
        
        switch(sceneName) {
            case "MainScene":
                actColors = new int[] { 0 /*red*/, 1 /*green*/ };
                break;
            default:
                actColors = new int[] { 0, 1, 2 };
                break;
        }

        return actColors;
    }
}



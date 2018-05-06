using Godot;
using System;

public class CamLock : Node2D {
    // Signals /////////////////////////////////////////////////////////////////
    public static String PLAYER_ENTERED = "CamLock::player::entered";
    public static String PLAYER_EXITED = "CamLock::player::exited";

    // Locking /////////////////////////////////////////////////////////////////
    public struct CamLockFlags {
        public bool xMin;
        public bool xMax;
        public bool yMin;
        public bool yMax;
    }

    [Export]
    public bool xMin = true;
    [Export]
    public bool xMax = true;
    [Export]
    public bool yMin = true;
    [Export]
    public bool yMax = true;
    [Export]
    public bool unLockOnExit = false;

    private CamLockFlags flags = new CamLockFlags();

    public struct CamLockLimits {
        public Vector2 min;
        public Vector2 max;
    }

    private Area2D detect;
    private ReferenceRect bounds;
    private CamLockLimits limits = new CamLockLimits();

    // Using the polygon contained within the CamLock, determines
    // the minimum and maximum [x, y] values in order to create
    // a rectangle that clamps the camera.
    private CamLockLimits FindLimits() {
        CamLockLimits lim = new CamLockLimits();
        Rect2 b = this.bounds.GetRect();

        lim.min.x = b.Position.x;
        lim.min.y = b.Position.y;

        lim.max.x = b.Position.x + b.Size.x;
        lim.max.y = b.Position.y + b.Size.y;

        return lim;
    }

    public override void _Ready() {
        this.detect = this.GetNode("Detect") as Area2D;
        this.bounds = this.GetNode("Bounds") as ReferenceRect;

        this.AddUserSignal(PLAYER_ENTERED);
        this.AddUserSignal(PLAYER_EXITED);

        this.detect.Connect("body_entered", this, "OnEnter");
        this.detect.Connect("body_exited", this, "OnExit");
        this.detect.Connect("area_entered", this, "OnEnter");
        this.detect.Connect("area_exited", this, "OnExit");

        this.limits = this.FindLimits();

        // For convenience, convert the editor exported values to our struct
        this.flags.xMin = this.xMin;
        this.flags.xMax = this.xMax;
        this.flags.yMin = this.yMin;
        this.flags.yMax = this.yMax;
    }

    // Using the limits defined on this instance of CamLock, applies the
    // limits to the provided Cam object, depending on the switch flags
    // that can be set via the Godot editor.
    public Cam.CamLimits LockCamera(Cam cam) {
        Cam.CamLimits limits = cam.GetLimits();
        // X
        if (this.flags.xMin) {
            limits.min.x = this.limits.min.x;
        }
        if (this.flags.xMax) {
            limits.max.x = this.limits.max.x;
        }
        // Y
        if (this.flags.yMin) {
            limits.min.y = this.limits.min.y;
        }
        if (this.flags.yMax) {
            limits.max.y = this.limits.max.y;
        }
        return limits;
    }

    public bool ShouldUnlockOnExit() {
        return this.unLockOnExit;
    }

    public void OnEnter(Godot.Object obj) {
        if (obj is Collider) {
            Entity ent = ((Collider)obj).GetNodeOwner();
            if (ent is PlayerNode) {
                this.EmitSignal(PLAYER_ENTERED, new object[] { this });
            }
        }
    }

    public void OnExit(Godot.Object obj) {
        if (obj is Collider) {
            Entity ent = ((Collider)obj).GetNodeOwner();
            if (ent is PlayerNode) {
                this.EmitSignal(PLAYER_EXITED, new object[] { this });
            }
        }
    }
}

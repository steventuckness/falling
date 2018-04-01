using Godot;
using System;

public class SwitchBase : Node2D, ISwitch {
    private Area2D area2d;
    private bool isPressed = false;
    private bool wasPressedLastFrame = false;

    // TODO: Extract this out
    public static String SIGNAL_SWITCH_ON = "Switch::on";
    public static String SIGNAL_SWITCH_OFF = "Switch::off";

    private int playersOnSwitch = 0;

    public override void _Ready() {
        this.area2d = (Area2D) this.GetNode("Area2D");

        // Signals
        this.AddUserSignal(SIGNAL_SWITCH_ON);
        this.AddUserSignal(SIGNAL_SWITCH_OFF);

        this.area2d.Connect("area_entered", this, "onAreaEntered");
        this.area2d.Connect("area_exited", this, "onAreaExited");
    }

    private void onAreaEntered(Godot.Object body) {
        PlayerNode player = PlayerNode.GetPlayerNodeFromChild(body);
        if (player != null) {
            this.playersOnSwitch++;
        }
    }

    private void onAreaExited(Godot.Object body) {
        PlayerNode player = PlayerNode.GetPlayerNodeFromChild(body);
        if (player != null) {
           this.playersOnSwitch--;
        }
    }

    public override void _PhysicsProcess(float delta) {
        // Allow sub-classes to override the implementation
        this.Process(delta);
    }

    public virtual void Process(float delta) {                
        if(this.IsBeingPressed() && this.CanTurnOn()) {
            this.TurnOn();
        } else if (!this.IsBeingPressed() && !this.CanTurnOn()) {
            this.TurnOff();
        } 
    }

    public bool WasPressedLastFrame() {
        return this.wasPressedLastFrame;
    }

    public bool UpdateWasPressedLastFrame() {
        if(this.IsBeingPressed()) {
            this.wasPressedLastFrame = true;
        } else {
            this.wasPressedLastFrame = false;
        }

        return this.wasPressedLastFrame;
    }

    public bool IsBeingPressed() {
        return this.playersOnSwitch > 0;
    }

    public void TurnOn() {
        this.isPressed = true;
        this.EmitSignal(SIGNAL_SWITCH_ON, new object[]{ this });
        this.Update();
    }

    public void TurnOff() {
        this.isPressed = false;
        this.EmitSignal(SIGNAL_SWITCH_OFF, new object[]{ this });
        this.Update();
    }

    public bool IsOn() {
        return this.isPressed;
    }

    public bool CanTurnOn() {
        return !this.IsOn();
    }

    public bool Output() {
        return this.IsOn();
    }

    public override void _Draw() {
        Color c = this.IsOn() ? new Color(0, 256, 0, 1) : new Color(256, 0, 0, 1);
        this.DrawRect(new Rect2(new Vector2(-8, -8), new Vector2(16, 16)), c);
    }
}

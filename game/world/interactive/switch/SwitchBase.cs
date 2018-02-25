using Godot;
using System;

public class SwitchBase : Node2D, ISwitch {
    private RayCast2D cast;
    private bool isPressed = false;
    private bool wasPressedLastFrame = false;

    // TODO: Extract this out
    public static String SIGNAL_SWITCH_ON = "Switch::on";
    public static String SIGNAL_SWITCH_OFF = "Switch::off";

    public override void _Ready() {
        this.cast = (RayCast2D) this.GetNode("RayCast2D");

        // Signals
        this.AddUserSignal(SIGNAL_SWITCH_ON);
        this.AddUserSignal(SIGNAL_SWITCH_OFF);
    }

    public override void _PhysicsProcess(float delta) {
        // Allow sub-classes to override the implementation
        this.Process(delta);
    }

    public virtual void Process(float delta) {
        if(this.IsBeingPressed() && this.CanTurnOn()) {
            this.TurnOn();
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
        if(!this.cast.IsColliding()) {
            return false;
        }

        Godot.Object collider = this.cast.GetCollider();

        // TODO: Switches can be pressed by other entities as well, handle that
        if(collider is Player) {
            return true;
        }

        return false;
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

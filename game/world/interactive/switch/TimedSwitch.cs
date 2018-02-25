using Godot;
using System;

public class TimedSwitch : SwitchBase {
    private float turnsOffAfter = 3.0f;
    private float notPressedFor = 0.0f;

    public override void Process(float delta) {
        base.Process(delta);
        if(this.IsBeingPressed()) {
            this.notPressedFor = this.turnsOffAfter;
        } else if(this.IsOn() && !this.IsBeingPressed()) {
            this.notPressedFor -= delta;

            if(this.notPressedFor <= 0) {
                this.TurnOff();
            }
        }
    }
}

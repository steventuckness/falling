using Godot;
using System;

public class ToggleSwitch : SwitchBase {
    public override void Process(float delta) {
        bool wasPressed = this.WasPressedLastFrame();
        this.UpdateWasPressedLastFrame();
        if(!wasPressed && this.IsBeingPressed()) {
            if(this.IsOn()) {
                this.TurnOff();
            } else {
                this.TurnOn();
            }
        }
    }
}

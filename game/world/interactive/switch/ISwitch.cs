using Godot;
using System;

public interface ISwitch : ISignal {
    void TurnOn();
    void TurnOff();
    bool IsOn();
    bool CanTurnOn();
}

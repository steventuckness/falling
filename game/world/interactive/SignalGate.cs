using Godot;
using System;
using System.Collections.Generic;

public class SignalGate : Node2D, ISignal {
    [Export]
    public String inputGroup;
    [Export]
    public bool orMode = false;
    private List<ISignal> inputSignals;

    public override void _Ready() {
        this.inputSignals = new List<ISignal>();
        Godot.Array nodes = this.GetTree().GetNodesInGroup(inputGroup);
        foreach (ISignal s in nodes) {
            this.inputSignals.Add(s);
        }
    }

    public bool Output() {
        bool output = this.inputSignals[0].Output();
        foreach (ISignal s in this.inputSignals) {
            output = (this.orMode) ? (output || s.Output()) : (output && s.Output());
        }
        return output;
    }
}

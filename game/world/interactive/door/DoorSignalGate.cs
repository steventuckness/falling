using Godot;
using System;
using System.Collections.Generic;

public class DoorSignalGate : Node2D, ISignal {
    [Export]
    public String inputGroup = "";

    [Export]
    public String doorGroup = "";

    [Export]
    public bool orMode = false;

    private List<IDoor> doors;
    private List<ISignal> signals;
    
    public bool Output() {
        bool output = this.signals[0].Output();
        foreach(ISignal s in this.signals) {
            output = (this.orMode) ? (output || s.Output()) : (output && s.Output());
        }
        return output;
    }

    public override void _Ready() {
        this.doors = new List<IDoor>();
        this.signals = new List<ISignal>();

        object[] inputs = this.GetTree().GetNodesInGroup(inputGroup);
        foreach (ISignal n in inputs) {
            this.signals.Add(n);
        }

        object[] doors = this.GetTree().GetNodesInGroup(doorGroup);
        foreach (IDoor n in doors) {
            this.doors.Add(n);
        }
    }

    public override void _PhysicsProcess(float delta) {
        bool shouldOpenDoors = this.Output();

        if(shouldOpenDoors) {
            foreach(IDoor d in this.doors) {
                d.Open();
            }
        } else {
            foreach(IDoor d in this.doors) {
                d.Close();
            }
        }
    }
}

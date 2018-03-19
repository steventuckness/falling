using System;
using Godot;
using System.Collections.Generic;
namespace Recorder {

public class Recording<S> {
    public List<Frame<S>> Entries;
    public float Length;

    public Recording(List<Frame<S>> entries, float length) {
        this.Entries = entries;
        this.Length = length;
    }
}

}

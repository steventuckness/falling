using System;
using Godot;
using System.Collections.Generic;

public class Recording {
    public List<KeyEntry> entries;
    public float length;

    public Recording(List<KeyEntry> entries, float length) {
        this.entries = entries;
        this.length = length;
    }
}

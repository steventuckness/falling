using System;

public class KeyEntry {
    public enum EventType {
        PRESS, RELEASE
    }

    public string keyCode;
    public float timestamp;

    public EventType eventType;
    public KeyEntry(string keyCode, float timestamp, EventType eventType) {
        this.keyCode = keyCode;
        this.timestamp = timestamp;
        this.eventType = eventType;
    }
}

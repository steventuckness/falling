using System;
using System.Collections.Generic;
using Godot;

public class PlayerClone : Player {
    public Recording recording;

    private float currentTime = 0f;
    private bool isPlaying = true;

    public Vector2 initialPosition;

    public Vector2 initiaVelocity;

    // this will determine how sensitive the inputs will be triggered
    public float timeSensitivity = 0.01f;

    private void SanityCheck() {
        if (initialPosition == null) {
            throw new Exception("Clone has an empty initial position.");
        }
        if (initiaVelocity == null) {
            throw new Exception("Clone has an empty initial velocity");
        }
        if (recording == null) {
            throw new Exception("Clone has no recording");
        }
        if (recording.entries == null) {
            throw new Exception("Clone recording has no entries");
        }
    }

    public override void _Ready() {
        this.menuEnabled = false;
        base._Ready();
        SanityCheck();
        GetInputRecorder().enabled = false;
    }

    /// <summary>
    /// Sorts in reverse order. The first element of the array will be the one
    /// with the greatest timestamp.
    /// </summary>
    /// <param name="entries"></param>
    /// <returns></returns>
    private List<KeyEntry> SortEntries(List<KeyEntry> entries) {
        entries.Sort((a, b) => b.timestamp.CompareTo(a.timestamp));
        return entries;
    }

    private List<KeyEntry> GetFrameKeys(float startTime, float endTime)
        => SortEntries(
            recording.entries.FindAll((entry) => entry.timestamp >= startTime && entry.timestamp <= endTime)
        );

    private List<KeyEntry> GetFrameKey(string key, float startTime, float endTime)
        => GetFrameKeys(startTime, endTime).FindAll((entry) => entry.keyCode == key);

    private KeyEntry LastOccurenceOf(string key, float startTime, float endTime)  {
        try {
            return GetFrameKey(key, startTime, endTime)[0];
        } catch (ArgumentOutOfRangeException e) {
            return null;
        }
    }

    private bool HasEvent(KeyEntry key, KeyEntry.EventType eventType) {
        if (!this.isPlaying || key == null) {
            return false;
        }
        if (key.eventType == eventType) {
            return true;
        }
        return false;
    }

    public override void _Process(float delta) {
        if (!isPlaying) {
            return;
        }
        this.currentTime += delta;
        if (this.currentTime > this.recording.length) {
            this.StartPlayback();
        }
    }

    public void StopPlayback() {
        this.isPlaying = false;
    }

    public void StartPlayback() {
        this.currentTime = 0;
        this.velocity = this.initiaVelocity;
        this.SetPosition(this.initialPosition);
    }

    public override bool IsActionPressed(string key) {
        KeyEntry lastKeyPress = LastOccurenceOf(key, 0f, this.currentTime);
        return HasEvent(lastKeyPress, KeyEntry.EventType.PRESS);
    }

    public override bool IsActionJustPressed(string key) {
        KeyEntry lastKeyPress = LastOccurenceOf(key, this.currentTime - this.timeSensitivity, this.currentTime);
        return HasEvent(lastKeyPress, KeyEntry.EventType.PRESS);
    }

    public override bool IsActionJustReleased(string key) {
        KeyEntry lastKeyPress = LastOccurenceOf(key, this.currentTime - this.timeSensitivity, this.currentTime);
        return HasEvent(lastKeyPress, KeyEntry.EventType.RELEASE);
    }
}

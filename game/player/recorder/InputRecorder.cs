using System;
using Godot;
using System.Collections.Generic;

public class InputRecorder : Node {

	public static string SIGNAL_RECORDING_STARTED = "RecordingStarted";
	public static string SIGNAL_RECORDING_STOPPED = "RecordingStopped";

	// Tried to use string[] here, but it seems [Export] support for that
	// causes issues with the values. Using a list for now, since that seems
	// to work.
	public List<String> keysToRecord = new List<String> {
		"key_left",
		"key_right",
		"key_jump",
		"key_sprint"
	};

	[Export]
	public bool enabled = true;

	[Export]
	public string triggerKey = "key_record";

	[Export]
	public bool holdToRecord = false;

	public bool isRecording;
	public Recording lastRecording;
	private float time;

	public List<KeyEntry> entries;

	public override void _Ready() {
		this.AddUserSignal(SIGNAL_RECORDING_STARTED);
		this.AddUserSignal(SIGNAL_RECORDING_STOPPED);
	}

	public override void _Process(float delta) {
		if (!enabled) {
			return;
		}
		StartStopRecording();
		if (isRecording) {
			Record(delta);
		}
	}

	private void StartStopRecording() {
		if (isRecording && 
			(  (holdToRecord && !Input.IsActionJustPressed(triggerKey))
			|| (!holdToRecord && Input.IsActionJustPressed(triggerKey))
			)
		) {
			StopRecording();
		}
		else if (!isRecording && Input.IsActionJustPressed(triggerKey)) {
			StartRecording();
		}
	}

	private void Record(float delta) {
		this.time += delta;
		foreach (string key in keysToRecord) {
			if (Input.IsActionJustPressed(key)) {
				entries.Add(new KeyEntry(key, this.time, KeyEntry.EventType.PRESS));
				GD.Print("Pressed: " + key + " | " + time);
			}
			if (Input.IsActionJustReleased(key)) {
				entries.Add(new KeyEntry(key, this.time, KeyEntry.EventType.RELEASE));
			}
		}
	}

	public void StartRecording() {
		time = 0;
		isRecording = true;
		entries = new List<KeyEntry>();
		this.EmitSignal(SIGNAL_RECORDING_STARTED);
		GD.Print("Recording started.");
	}

	public void StopRecording() {
		isRecording = false;
		this.lastRecording = new Recording(entries, time);
		this.EmitSignal(SIGNAL_RECORDING_STOPPED);
		GD.Print("Recording ended.");
	}

}

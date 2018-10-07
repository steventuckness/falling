using System;
using Godot;
using System.Collections.Generic;

namespace Recorder {

public class FrameRecorder<S> {

    public delegate S StateRetrievalDel();
    public delegate bool InputRequestDel(string key);
    public delegate void StartRecordingDel();
    public delegate void StopRecordingDel(Recording<S> recording);

    public bool IsEnabled = true;
    public string TriggerKey = "key_record";
    public bool HoldToRecord = false;
    public bool IsRecording { get { return isRecording; }}
    public StateRetrievalDel GetState;
    public InputRequestDel IsActionJustPressed;

    private bool isRecording;
    private float Time;

    private float maxTime = 5.0f; 
    private List<Frame<S>> Frames;
    private StartRecordingDel onRecordingStart;
    private StopRecordingDel onRecordingStop;

    public FrameRecorder(StateRetrievalDel getState,
    StartRecordingDel onRecordingStart,
    StopRecordingDel onRecordingStop)  {
        this.GetState = getState;
        this.onRecordingStart = onRecordingStart;
        this.onRecordingStop = onRecordingStop;
        this.IsActionJustPressed = 
            (string key) => Input.IsActionJustPressed(key);

    }

    private void StartStopRecording() {
        if (IsRecording && 
            (  (HoldToRecord && !this.IsActionJustPressed(TriggerKey))
            || (!HoldToRecord && this.IsActionJustPressed(TriggerKey))
            || this.Time >= this.maxTime
            )
        ) {
            StopRecording();
        }
        else if (!IsRecording && this.IsActionJustPressed(TriggerKey)) {
            StartRecording();
        }
    }

    public void StartRecording() {
        Time = 0;
        isRecording = true;
        Frames = new List<Frame<S>>();
        onRecordingStart();
    }

    public Recording<S> StopRecording() {
        isRecording = false;
        Recording<S> recording = new Recording<S>(Frames, Time);
        onRecordingStop(recording);
        return recording;
    }

    public void Process(float delta) {
        if (!IsEnabled) return;
        StartStopRecording();
        if (isRecording) {
            Record(delta);
        }
    }

    private void Record(float delta) {
        this.Time += delta;
        Frames.Add(new Frame<S>(this.Time, this.GetState()));
    }

    public float MaxTime {
        set {
            this.maxTime = value;
        }
        get => this.maxTime;
    }

    public float GetRecordCapacityUsedPerentage() => this.Time / this.maxTime;
}

}

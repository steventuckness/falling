
using System;

namespace Recorder {

public class Frame<S> {
    public float Timestamp;
    public S State;

    public Frame(float timestamp, S state) {
        this.Timestamp = timestamp;
        this.State = state;
    }
}

}

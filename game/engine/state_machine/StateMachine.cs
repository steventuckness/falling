using Godot;
using System;
using System.Collections.Generic;

public class StateMachine<T> {
    private State<T> currentState = null;
    private State<T> nextState = null;
    private float currentStateTime = 0;

    public void Init(State<T> initialState) {
        this.nextState = initialState;
    }

    public State<T> GetCurrentState() {
        return this.currentState;
    }

    // This will transition the state on the next update tick
    public void TransitionState(State<T> next) {
        this.nextState = next;
    }

    // Updates the state machine
    public void Update(float delta, T owner) {
        // A change has happened!
        if (this.nextState != this.currentState) {
            if (this.currentState != null) {
                this.currentState.OnExit(delta, owner);
            }
            this.nextState.OnEnter(delta, owner);
            this.currentState = this.nextState;
            this.currentStateTime = 0; // Restart the timer
        }
        else {
            this.currentStateTime += delta;
        }

        if (this.currentState != null) {
            State<T> next = this.currentState.Update(delta, owner, this.currentStateTime);
            if(next != null && next != this.currentState) {
                this.TransitionState(next);
            }
        }
    }
}

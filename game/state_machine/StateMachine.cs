using Godot;
using System;
using System.Collections.Generic;

public class StateMachine<T> {
    private State<T> initialState = null;
    private State<T> currentState = null;
    private float currentStateTime = 0;

    public void Init(State<T> initialState) {
        this.initialState = initialState;
    }

    public void Update(float delta, T owner) {
        // Handle the first time entering a state
        if(this.currentState == null) {
            this.currentState = this.initialState;
            this.initialState.OnEnter(delta, owner);
        }

        if(this.currentState != null) {
            State<T> next = this.currentState.Update(delta, owner, this.currentStateTime);
            if(next != null && next != this.currentState) {
                this.currentStateTime = 0;
                this.currentState.OnExit(delta, owner);
                next.OnEnter(delta, owner);
                this.currentState = next;
            }
            if(next == null || next == this.currentState) {
                this.currentStateTime += delta;
            }
        }
    }
}

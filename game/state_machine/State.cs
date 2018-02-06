using Godot;
using System;

public abstract class State<T> {
    public virtual String GetName(){ return ""; }
    public virtual void OnEnter(float delta, T owner) {}
    public virtual void OnExit(float delta, T owner) {}
    public virtual State<T> Update(float delta, T owner, float timeInState) {
        return null;
    }
}

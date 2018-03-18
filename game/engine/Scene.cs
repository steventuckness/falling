using Godot;
using System;
using System.Collections.Generic;

public class Scene : Node2D {
    private SceneManager manager = new SceneManager();
    public SceneManager GetManager() => this.manager;
}

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages changing scenes. See scene_switching.md for more information.
/// Ensure all scenes are in the acts list.
/// Use Next() to go to the next scene, or GoToScene to jump to a particular
/// scene.
/// </summary>
public class ActManager : Node {

    private const String PRELOAD_DONE = "PreloadDone";

    private String currentAct;

    public List<String> acts = new List<String> {
        "res://levels/BlazeIt.tscn",
        "res://levels/Invisibility.tscn",
        "res://levels/NecessarySuicide.tscn",
        "res://levels/SwitchItUp.tscn"
    };

    public ActManager() : base() {
        ValidateActs();
        AddUserSignal(PRELOAD_DONE);
        this.currentAct = acts[0];
    }

    public void NextAct() {
        GoToScene(NextActResource());
    }

    public void PreloadActs() {
        foreach (String act in acts) {
            ResourceLoader.Load(act);
        }
        EmitSignal(PRELOAD_DONE);
    }

    public void GoToScene(String path) {
        CallDeferred("DeferredGotoScene", path);
    }

    private void ValidateActs() {
        foreach (String act in acts) {
            if (!(new File()).FileExists(act)) {
                throw new Exception("Act " + act + " does not exist.");
            }
        }
    }

    private String NextActResource() {
        int currentSceneIndex = acts.FindIndex((act) => act == currentAct);
        if (currentSceneIndex + 1 >= acts.Count) {
            throw new Exception("Cannot go past the last scene in the " +
             "ActManager.");
        }
        if (currentSceneIndex < 0) {
            throw new Exception("The current scene is not registered in the " +
                " act manager, or the act manager was not used to load the" +
                " current scene. Ensure that the act manager was used to load" +
                " the current scene and that it is properly registered in" +
                " the acts variable.");
        }
        return acts[currentSceneIndex + 1];

    }

    private Node GetCurrentScene() {
        Viewport root = GetTree().GetRoot();

        // By Godot standards, the last child of the root is always the current
        // scene.
        return root.GetChild(root.GetChildCount() - 1);
    }

    private Node InstantiateScene(string path) =>
        ((PackedScene)ResourceLoader.Load(path)).Instance();

    private void DeferredGotoScene(String path) {
        GetCurrentScene().Free();
        Node nextScene = InstantiateScene(path);
        GetTree().GetRoot().AddChild(nextScene);
        GetTree().SetCurrentScene(nextScene);
        this.currentAct = path;
    }
}

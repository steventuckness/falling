# Scene Switching

## Autoload

Scene switching in Godot is done by using an `autoload`. Those are scripts that
get attached to all scenes in a game, once they are loaded as main scenes. 

See http://docs.godotengine.org/en/3.0/getting_started/step_by_step/singletons_autoload.html

The main autoload for scene switching is `ActManager`.

## Scene Caching

Godot automatically caches all the resources that are loaded. Hence loading a
resource again will not read it from disk. It is enough to just call
`ResourceLoader.Load(path)` and Godot will know if that resource has been loaded
before. However, **never call that directly** outside the `ActManager`. Otherwise,
the `ActManager` will lost track of what scene you currently are.

## Switching scenes

To go to the next scene, access the `ActManager` from any scene and call the 
`Next()` method:

```
((ActManager)GetNode("/root/actmanager")).Next();
```


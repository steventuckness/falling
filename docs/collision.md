# Collision
This doc describes the custom collision built into Godot.

## Hierarchy
First, any object which needs to interact with the collision system should be an `Entity`. An entity, in order to be collidable, should have a `Collider` node as a child.

**Example**
```bash
|- Player (Entity)
    |- Collider (BoxCollider)
|- CollisionTiles (SolidTiles)
    |- Collider (GridCollider)
```
Note: You **should** name the collider `Collider`.

## Collision Testing
All subclasses of entity have access to the `CollisionEngine` member variable. This class houses various helpers for testing collision between objects in the scene.

**Example**
```cs
/* Player.cs */
public class Player {
    /* Performs a check exactly 1 pixel below the current position, using the collider */
    public bool IsOnFloor() =>
        this.collision.CollideCheck<Solid>(this.GetPosition() + new Vector(0, 1));
}
```

## Movement
To move an `Entity` (this might be refactored into `Actor` later), you can call the base movement methods.

* `MoveX(float x, OnCollide onCollide)` - Move the entity in the x direction
* `MoveY(float y, OnCollide onCollide)` - Move the entity in the y direction
* `Move(Vector2 move, OnCollide onCollideH, OnCollide onCollideV)` - Helper to move the entity in the `x` and `y` direction (simply calls both the above methods).

Note: You can provide a delegate to handle the collision, should it occur.

**Example**
```cs
/* Player.cs */
public class Player {
    public override void _PhysicsProcess(float delta) {
        this.MoveX(this.velocity.x * delta, () => {
            this.velocity.x = 0;
        });
        this.MoveY(this.velocity.y * delta, () => {
            this.velocity.y = 0;
        });
    }
}
```


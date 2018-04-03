using Godot;
using System;

public class PlatformRider : Component {
    public bool IsRiding(Solid platform) {
        return this.entity.collision.CollideFirst<Solid>(
            this.entity.GetPosition() + new Vector2(0, 1)
        ) == platform;
    }
}

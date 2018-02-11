using System;
using Godot;

public class Acceleration {
    public static Vector2 Apply(
        Vector2 acceleration,
        float delta,
        Vector2 velocity
    ) =>
        new Vector2(
            velocity.x + (delta * acceleration.x),
            velocity.y + (delta * acceleration.y)
        );

    public static Vector2 ApplyX(
        float acceleration,
        float delta,
        Vector2 velocity
    ) =>
        Apply(new Vector2(acceleration, 0), delta, velocity);

    public static Vector2 ApplyY(
        float acceleration,
        float delta,
        Vector2 velocity
    ) =>
        Apply(new Vector2(0, acceleration), delta, velocity);

    public static Vector2 ApplyTerminalX(
        float terminalVel,
        float acceleration,
        float delta,
        Vector2 velocity
    ) =>
        Math.Abs(velocity.x) < terminalVel
            ? Apply(new Vector2(acceleration, 0), delta, velocity)
            : velocity;

    public static Vector2 ApproachX(
        float approach,
        float acceleration,
        float delta,
        Vector2 velocity
    ) =>
        velocity.x < approach ?
            new Vector2(Mathf.Min(velocity.x + (acceleration * delta), approach), velocity.y) :
            new Vector2(Mathf.Max(velocity.x - (acceleration * delta), approach), velocity.y);

    public static Vector2 ApplyTerminalY(
        float terminalVel, 
        float acceleration, 
        float delta, 
        Vector2 velocity
    ) => 
        Math.Abs(velocity.y) < terminalVel
            ? Apply(new Vector2(0, acceleration), delta, velocity)
            : velocity;

}

using System;
using Godot;
using System.Collections;
using System.Collections.Generic;

/*
 * The PlayerCollision class can house common collision checks
 * for the player class.
 */
public class PlayerCollision {
    private Player player;
    private enum RayCastOrigins {
        TOP_LEFT,
        TOP_MIDDLE,
        TOP_RIGHT,
        CENTER_RIGHT,
        BOTTOM_RIGHT,
        BOTTOM_MIDDlE,
        BOTTOM_LEFT,
        CENTER_LEFT
    };

    public bool isOnGround = false;
    private Dictionary<RayCastOrigins, Vector2> raycastOrigins;
    private object[] exclude;

    public PlayerCollision(Player player) {
        this.player = player;
        this.raycastOrigins = new Dictionary<RayCastOrigins, Vector2>();
        this.exclude = new object[]{ this.player };
    }

    // Given the desired points around the player, this will prime the
    // starting points of the raycasts.
    private void PrimeRayCastOrigins() {
        Vector2 p = this.player.GetGlobalPosition();
        CollisionShape2D collider = (CollisionShape2D) this.player.GetNode("CollisionShape2D");
        RectangleShape2D rect = (RectangleShape2D) collider.GetShape();
        Vector2 extent = rect.GetExtents();

        Vector2 topLeft = p + extent * (new Vector2(-1, -1));
        Vector2 topRight = p + extent * (new Vector2(1, -1));
        Vector2 bottomRight = p + extent * (new Vector2(1, 1));
        Vector2 bottomLeft = p + extent * (new Vector2(-1, 1));

        this.raycastOrigins.Clear();
        // Corners
        this.raycastOrigins.Add(RayCastOrigins.TOP_LEFT, topLeft);
        this.raycastOrigins.Add(RayCastOrigins.TOP_RIGHT, topRight);
        this.raycastOrigins.Add(RayCastOrigins.BOTTOM_RIGHT, bottomRight);
        this.raycastOrigins.Add(RayCastOrigins.BOTTOM_LEFT, bottomLeft);

        // Centers
        this.raycastOrigins.Add(RayCastOrigins.TOP_MIDDLE, (topLeft + topRight) / 2);
        this.raycastOrigins.Add(RayCastOrigins.BOTTOM_MIDDlE, (bottomLeft + bottomRight) / 2);
        this.raycastOrigins.Add(RayCastOrigins.CENTER_RIGHT, (topRight + bottomRight) / 2);
        this.raycastOrigins.Add(RayCastOrigins.CENTER_LEFT, (topLeft + bottomLeft) / 2);
    }

    // Checks to see if the player is on the ground by casting rays
    // from the feet of the player.
    public bool IsOnGround() {
        // Prime raycast origins
        float skinWidth = this.player.GetSafeMargin();
        Vector2 bottomRight = this.raycastOrigins[RayCastOrigins.BOTTOM_RIGHT];
        Vector2 bottomLeft = this.raycastOrigins[RayCastOrigins.BOTTOM_LEFT];

        // Use the skin width to calculate raycast length
        Vector2 castSize = new Vector2(0, skinWidth + 0.1f);

        // Check at 2 points (bottom left and bottom right)
        // TODO: This could be enhanced to check for more points
        Dictionary<object, object> dr = this.player.GetWorld2d().GetDirectSpaceState().IntersectRay(
            bottomRight, bottomRight + castSize, this.exclude
        );
        Dictionary<object, object> dl = this.player.GetWorld2d().GetDirectSpaceState().IntersectRay(
            bottomLeft, bottomLeft + castSize, this.exclude
        );

        return dl.ContainsKey("collider") || dr.ContainsKey("collider");
    }

    public void Update() {
        this.PrimeRayCastOrigins();
        this.isOnGround = IsOnGround();
    }
}

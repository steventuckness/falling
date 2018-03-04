using Godot;
using System;

public class Cam : Node2D {
    private int maxCamValue = 10000000;
    private PlayerNode follow;
    private Camera2D camera;
    private Vector2 viewportSize;
    private Vector2 center;

    public struct CamLimits {
        public Vector2 min;
        public Vector2 max;
    }

    private CamLimits camLimits = new CamLimits();
    [Export]
    public Vector2 anchorMargin = new Vector2(0.333f, 0.333f);
    [Export]
    public Vector2 cameraSnapStep = new Vector2(128f, 128f);
    [Export]
    public bool hSnapWithVelocity = false;
    [Export]
    public bool showDebugLines = false;

    // Anchors /////////////////////////////////////////////////////////////////
    private struct HorizontalAnchor {
        public Vector2 center;
        public Vector2 left;
        public Vector2 right;
    }
    private struct VerticalAnchor {
        public Vector2 center;
        public Vector2 top;
        public Vector2 bottom;
    }
    private HorizontalAnchor horizontalAnchors = new HorizontalAnchor();
    private VerticalAnchor verticalAnchors = new VerticalAnchor();

    // State ///////////////////////////////////////////////////////////////////
    private enum HorizontalSnapState {
        LEFT,
        RIGHT
    };
    private enum VerticalSnapState {
        UNLOCKED,
        SNAP_TO_FOOR
    };
    // Data relevant to the vertical state of the cam
    private struct VerticalStateData {
        public bool reachedSnap;
        public float fallThreshold;
    }

    private HorizontalSnapState horizontalSnapState = HorizontalSnapState.LEFT;
    private VerticalSnapState verticalSnapState = VerticalSnapState.UNLOCKED;
    private VerticalStateData verticalStateData = new VerticalStateData();

    public override void _Ready() {
        // Init
        this.camera = (Camera2D)this.GetNode("Camera2D");
        this.viewportSize = this.GetViewport().GetSize();
        this.center = this.viewportSize * 0.5f;

        // Camera bounds
        this.camLimits = new CamLimits();
        this.camLimits.min = new Vector2(-maxCamValue, -maxCamValue);
        this.camLimits.max = new Vector2(maxCamValue, maxCamValue);

        // Horizontal anchors
        horizontalAnchors.center = this.viewportSize * 0.5f; // Perfect center
        horizontalAnchors.left = new Vector2(this.viewportSize.x * this.anchorMargin.x, this.viewportSize.y * 0.5f);
        horizontalAnchors.right = new Vector2(this.viewportSize.x - this.viewportSize.x * this.anchorMargin.x, this.viewportSize.y * 0.5f);

        // Vertical anchors
        verticalAnchors.center = this.viewportSize * 0.5f;
        verticalAnchors.top = new Vector2(this.viewportSize.x * 0.5f, this.viewportSize.y * anchorMargin.y);
        verticalAnchors.bottom = new Vector2(this.viewportSize.x * 0.5f, this.viewportSize.y - (this.viewportSize.y * anchorMargin.y));

        // State
        this.verticalStateData.reachedSnap = false;
        this.verticalStateData.fallThreshold = 32f;
    }

    public void Follow(PlayerNode follow) {
        this.follow = follow;
        // TODO: Consider entering another state where we move the camera
        // to the new follow's position before letting other things happen.
        this.SetPosition(this.follow.GetPosition());
    }

    public CamLimits GetLimits() {
        return this.camLimits;
    }

    public void UnsetLimits() {
        this.camLimits.min = new Vector2(-this.maxCamValue, -this.maxCamValue);
        this.camLimits.max = new Vector2(this.maxCamValue, this.maxCamValue);
    }

    public void SetLimits(CamLimits limits) {
        this.camLimits = limits;
    }

    private Vector2 ClampLimits(Vector2 pos) {
        CamLimits limits = this.camLimits;
        Vector2 min = limits.min;
        Vector2 max = limits.max;

        // Adjust for screen size (center of camera)
        min.x += this.viewportSize.x * 0.5f;
        min.y += this.viewportSize.y * 0.5f;
        max.x -= this.viewportSize.x * 0.5f;
        max.y -= this.viewportSize.y * 0.5f;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        return pos;
    }

    private float DistanceToFloor() {
        Vector2 floorLine = this.center - this.verticalAnchors.bottom;
        Vector2 normalized = this.GetPosition() - floorLine;
        return (this.follow.GetPosition() - normalized).y;
    }

    private float HandleHorizontalState(float delta) {
        float x = this.GetPosition().x;
        float offset = 0.0f;
        float playerX = this.follow.GetPosition().x;
        float currentX = this.GetPosition().x;
        float step = this.cameraSnapStep.x;

        switch (this.horizontalSnapState) {
            case HorizontalSnapState.LEFT:
                if (this.follow.velocity.x < 0) {
                    this.horizontalSnapState = HorizontalSnapState.RIGHT;
                }
                else if (!this.hSnapWithVelocity || this.follow.velocity.x > 0) {
                    // TODO: Refactor this
                    offset = this.center.x - this.horizontalAnchors.left.x;

                    float desiredX = playerX + offset;
                    float currentDelta = desiredX - currentX;

                    x = x + Mathf.Sign(desiredX - currentX) * (step * delta);

                    // If we reached or went beyond our desired position, stop it
                    if (Mathf.Sign(desiredX - x) != Mathf.Sign(currentDelta) || x == desiredX) {
                        x = desiredX;
                    }
                }
                break;
            case HorizontalSnapState.RIGHT:
                if (this.follow.velocity.x > 0) {
                    this.horizontalSnapState = HorizontalSnapState.LEFT;
                }
                else if (!this.hSnapWithVelocity || this.follow.velocity.x < 0) {
                    // TODO: Refactor this
                    offset = this.center.x - this.horizontalAnchors.right.x;

                    float desiredX = playerX + offset;
                    float currentDelta = desiredX - currentX;

                    x = x + Mathf.Sign(desiredX - currentX) * (step * delta);

                    // If we reached or went beyond our desired position, stop it
                    if (Mathf.Sign(desiredX - x) != Mathf.Sign(currentDelta) || x == desiredX) {
                        x = desiredX;
                    }
                }
                break;
        }

        return x;
    }

    private float HandleVerticalState(float delta) {
        float playerY = this.follow.GetPosition().y;
        float currentY = this.GetPosition().y;
        float y = currentY;
        float distanceToFloor = this.DistanceToFloor();

        switch (this.verticalSnapState) {
            case VerticalSnapState.UNLOCKED:
                if (this.follow.IsOnFloor()) {
                    this.verticalSnapState = VerticalSnapState.SNAP_TO_FOOR;
                    this.verticalStateData.reachedSnap = false;
                }
                if (this.follow.IsFalling()) {
                    // We've fallen below a threshold, so now we'll start snapping
                    if (distanceToFloor > this.verticalStateData.fallThreshold) {
                        this.verticalStateData.reachedSnap = false;
                        this.verticalSnapState = VerticalSnapState.SNAP_TO_FOOR;
                    }
                }
                break;
            case VerticalSnapState.SNAP_TO_FOOR:
                // We only want to enter the unlocked vertical camera (no tracking) when...
                // 1. Our desired snap point has been met AND
                //      1.a. The entity is jumping OR
                //      1.b. The entity is falling AND
                //          1.b.i. The entity has not fallen greater than our fall threshold
                // 2. Otherwise we want to vertical snap the camera
                if (
                    this.verticalStateData.reachedSnap && (this.follow.IsJumping() ||
                    (this.follow.IsFalling() && distanceToFloor <= this.verticalStateData.fallThreshold))
                ) {
                    this.verticalSnapState = VerticalSnapState.UNLOCKED;
                }
                else {
                    // Handle the snapped to floor state
                    float step = this.cameraSnapStep.y;
                    float offset = this.center.y - this.verticalAnchors.bottom.y;
                    float desiredY = playerY + offset;
                    float currentDelta = desiredY - currentY;

                    y = y + Mathf.Sign(desiredY - currentY) * (step * delta);

                    // If we reached or went beyond our desired position, stop it
                    if (Mathf.Sign(desiredY - y) != Mathf.Sign(currentDelta) || y == desiredY) {
                        this.verticalStateData.reachedSnap = true;
                        y = desiredY;
                    }
                }
                break;
        }

        return y;
    }

    public override void _PhysicsProcess(float delta) {
        this.SetPosition(this.ClampLimits(
            new Vector2(
                this.HandleHorizontalState(delta),
                this.HandleVerticalState(delta)
            )
        ));
    }

    public override void _Draw() {
        if (!this.showDebugLines) {
            return;
        }

        // Center
        this.DrawLine(
            new Vector2(0, -this.center.y),
            new Vector2(0, this.center.y),
            new Color(256, 0, 0, 1),
            1, false
        );

        // Left Anchor
        this.DrawLine(
            new Vector2(-(this.center.x - this.horizontalAnchors.left.x), -this.center.y),
            new Vector2(-(this.center.x - this.horizontalAnchors.left.x), this.center.y),
            new Color(256, 0, 0, 1),
            1, false
        );

        // Right Anchor
        this.DrawLine(
            new Vector2(-(this.center.x - this.horizontalAnchors.right.x), -this.center.y),
            new Vector2(-(this.center.x - this.horizontalAnchors.right.x), this.center.y),
            new Color(256, 0, 0, 1),
            1, false
        );

        // Platform
        this.DrawLine(
            new Vector2(-this.center.x, -(this.center.y - this.verticalAnchors.bottom.y)),
            new Vector2(this.center.x, -(this.center.y - this.verticalAnchors.bottom.y)),
            new Color(256, 0, 0, 1),
            1, false
        );
    }
}

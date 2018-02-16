using Godot;
using System;

public class Cam : Node2D {
    private int         maxCamValue = 10000000;
    private Player      follow;
    private Camera2D    camera;
    private Vector2     viewportSize;
    private Vector2     center;

    // Offset management
    private Vector2 offset;

    public struct CamLimits {
        public Vector2 min;
        public Vector2 max;
    }

    // Anchor management
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

    private struct ActiveAnchors {
        public Vector2 h;
        public Vector2 v;
    }

    private CamLimits           camLimits = new CamLimits();
    private HorizontalAnchor    hAnchors = new HorizontalAnchor();
    private VerticalAnchor      vAnchors = new VerticalAnchor();
    private ActiveAnchors       activeAnchors = new ActiveAnchors();

    [Export]
    public Vector2 anchorMargin = new Vector2(0.333f, 0.333f);

    public void Follow(Player follow) {
        this.follow = follow;
    }

    public override void _Ready() {
        // Init
        this.camera = (Camera2D)this.GetNode("Camera2D");
        this.viewportSize = this.GetViewport().GetSize();
        this.center = this.viewportSize * 0.5f;
        this.offset = new Vector2(0, 0);

        // Camera bounds
        this.camLimits = new CamLimits();
        this.camLimits.min = new Vector2(-maxCamValue, -maxCamValue);
        this.camLimits.max = new Vector2(maxCamValue, maxCamValue);

        // Horizontal anchors
        hAnchors.center = this.viewportSize * 0.5f; // Perfect center
        hAnchors.left = new Vector2(this.viewportSize.x * this.anchorMargin.x, this.viewportSize.y * 0.5f);
        hAnchors.right = new Vector2(this.viewportSize.x - this.viewportSize.x * this.anchorMargin.x, this.viewportSize.y * 0.5f);

        // Vertical anchors
        vAnchors.center = this.viewportSize * 0.5f;
        vAnchors.top = new Vector2(this.viewportSize.x * 0.5f, this.viewportSize.y * anchorMargin.y);
        vAnchors.bottom = new Vector2(this.viewportSize.x * 0.5f, this.viewportSize.y - (this.viewportSize.y * anchorMargin.y));

        // Active anchor
        activeAnchors.h = hAnchors.center;
        activeAnchors.v = vAnchors.center;
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

    private Vector2 HandleHorizontal(Player follow) {
        if(follow.velocity.x > 0) {
            this.activeAnchors.h = this.hAnchors.left;
        } else if(follow.velocity.x < 0) {
            this.activeAnchors.h = this.hAnchors.right;
        }
        return new Vector2(this.center.x - this.activeAnchors.h.x, 0);
    }

    private Vector2 HandleVertical(Player follow) {
        if(follow.velocity.y > 0) {
            this.activeAnchors.v = this.vAnchors.top;
        } else if(follow.velocity.y < 0) {
            this.activeAnchors.v = this.vAnchors.bottom;
        }
        return new Vector2(0, this.center.y - this.activeAnchors.v.y);
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

    public override void _PhysicsProcess(float delta) {
        // Assume we're "centered" on the thing we're following
        Vector2 currentPos = this.GetPosition();
        Vector2 followPos = this.follow.GetPosition();
        Vector2 desiredOffset = new Vector2(0, 0);

        // Horizontal
        desiredOffset += this.HandleHorizontal(this.follow);
        // Vertical
        desiredOffset += this.HandleVertical(this.follow);

        // Use offset to position the camera
        Vector2 pos = followPos + desiredOffset;

        pos.x = Mathf.Lerp(currentPos.x, pos.x, 1f * delta);
        pos.y = Mathf.Lerp(currentPos.y, pos.y, 1f * delta);

        Vector2 clamped = this.ClampLimits(pos);

        this.SetPosition(clamped);
    }
}

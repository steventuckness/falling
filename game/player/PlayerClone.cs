using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public class PlayerClone : Player {
    public Recorder.Recording<PlayerRecorderFrame> recording;
    private Recorder.FramePlayer<PlayerRecorderFrame> movementPlayer;

    private SubPixelFloat move;

    public override void _Ready() {
        this.menuEnabled = false;
        base._Ready();
        this.movementPlayer = new Recorder.FramePlayer<PlayerRecorderFrame>(
            this.recording,
            (state) => state.ApplyOnPlayer(this)
        );

        this.move = new SubPixelFloat();
        this.movementPlayer.loop = false;
        this.movementPlayer.StartPlayback();
        this.movementPlayer.GoToTheEnd();
        this.movementPlayer.StopPlayback();
    }

    public override void SetColor(PlayerColor.Value color) {
        base.SetColor(color);
        var originalColor = PlayerColor.ToColor(color);
        var alphaColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.7f);
        ((Sprite)this.GetNode("Sprite")).SetModulate(alphaColor);
    }

    public override void _PhysicsProcess(float delta) {
        if (this.movementPlayer.IsAtTheEnd()) {
            if (this.stillNeedsLerping() && !this.movementPlayer.IsPlaying()) {
                lerpCloneToStartingPosition(delta);
                return;
            }
            else {
                this.movementPlayer.StartPlayback();
            }
        }

        Vector2 positionBefore = this.GetPosition();
        this.movementPlayer.Process(delta);
        Vector2 position = this.GetPosition();
        Vector2 velocity = position - positionBefore;
        // Movement occurred
        if (velocity.x != 0 || velocity.y != 0) {
            // Revert movement for now
            this.SetPosition(positionBefore);
        }
        // this.SetPosition(position);
        // Grab a list of the actual player objects in the scene.
        List<PlayerNode> players = this.node.scene.GetManager().GetEntitiesBy<PlayerNode>()
            .Where(p => p != this.node && p.implementation.GetType() == typeof(Player)).ToList();

        // Check to see if any of the players are riding on this clone.
        List<PlayerNode> riders = players.Where(p => p.IsRidingClone(this)).ToList();

        Collider thisCollider = this.node.GetCollider();
        thisCollider.IsCollidable = false;

        // Horizontal
        if (velocity.x != 0) {
            // Move in the x-direction
            this.SetPosition(this.GetPosition() + new Vector2(velocity.x, 0));

            foreach (PlayerNode p in players) {
                Collider pCollider = p.GetCollider();

                // Overlaps
                if (p.GetCollider().Collides(this.node.GetCollider())) {
                    float moveByX = velocity.x > 0 ? (thisCollider.Right() - pCollider.Left()) : (thisCollider.Left() - pCollider.Right());
                    p.MoveX(moveByX, p.CollideNoop);
                }
                else if (riders.Contains(p)) {
                    p.MoveX(velocity.x, p.CollideNoop);
                }
            }
        }

        // Vertical
        if (velocity.y != 0) {
            // Move in the y-direction
            this.SetPosition(this.GetPosition() + new Vector2(0, velocity.y));

            foreach (PlayerNode p in players) {
                Collider pCollider = p.GetCollider();

                // Overlaps
                if (p.GetCollider().Collides(this.node.GetCollider())) {
                    float moveByY = velocity.y > 0 ? (thisCollider.Bottom() - pCollider.Top()) : (thisCollider.Top() - pCollider.Bottom());
                    p.MoveY(moveByY, p.CollideNoop);
                }
                else if (riders.Contains(p)) {
                    p.MoveY(velocity.y, p.CollideNoop);
                }
            }
        }
        thisCollider.IsCollidable = true;
    }

    private Boolean stillNeedsLerping() {
        if ((this.GetPosition().DistanceTo(this.GetFirstRecordedPosition()) >= 1)) {
            return true;
        }
        return false;
    }

    private void lerpCloneToStartingPosition(float delta) {
        // TODO: refactor some code from the ActRespawn and call it here and there
        Vector2 position = this.GetPosition();
        Vector2 targetPosition = this.GetFirstRecordedPosition();

        Vector2 lerpedPosition = position.LinearInterpolate(targetPosition, 5f * delta);
        Vector2 diff = move.Update(lerpedPosition - position);

        // move clone back to the target position
        this.SetPosition(position + diff);
    }

    public override bool IsActionPressed(string key) {
        return false;
    }

    public override bool IsActionJustPressed(string key) {
        return false;
    }

    public override bool IsActionJustReleased(string key) {
        return false;
    }

    protected override void RecordingStarted() { }

    protected override void RecordingStopped(Recorder.Recording<PlayerRecorderFrame> recording) { }

    public override bool IsRidingClone(PlayerClone clone) => false;

    public Vector2 GetFirstRecordedPosition() {
        // get earliest position
        return this.recording.Entries.Last().State.Position;
    }
}

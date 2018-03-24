using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Rect;

public class Solid : Entity {
    private Vector2 velocity;
    [Export]
    public bool stationary = true;
    public bool isCollidable = true;
    public override void _Ready() {
        base._Ready();
        this.velocity = new Vector2(10, 10);
        this.remainders = new SubPixelFloat();
    }

    public override void _PhysicsProcess(float delta) {
        if (this.stationary) {
            return;
        }
        this.Move(this.velocity * delta);
    }

    public bool Overlaps(PlayerNode player) =>
        this.GetCollider().Collides(player.GetCollider());

    public virtual void Move(Vector2 move) {
        int moveX = this.remainders.UpdateX(move.x);
        int moveY = this.remainders.UpdateY(move.y);

        if (moveX == 0 && moveY == 0) {
            return;
        }
        List<PlayerNode> players = this.scene.GetManager().GetEntitiesBy<PlayerNode>();
        List<PlayerNode> riders = players.Where(p => p.IsRiding(this)).ToList();
        if (moveX != 0) {
            MoveX(moveX, players, riders);
        }
        if (moveY != 0) {
            MoveY(moveY, players, riders);
        }
    }

    private void MoveY(int moveY, List<PlayerNode> players, List<PlayerNode> riders) {
        this.SetPosition(this.GetPosition() + new Vector2(0, moveY));

        foreach (PlayerNode p in players) {
            // Player is overlapping, push
            if (this.Overlaps(p)) {
                Collider pCollider = p.GetCollider();
                Collider sCollider = this.GetCollider();
                float movePlayerY = moveY > 0 ? ((sCollider.Bottom()) - (pCollider.Top())) : ((sCollider.Top()) - (pCollider.Bottom()));
                p.MoveY(movePlayerY, p.CollideNoop);
            }
            else if (riders.Contains(p)) {
                p.MoveY(moveY, p.CollideNoop);
            }
        }
    }

    private void MoveX(int moveX, List<PlayerNode> players, List<PlayerNode> riders) {
        this.SetPosition(this.GetPosition() + new Vector2(moveX, 0));

        // Loop through all players that were touching
        foreach (PlayerNode p in players) {
            // Player is overlapping, push
            if (this.Overlaps(p)) {
                Collider pCollider = p.GetCollider();
                Collider sCollider = this.GetCollider();
                float movePlayerX = moveX > 0 ? ((sCollider.Right()) - (pCollider.Left())) : ((sCollider.Left()) - (pCollider.Right()));
                p.MoveX(movePlayerX, p.CollideNoop);
            }
            else if (riders.Contains(p)) {
                p.MoveX(moveX, p.CollideNoop);
            }
        }
    }
}

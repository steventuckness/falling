using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MovingPlatform : Component {
    [Export]
    public float speed = 64f;
    private float speedFactor;
    private float time = 0f;
    private Vector2 min;
    private Vector2 max;
    private float dist = 0f;

    public override void _EnterTree() {
        base._EnterTree();

        Node2D min = this.GetNode("Min") as Node2D;
        Node2D max = this.GetNode("Max") as Node2D;

        this.min = this.ToGlobal(min.GetPosition());
        this.max = this.ToGlobal(max.GetPosition());
        this.dist = this.min.DistanceTo(this.max);
        this.speedFactor = 1 / (this.dist / this.speed);
    }

    private List<Entity> GetRiders() {
        // TODO: We could likely be more efficient about this in the future
        // via more broadphase techniques instead of grabbing all entities.
        List<Entity> entities = this.entity.scene.GetManager().GetEntities();
        List<Entity> riders = entities
            .Where((e) =>
                e.GetComponent<PlatformRider>() != null &&
                e.GetComponent<PlatformRider>().IsRiding(this.entity as Solid)
            )
            .ToList();
        return riders;
    }
    private List<Entity> GetEntities() => this.entity.scene.GetManager().GetEntities();

    public bool Overlaps(Entity other) =>
        other.IsCollidable && this.entity.GetCollider().Collides(other.GetCollider());

    public override void _PhysicsProcess(float delta) {
        this.time += delta;
        var riders = this.GetRiders();
        var entities = this.GetEntities();

        Vector2 from = this.min;
        Vector2 to = this.max;
        Vector2 desired = from.LinearInterpolate(to, Interpolation.PingPong(this.time * this.speedFactor, 1f));
        Vector2 deltaMove = desired - this.entity.GetPosition();
        this.Move(deltaMove, riders, entities);
    }

    public void Move(Vector2 move, List<Entity> riders, List<Entity> entities) {
        Vector2 clampedMove = this.entity.remainders.Update(move);

        if (clampedMove.x == 0 && clampedMove.y == 0) {
            return;
        }
        this.entity.IsCollidable = false;
        this.MoveX((int)clampedMove.x, riders, entities);
        this.MoveY((int)clampedMove.y, riders, entities);
        this.entity.IsCollidable = true;
    }

    public void MoveX(int x, List<Entity> riders, List<Entity> entities) {
        if (x == 0) {
            return;
        }
        this.entity.SetPosition(this.entity.GetPosition() + new Vector2(x, 0));
        Collider c = this.entity.GetCollider();
        foreach (Entity e in entities) {
            if (this.Overlaps(e) && e.GetComponent<PlatformPush>() != null) {
                Collider eCollider = e.GetCollider();
                float movePlayerX = x > 0 ? ((c.Right()) - (eCollider.Left())) : ((c.Left()) - (eCollider.Right()));
                if (e is PlayerNode && (e as PlayerNode).implementation is Player) {
                    var asPlayer = e as PlayerNode;
                    asPlayer.MoveX(movePlayerX, asPlayer.Kill);
                }
                else {
                    e.MoveX(movePlayerX, null);
                }
            }
            else if (riders.Contains(e)) {
                e.MoveX(x, null);
            }
        }
    }

    public void MoveY(int y, List<Entity> riders, List<Entity> entities) {
        if (y == 0) {
            return;
        }
        this.entity.SetPosition(this.entity.GetPosition() + new Vector2(0, y));
        Collider c = this.entity.GetCollider();
        foreach (Entity e in entities) {
            if (this.Overlaps(e) && e.GetComponent<PlatformPush>() != null) {
                Collider eCollider = e.GetCollider();
                float movePlayerY = y > 0 ? ((c.Bottom()) - (eCollider.Top())) : ((c.Top()) - (eCollider.Bottom()));
                if (e is PlayerNode && (e as PlayerNode).implementation is Player) {
                    var asPlayer = e as PlayerNode;
                    asPlayer.MoveY(movePlayerY, asPlayer.Kill);
                }
                else {
                    e.MoveY(movePlayerY, null);
                }
            }
            else if (riders.Contains(e)) {
                e.MoveY(y, null);
            }
        }
    }
}

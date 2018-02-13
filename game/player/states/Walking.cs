using System;
using Godot;

public class Walking : State<Player> {
    public override String GetName() { return "Walking"; }

    public override void OnEnter(float delta, Player player) {
        player.velocity *= new Vector2(1, 0);
        player.PlayAnimation(Player.Animation.Walking);
    }

    public override void OnExit(float delta, Player owner) {
    }

    private RayCast2D[] GetFeet(Player player) {
        return new RayCast2D[] {
            player.GetNode("BL") as RayCast2D,
            player.GetNode("BC") as RayCast2D,
            player.GetNode("BR") as RayCast2D
        };
    }

    // Detect if the player is going down the slope by
    // 1). Checking if one of the feet raycasts is colliding AND
    // 2). The collision normal of the foot is not a flat floor AND
    // 3). The collision normal x component is in the same direction of
    //      the player's velocity.
    private bool DetectDownSlope(RayCast2D[] feet, Player p) {
        bool enteringDown = false;
        Vector2 v = p.velocity;
        Vector2 floor = new Vector2(0, -1);

        foreach (RayCast2D foot in feet) {
            Vector2 collNormal = foot.GetCollisionNormal();
            if (foot.IsColliding() && collNormal.AngleTo(floor) != 0 && Math.Sign(collNormal.x) == Math.Sign(p.velocity.x)) {
                enteringDown = true;
            }
        }
        return enteringDown;
    }

    private bool DetectUpSlope(RayCast2D[] feet, Player p) {
        bool enteringUp = false;
        Vector2 v = p.velocity;
        Vector2 floor = new Vector2(0, -1);

        foreach (RayCast2D foot in feet) {
            Vector2 collNormal = foot.GetCollisionNormal();
            if (
                foot.IsColliding() &&
                collNormal.AngleTo(floor) != 0 &&
                Math.Sign(collNormal.x) == -Math.Sign(p.velocity.x)
            ) {
                enteringUp = true;
            }
        }
        return enteringUp;
    }

    private RayCast2D NearestSlope(RayCast2D[] feet, Player p, int x) {
        Vector2 floor = new Vector2(0, -1);
        Vector2 closest = new Vector2(float.MaxValue, float.MaxValue);
        RayCast2D closestCast = null;

        foreach (RayCast2D foot in feet) {
            Vector2 collNormal = foot.GetCollisionNormal();
            if (foot.IsColliding() && collNormal.AngleTo(floor) != 0 && Math.Sign(collNormal.x) == x) {
                Vector2 point = foot.GetCollisionPoint();
                if (point.y < closest.y) {
                    closest = point;
                    closestCast = foot;
                }
            }
        }
        return closestCast;
    }

    private RayCast2D NearestFloor(RayCast2D[] feet, Player p) {
        Vector2 floor = new Vector2(0, -1);
        Vector2 closest = new Vector2(float.MaxValue, float.MaxValue);
        RayCast2D closestCast = null;

        foreach (RayCast2D foot in feet) {
            Vector2 collNormal = foot.GetCollisionNormal();
            if (foot.IsColliding() && collNormal.Equals(floor)) {
                Vector2 point = foot.GetCollisionPoint();
                if (point.y < closest.y) {
                    closest = point;
                    closestCast = foot;
                }
            }
        }
        return closestCast;
    }

    private Vector2 GetNormalizedSlopeVector(Vector2 normal) {
        float angle = normal.AngleTo(new Vector2(0, -1));
        angle = angle < 0 ? (angle + Mathf.PI) : angle;

        return new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ).Normalized() * new Vector2(1, -1);
    }

    private Vector2 GetPlayerExtents(Player player) {
        CollisionShape2D shape = (CollisionShape2D)player.GetNode("CollisionShape2D");
        RectangleShape2D rect = (RectangleShape2D)shape.GetShape();
        return rect.GetExtents();
    }

    // When the player is moving horizontally, it can potentially
    // cause the player to fly off the edge of a slope. Instead,
    // this function will find the slope vector below the player
    // and adjust his velocity to create the illusion of following
    // that slope. This function could be enhanced in the future to
    // potentially "hack" into the way MoveAndSlide() works by converting
    // x velocity into downward y velocity to slide along the downward
    // slope.
    private void HandleGoingDownSlope(Player player, RayCast2D[] feet) {
        RayCast2D closestFoot = this.NearestSlope(
            feet,
            player,
            (int)Mathf.Sign(player.velocity.x)
        );

        Vector2 normal = closestFoot.GetCollisionNormal();

        // Slope vectors
        Vector2 upSlope = this.GetNormalizedSlopeVector(normal);
        Vector2 downSlope = upSlope * new Vector2(-1, -1);

        // TODO: We may still decide to keep this in the future.
        // First slam the player down to the slope
        // Vector2 pos = closestFoot.GetCollisionPoint();
        // pos += (this.GetPlayerExtents(player) * new Vector2(Mathf.Sign(normal.x), Mathf.Sign(normal.y)));
        // player.SetGlobalPosition(pos);

        // Manipulate player velocity so he stays on the slope going down
        player.velocity = downSlope * Math.Abs(player.velocity.x);
    }

    private void HandleGoingUpSlope(Player player, RayCast2D[] feet) {
        // Determine where the floor leveled off so we can
        // set the player back down on top of that.
        RayCast2D closestFoot = this.NearestFloor(feet, player);
        Vector2 extents = this.GetPlayerExtents(player);
        Vector2 pos = closestFoot.GetCollisionPoint();
        Vector2 point = player.GetGlobalPosition() - pos;
        Vector2 by = new Vector2(Mathf.Sign(point.x), Mathf.Sign(point.y));

        // Clamp the player back to the floor
        player.SetGlobalPosition(pos + (by * extents));
        player.velocity.y = 0;
    }

    private bool CurrentlyTouchingUpSlope(Player player, Vector2 preMoveVelocity) {
        Vector2 floor = new Vector2(0, -1);
        bool hitSlope = false;
        int collisionCount = player.GetSlideCount();
        if (collisionCount > 0) {
            KinematicCollision2D lastCollision = player.GetSlideCollision(0);
            if (lastCollision != null) {
                Vector2 lastNormal = lastCollision.GetNormal();
                if (lastNormal.AngleTo(floor) != 0 &&
                    Math.Sign(lastNormal.x) == -Math.Sign(preMoveVelocity.x)) {
                    hitSlope = true;
                }
            }
        }
        return hitSlope;
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        // Slope stuff
        Vector2 floor = new Vector2(0, -1);
        RayCast2D[] feet = this.GetFeet(player);
        bool onFloor = player.IsOnFloor();
        bool isSprinting = Input.IsActionPressed("key_sprint");
        player.DetectDirectionChange();
        Vector2 a = new Vector2(player.groundAcceleration * delta, 0);
        int isLeft = Input.IsActionPressed("key_left") ? 1 : 0;
        int isRight = Input.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        if (dir == 0) {
            return player.stateIdle;
        }
        if (onFloor && Input.IsActionJustPressed("key_jump")) {
            return player.stateJumping;
        }
        if(!onFloor) {
            return player.stateFalling;
        }

        float approachSpeed = dir * (isSprinting ? player.groundSprintMaxSpeed : player.groundMaxSpeed);
        player.velocity = Acceleration.ApproachX(
            approachSpeed,
            player.groundAcceleration,
            delta,
            player.velocity
        );
        player.ApplyGravity(delta);

        bool enteringDown = this.DetectDownSlope(feet, player);
        bool enteringUp = this.DetectUpSlope(feet, player);

        // Move the player
        Vector2 preMoveVelocity = player.velocity;
        player.Move(0f);

        bool hitSlope = this.CurrentlyTouchingUpSlope(player, preMoveVelocity);

        // Player has left the floor
        if (onFloor && !player.IsOnFloor()) {
            if (enteringDown) {
                this.HandleGoingDownSlope(player, feet);
            }
            else if (enteringUp) {
                this.HandleGoingUpSlope(player, feet);
            }
        }
        // Player is still on the floor and currently on the up-slope
        if (onFloor && hitSlope) {
            // We don't want the slope to slow down our horizontal movement
            player.velocity.x = preMoveVelocity.x;
        }
        return null;
    }
}

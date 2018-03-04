using System;
using Godot;

public class Gliding : State<Player> {

    public override void OnEnter(float delta, Player owner) {
        owner.PlayAnimation(Player.Animation.Gliding);
    }

    public override void OnExit(float delta, Player player) {
        Sprite playerSprite = (Sprite)player.GetNode("Spritesheet");
        //playerSprite.SetRotation(0);
        player.node.SetRotation(0);

    }

    private Vector2 AddLift(Player player, float delta) {
        return Acceleration.ApplyY(
            player.glideLift * Math.Abs(player.velocity.x) * -1,
            delta,
            player.velocity
        );
    }

    private Vector2 AddDrag(Player player, float delta) {
        return Acceleration.ApplyX(
            player.glideDrag * player.GetDirectionMultiplier() * -1,
            delta,
            player.velocity
        );
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {
        if(!Input.IsActionPressed("key_jump")) {
            return player.stateFalling;
        }

        player.DetectDirectionChange();

        // TEMPORARY UNTIL WE HAVE REAL SPRITES
        Sprite playerSprite = (Sprite)player.GetNode("Spritesheet");
        if (player.direction == Player.Direction.Left) {
            //playerSprite.SetRotation(-1.5708f); // -90 deg
            player.SetRotation(-1.5708f);
 
        } else {
            //playerSprite.SetRotation(1.5708f); // 90 deg
            player.SetRotation(1.5708f);
        }
        player.velocity.x = Math.Abs(player.velocity.x) * player.GetDirectionMultiplier();

        player.velocity = AddLift(player, delta);
        player.velocity = AddDrag(player, delta);
        player.ApplyGravity(delta);
        player.velocity.y = Math.Min(player.glideMaxYSpeed, player.velocity.y);

        /*if (Math.Abs(player.velocity.x) <= player.glideMinXSpeed) {
            player.velocity.x = player.glideMinXSpeed * player.GetDirectionMultiplier();
        } */  

        player.MoveAndSlide(player.velocity);

        if (player.IsOnFloor()) {
            return player.stateIdle;
        }
        if (player.IsOnWall()) {
            return player.stateKnockback;
        }
        return null;
    }

}

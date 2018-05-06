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

    public override State<Player> Update(float delta, Player player, float timeInState) {
        // Slope stuff
        bool onFloor = player.IsOnFloor();
        bool isSprinting = player.IsActionPressed("key_sprint");
        bool isDown = player.IsActionPressed("key_down");
        bool isJump = player.IsActionJustPressed("key_jump");
        player.DetectDirectionChange();
        int isLeft = player.IsActionPressed("key_left") ? 1 : 0;
        int isRight = player.IsActionPressed("key_right") ? 1 : 0;
        int dir = isRight - isLeft;

        if (dir == 0) {
            return player.stateIdle;
        }
        if (onFloor && isJump && !isDown) {
            return player.stateJumping;
        }
        if(!onFloor) {
            return player.stateFalling;
        }
        if(isDown && isJump) {
            player.fallThroughPlatform = true;
        }

        player.GroundControl(delta);
        player.ApplyGravity(delta);
        player.MoveX(player.velocity.x * delta, () => {
            player.velocity.x = 0;
        });
        player.MoveY(player.velocity.y * delta, () => {
            player.velocity.y = 0;
        });
        return null;
    }
}

using System;
using Godot;

public class PlayerRecorderFrame {
    public Vector2 Position;
    public Player.Direction Direction;
    public Player.Animation Animation;

    public PlayerRecorderFrame(Vector2 position, Player.Direction direction, Player.Animation animation) {
        this.Position = position;
        this.Direction = direction;
        this.Animation = animation;
    }

    public static PlayerRecorderFrame FromPlayer(Player player) =>
        new PlayerRecorderFrame(
            player.GetPosition(),
            player.direction,
            player.CurrentAnimation);

    public Player ApplyOnPlayer(Player player) {
        player.SetPosition(this.Position);
        player.PlayAnimation(this.Animation);
        player.direction = this.Direction;
        return player;
    }

}

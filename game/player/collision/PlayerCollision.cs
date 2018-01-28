using System;
using Godot;

public class PlayerCollision {
    private Player player;

    public bool isOnGround = false;

    public PlayerCollision(Player player) {
        this.player = player;
    }

    private bool CheckGround() {
        // TODO: This is a very primitive check (and has flaws)
        // If you are 1px above the ground, this will give a false-positive
        return this.player.TestMove(this.player.GetTransform(), new Vector2(0, 1));
    }

    public void Update() {
        this.isOnGround = CheckGround();
    }
}

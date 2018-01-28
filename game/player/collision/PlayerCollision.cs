using System;
using Godot;

public class PlayerCollision {
    private Player player;

    public bool isOnGround = false;

    public PlayerCollision(Player player) {
        this.player = player;
    }

    private bool CheckGround() {
        Vector2 p = this.player.GetGlobalPosition();
        CollisionShape2D collider = (CollisionShape2D) this.player.GetNode("CollisionShape2D");
        RectangleShape2D rect = (RectangleShape2D) collider.GetShape();
        Vector2 extent = rect.GetExtents();

        // Prime raycast origins
        Vector2 bottomRight = p + extent * (new Vector2(1, 1));
        Vector2 bottomLeft = p + extent * (new Vector2(-1, 1));

        GD.Print("VBr: ", bottomRight);
        GD.Print("VBl: ", bottomLeft);
        // GD.Print(rect);
        // GD.Print(rect.GetExtents());

        // TODO: This is a very primitive check (and has flaws)
        // If you are 1px above the ground, this will give a false-positive
        return this.player.TestMove(this.player.GetTransform(), new Vector2(0, 1));
    }

    public void Update() {
        this.isOnGround = CheckGround();
    }
}

using Godot;
using System;

public class GridCollider : Collider {
    public override bool Collides(BoxCollider other) {
        TileMap parent = (TileMap) this.GetNodeOwner();
        return false;
    }
}

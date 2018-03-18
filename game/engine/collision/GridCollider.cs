using Godot;
using System;

public class GridCollider : Collider {
    private TileMap map;

    public void SetMap(TileMap map) {
        this.map = map;
    }

    public TileMap GetMap() => this.map;

    public override bool Collides(BoxCollider other) {
        return false;
    }
}

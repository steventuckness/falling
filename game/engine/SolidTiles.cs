using Godot;
using System;

public class SolidTiles : Solid {
    private TileMap map;

    public override void _Ready() {
        base._Ready();
        this.map = (TileMap) this.GetNode("TileMap");
        GridCollider grid = (GridCollider) this.GetCollider();

        grid.SetMap(this.map);
    }

    public TileMap GetMap() => this.map;
}

using Godot;
using System;

public struct SubPixelFloat {
    public Vector2 remainders;
    public void Reset() {
        remainders = new Vector2();
    }
    public Vector2 Update(Vector2 amount) {
        this.remainders += amount;
        Vector2 move = new Vector2(this.remainders.x, this.remainders.y);
        move.x = (int) Math.Round(move.x);
        move.y = (int) Math.Round(move.y);
        this.remainders -= move;
        return move;
    }

    public int UpdateX(float x) {
        this.remainders.x += x;
        int moveX = (int) Math.Round(this.remainders.x);
        this.remainders.x -= moveX;
        return moveX;
    }

    public int UpdateY(float y) {
        this.remainders.y += y;
        int moveY = (int) Math.Round(this.remainders.y);
        this.remainders.y -= moveY;
        return moveY;
    }
}

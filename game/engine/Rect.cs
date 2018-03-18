using Godot;
using System;

namespace Rect {
    public static class Rect {
        public static Vector2[] Points(this Rect2 rect) {
            return new Vector2[]{
                new Vector2(rect.Left(), rect.Top()),
                new Vector2(rect.Right(), rect.Top()),
                new Vector2(rect.Right(), rect.Bottom()),
                new Vector2(rect.Left(), rect.Bottom())
            };
        }
        public static float Left(this Rect2 rect) {
            return rect.Position.x;
        }
        public static float Right(this Rect2 rect) {
            return rect.Position.x + rect.Size.x;
        }
        public static float Top(this Rect2 rect) {
            return rect.Position.y;
        }
        public static float Bottom(this Rect2 rect) {
            return rect.Position.y + rect.Size.y;
        }
        public static bool Overlaps(this Rect2 rect, Rect2 other) {
            return (
                rect.Left() < other.Right() &&
                rect.Right() > other.Left() &&
                rect.Bottom() > other.Top() && 
                rect.Top() < other.Bottom()
            );
        }
    }
}

using System.Numerics;

namespace SharpSand;

public static class Direction {
    public static readonly Vector2 None = new Vector2(0, 0);

    public static readonly Vector2 Up = new Vector2(0, -1);
    public static readonly Vector2 Down = new Vector2(0, 1);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 Right = new Vector2(1, 0);
    public static readonly Vector2 UpLeft = new Vector2(-1, -1);
    public static readonly Vector2 UpRight = new Vector2(1, -1);
    public static readonly Vector2 DownLeft = new Vector2(-1, 1);
    public static readonly Vector2 DownRight = new Vector2(1, 1);

    public static readonly Vector2[] Horizontal = new Vector2[] { Direction.Left, Direction.Right };
    public static readonly Vector2[] Vertical = new Vector2[] { Direction.Up, Direction.Down };

    public static readonly Vector2[] Upward = new Vector2[] { Direction.Up, Direction.UpLeft, Direction.UpRight };
    public static readonly Vector2[] Downward = new Vector2[] { Direction.Down, Direction.DownLeft, Direction.DownRight };

    public static readonly Vector2[] LowerHalf = new Vector2[] { Direction.Left, Direction.DownLeft, Direction.Down, Direction.DownRight, Direction.Right };
    public static readonly Vector2[] UpperHalf = new Vector2[] { Direction.Left, Direction.UpLeft, Direction.Up, Direction.UpRight, Direction.Right };

    public static readonly Vector2[] Cardinal = new Vector2[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
    public static readonly Vector2[] Intercardinal = new Vector2[] { Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight };

    public static readonly Vector2[] Full = new Vector2[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right, Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight };

    public static Vector2 Reverse(Vector2 direction) {
        if (direction == Up) return Down;
        if (direction == Down) return Up;
        if (direction == Left) return Right;
        if (direction == Right) return Left;
        if (direction == UpLeft) return DownRight;
        if (direction == UpRight) return DownLeft;
        if (direction == DownLeft) return UpRight;
        if (direction == DownRight) return UpLeft;
        return None;
    }
}
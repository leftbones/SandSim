using System.Numerics;

namespace SharpSand;

public static class Direction {
    public static readonly Vector2 None = new Vector2(0, 0);

    // Directions
    public static readonly Vector2 Up = new Vector2(0, -1);
    public static readonly Vector2 Down = new Vector2(0, 1);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 Right = new Vector2(1, 0);
    public static readonly Vector2 UpLeft = new Vector2(-1, -1);
    public static readonly Vector2 UpRight = new Vector2(1, -1);
    public static readonly Vector2 DownLeft = new Vector2(-1, 1);
    public static readonly Vector2 DownRight = new Vector2(1, 1);

    // Direction Sets
    public static readonly List<Vector2> Vertical = new List<Vector2>() { Direction.Up, Direction.Down };
    public static readonly List<Vector2> Horizontal = new List<Vector2>() { Direction.Left, Direction.Right };

    public static readonly List<Vector2> Upward = new List<Vector2>() { Direction.Up, Direction.UpLeft, Direction.UpRight };
    public static readonly List<Vector2> Downward = new List<Vector2>() { Direction.Down, Direction.DownLeft, Direction.DownRight };

    public static readonly List<Vector2> DiagonalUp = new List<Vector2>() { Direction.UpLeft, Direction.UpRight };
    public static readonly List<Vector2> DiagonalDown = new List<Vector2>() { Direction.DownLeft, Direction.DownRight };

    public static readonly List<Vector2> UpperHalf = new List<Vector2>() { Direction.Left, Direction.UpLeft, Direction.Up, Direction.UpRight, Direction.Right };
    public static readonly List<Vector2> LowerHalf = new List<Vector2>() { Direction.Left, Direction.DownLeft, Direction.Down, Direction.DownRight, Direction.Right };

    public static readonly List<Vector2> Cardinal = new List<Vector2>() { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
    public static readonly List<Vector2> Diagonal = new List<Vector2>() { Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight };

    public static readonly List<Vector2> Full = new List<Vector2>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right, Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight };

    // Direction Sets (Shuffled Order)
    public static List<Vector2> ShuffledVertical { get { return Vertical.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledHorizontal { get { return Horizontal.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledUpward { get { return Upward.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledDownward { get { return Downward.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledDiagonalUp { get { return DiagonalUp.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledDiagonalDown { get { return DiagonalDown.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledUpperHalf { get { return UpperHalf.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledLowerHalf { get { return LowerHalf.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledCardinal { get { return Cardinal.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledDiagonal { get { return Diagonal.OrderBy(a => RNG.Random.Next()).ToList(); } }
    public static List<Vector2> ShuffledFull { get { return Full.OrderBy(a => RNG.Random.Next()).ToList(); } }

    // Random Direction from Direction Set
    public static Vector2 RandomVertical { get { return Direction.Vertical[RNG.Range(0, 1)]; } }
    public static Vector2 RandomHorizontal { get { return Direction.Horizontal[RNG.Range(0, 1)]; } }
    public static Vector2 RandomUpward { get { return Direction.Upward[RNG.Range(0, 2)]; } }
    public static Vector2 RandomDownward { get { return Direction.Downward[RNG.Range(0, 2)]; } }
    public static Vector2 RandomDiagonalUp { get { return Direction.DiagonalUp[RNG.Range(0, 1)]; } }
    public static Vector2 RandomDiagonalDown { get { return Direction.DiagonalDown[RNG.Range(0, 1)]; } }
    public static Vector2 RandomUpperHalf { get { return Direction.UpperHalf[RNG.Range(0, 4)]; } }
    public static Vector2 RandomLowerHalf{ get { return Direction.LowerHalf[RNG.Range(0, 4)]; } }
    public static Vector2 RandomCardinal { get { return Direction.Cardinal[RNG.Range(0, 3)]; } }
    public static Vector2 RandomDiagonal { get { return Direction.Diagonal[RNG.Range(0, 3)]; } }
    public static Vector2 RandomFull { get { return Direction.Full[RNG.Range(0, 7)]; } }


    // Returns the reverse of the given direction
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

    // Returns the given direction mirrored vertically
    public static Vector2 MirrorVertical(Vector2 direction) {
        if (direction == Up) return Down;
        if (direction == Down) return Up;
        if (direction == UpLeft) return DownLeft;
        if (direction == UpRight) return DownRight;
        if (direction == DownLeft) return UpLeft;
        if (direction == DownRight) return UpRight;
        return None;
    }

    public static Vector2 MirrorHorizontal(Vector2 direction) {
        if (direction == Left) return Right;
        if (direction == Right) return Left;
        if (direction == UpLeft) return UpRight;
        if (direction == UpRight) return UpLeft;
        if (direction == DownLeft) return DownRight;
        if (direction == DownRight) return DownLeft;
        return None;
    }

    public static Vector2 GetMovementDirection(Vector2 a, Vector2 b) {
        return Vector2.Normalize(b - a);
    }
}
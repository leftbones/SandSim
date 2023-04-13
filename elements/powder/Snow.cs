using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Snow : Powder {
    public Snow(Vector2 position) : base(position) {
        Friction = 25;
        SetColor(new Color(255, 255, 255, 255));
    }

    public override void Update(Matrix matrix) {
        if (Settled)
            if (matrix.IsValid(Position + Direction.Down))
                Settled = false;
            else
                return;

        int DriftChance = 10;
        if (!matrix.IsValid(Position + Direction.Down))
            DriftChance = 90;

        // Left, Right, DownLeft, DownRight (Shuffled)
        List<Vector2> Directions = new List<Vector2>{Direction.Left, Direction.Right, Direction.DownLeft, Direction.DownRight};
        Directions = Directions.OrderBy(a => RNG.Random.Next()).ToList();

        foreach (Vector2 D in Directions) {
            if (RNG.Chance(DriftChance) && matrix.Swap(Position, Position + D))
                return;
        }

        // Down
        if (RNG.CoinFlip() && matrix.Swap(Position, Position + Direction.Down))
            return;

        // Sliding
        if (RNG.Chance(Friction)) {
            Settled = true;
            return;
        }

        int Dir = 1;
        if (RNG.CoinFlip())
            Dir = -1;

        if (matrix.Swap(Position, Position + new Vector2(Dir, 0)))
            return;

        // No movement
        Settled = true;
    }
}
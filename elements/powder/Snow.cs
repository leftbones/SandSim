using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Snow : Powder {
    public Snow(Vector2 position) : base(position) {
        Friction = 25;
        BaseColor = new Color(255, 255, 255, 255);
        ModifyColor();
    }

    public override void Update(Matrix matrix) {
        if (Settled) {
            if (matrix.IsEmpty(Position + Direction.Down))
                Settled = false;
            else
                return;
        }

        int DriftChance = 10;
        if (!matrix.IsEmpty(Position + Direction.Down))
            DriftChance = 90;

        // Left, Right, DownLeft, DownRight, Down (Shuffled)
        List<Vector2> Directions = Direction.LowerHalf.OrderBy(a => RNG.Random.Next()).ToList();

        foreach (Vector2 D in Directions) {
            if (RNG.Chance(DriftChance) && matrix.SwapIfEmpty(Position, Position + D))
                return;
        }

        // Down
        if (RNG.CoinFlip() && matrix.SwapIfEmpty(Position, Position + Direction.Down))
            return;

        // Sliding
        if (RNG.Chance(Friction)) {
            Settled = true;
            return;
        }

        int Dir = 1;
        if (RNG.CoinFlip())
            Dir = -1;

        if (matrix.SwapIfEmpty(Position, Position + new Vector2(Dir, 0)))
            return;

        // No movement
        if (Position == LastPosition)
            Settled = true;
    }
}
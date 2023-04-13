using System.Numerics;
using Raylib_cs;

namespace SharpSand;

////
// Base Powder
// - Attempts to move Down first, then DownLeft or DownRight (random choice) if that fails
// - Chance to move Left or Right depending on Friction property

abstract class Powder : Element {
    public Powder(Vector2 position) : base(position) {
        Type = ElementType.Powder;
    }

    public override void Update(Matrix matrix) {
        if (Settled)
            if (matrix.IsValid(Position + Direction.Down))
                Settled = false;
            else
                return;
        
        // Down
        if (matrix.Swap(Position, Position + Direction.Down))
            return;

        // Down Left + Down Right
        if (matrix.Swap(Position, Position + Direction.DownLeft))
            return;

        if (matrix.Swap(Position, Position + Direction.DownRight))
            return;

        // Sliding
        if (RNG.Chance(Friction)) {
            Settled = true;
            return;
        }

        int Dir = 1;
        if (RNG.CoinFlip())
            Dir = -Dir;

        if (matrix.Swap(Position, Position + new Vector2(Dir, 0)))
            return;

        Settled = true;
    }
}
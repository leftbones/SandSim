using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Liquid : Element {
    public Liquid(Vector2 position) : base(position) {
        Type = ElementType.Liquid;
    }

    public override void Update(Matrix matrix) {
        if (Settled) {
            if (matrix.IsEmpty(Position + Direction.Down) || matrix.IsEmpty(Position + Direction.Left) || matrix.IsEmpty(Position + Direction.Right))
                Settled = false;
            else
                return;
        }

        // Small chance to move down + left/right
        if (RNG.Chance(5)) {
            if (RNG.CoinFlip()) {
                if (matrix.SwapIfEmpty(Position, Position + Direction.DownLeft))
                    return;

                if (matrix.SwapIfEmpty(Position, Position + Direction.DownRight))
                    return;
            } else {
                if (matrix.SwapIfEmpty(Position, Position + Direction.DownRight))
                    return;

                if (matrix.SwapIfEmpty(Position, Position + Direction.DownLeft))
                    return;
            }
        }

        // Move down (through gasses if necessary)
        if (matrix.SwapIfEmpty(Position, Position + Direction.Down))
            return;

        if (matrix.IsGas(Position + Direction.Down) && matrix.Swap(Position, Position + Direction.Down))
            return;

        // Move left/right
        if (matrix.Tick % 2 == 0) {
            for (int i = 0; i < DispersionRate; i++) {
                if (RNG.CoinFlip() && !matrix.SwapIfEmpty(Position, Position + Direction.Left)) {
                    Settled = true;
                    return;
                }
            }
        } else {
            for (int i = 0; i < DispersionRate; i++) {
                if (RNG.CoinFlip() && !matrix.SwapIfEmpty(Position, Position + Direction.Right)) {
                    Settled = true;
                    return;
                }
            }
        }

        // No movement
        if (Position == LastPosition)
            Settled = true;
    }
}
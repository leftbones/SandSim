using System.Numerics;
using Raylib_cs;

namespace SharpSand;


abstract class Gas : Element {
    public Gas(Vector2i position) : base(position) {
        Type = ElementType.Gas;
        Spread = 3.0f;
    }

    public override void Step(Matrix matrix) {
        // Move upward if the space above contains liquid (regardless of density)
        if (matrix.SwapIfType(Position, Position + Direction.Up, ElementType.Liquid))
            return;

        // Move upward if the space above contains a denser gas
        if (matrix.SwapIfLessDense(Position, Position + Direction.Up))
            return;

        // Chance to not move at all
        if (RNG.Chance(25))
            return;

        // Attempt to move up/down based on density relative to air
        List<Vector2i> Directions = Density < 0 ? Direction.ShuffledUpward : Direction.ShuffledDownward;
        if (Density == 0.0f) Directions = Direction.ShuffledFull;
        foreach (Vector2i MoveDir in Directions) {
            if (RNG.Roll(Math.Abs(Density)) && matrix.SwapIfEmpty(Position, Position + MoveDir))
                return;
        }

        // Move left/right based on spread rate and the current tick
        Vector2i Dir = matrix.Tick % 2 == 0 ? Direction.Left : Direction.Right;
        for (int i = 0; i < Spread; i++) {
            if (RNG.CoinFlip() && !matrix.SwapIfEmpty(Position, Position + Dir)) {
                return;
            }
        }
    }
}


////
// Air doesn't really need it's own file because it doesn't do anything
class Air : Gas {
    public Air(Vector2i position) : base(position) {
        Color = Color.BLACK;
    }

    public override void Step(Matrix matrix) { }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;


abstract class Gas : Element {
    public Gas(Vector2i position) : base(position) {
        Health = 1.0f;
        Type = ElementType.Gas;
        Spread = 3.0f;
        Drift = 0.2f;
    }

    public override void Step(Matrix matrix) {
        // Chance to move horizontally based on drift if density is not equal to air
        if (Density != 0.0f) {
            foreach (Vector2i DriftDir in Direction.ShuffledHorizontal) {
                if (RNG.Roll(Drift)) {
                    if (matrix.SwapIfEmpty(Position, Position + DriftDir))
                        return;
                }
            }
        }

        // Move upward if the space above contains liquid (regardless of density)
        if (matrix.SwapIfType(Position, Position + Direction.Up, ElementType.Liquid))
            return;

        // Move upward if the space above contains a denser gas
        if (matrix.SwapIfLessDense(Position, Position + Direction.Up))
            return;

        // Chance to not move at all
        if (RNG.Chance(25))
            return;

        // Attempt to move up/down based on density relative to air, moves in any direction if density is equal to air
        float Roll = Math.Abs(Density);
        List<Vector2i> Directions = Density < 0 ? Direction.ShuffledUpward : Direction.ShuffledDownward;
        if (Density == 0.0f) {
            Directions = Direction.ShuffledFull;
            Roll = 0.1f;
        }

        foreach (Vector2i MoveDir in Directions) {
            if (RNG.Roll(Roll) && matrix.SwapIfEmpty(Position, Position + MoveDir))
                return;
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
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Liquid : Element {
    public Liquid(Vector2 position) : base(position) {
        Type = ElementType.Liquid;
    }

    public override void Step(Matrix matrix) {
        // Unsettle if down/left/right space is empty
        if (Settled) {
            if (matrix.IsTypeOrEmpty(Position + Direction.Down, ElementType.Gas) || matrix.IsTypeOrEmpty(Position + Direction.Left, ElementType.Gas) || matrix.IsTypeOrEmpty(Position + Direction.Right, ElementType.Gas))
                Settled = false;
            else
                return;
        }

        // Chance to move down + left/right when falling
        if (RNG.Chance(5)) {
            List<Vector2> DLDR = Direction.ShuffledDiagonalDown;
            if (matrix.SwapIfTypeOrEmpty(Position, Position + DLDR[0], ElementType.Gas))
                return;

            if (matrix.SwapIfTypeOrEmpty(Position, Position + DLDR[1], ElementType.Gas))
                return;
        }

        // Move down if space is empty or contains gas
        if (matrix.SwapIfTypeOrEmpty(Position, Position + Direction.Down, ElementType.Gas))
            return;

        // Move left/right based on the current tick
        Vector2 MoveDir = matrix.Tick % 2 == 0 ? Direction.Left : Direction.Right;
        for (int i = 0; i < (int)Spread; i++) {
            if (RNG.CoinFlip() && !matrix.SwapIfTypeOrEmpty(Position, Position + MoveDir, ElementType.Gas)) {
                Settled = true;
                return;
            }

            else if (RNG.Chance(50 + (i * 2)) && matrix.IsTypeOrEmpty(Position + Direction.Down, ElementType.Gas))
                return;
        }
    }
}
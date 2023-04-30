using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Liquid : Element {
    public Liquid(Vector2i position) : base(position) {
        Health = 10.0f;
        Type = ElementType.Liquid;
        Density = 100.0f;
    }

    public override void Step(Matrix matrix) {
        // Unsettle if down/left/right space is empty or contains a gas
        if (Settled) {
			foreach (Vector2i Dir in Direction.LowerHalf)
				if (matrix.IsTypeOrEmpty(Position + Dir, ElementType.Gas))
					Settled = false;
        }

		if (Settled)
			return;

        // Chance to move down + left/right when falling
        if (RNG.Chance(5)) {
            List<Vector2i> DLDR = Direction.ShuffledDiagonalDown;
            if (matrix.SwapIfTypeOrEmpty(Position, Position + DLDR[0], ElementType.Gas))
                return;

            if (matrix.SwapIfTypeOrEmpty(Position, Position + DLDR[1], ElementType.Gas))
                return;
        }

        // Move down if space is empty or contains a less dense liquid or contains gas
        if (matrix.SwapIfLessDense(Position, Position + Direction.Down) || matrix.SwapIfTypeOrEmpty(Position, Position + Direction.Down, ElementType.Gas))
            return;

        // Move left/right based on the current tick
        Vector2i MoveDir = matrix.Tick % 2 == 0 ? Direction.Left : Direction.Right;
        for (int i = 0; i < Spread; i++) {
            if (RNG.CoinFlip() && !matrix.SwapIfTypeOrEmpty(Position, Position + MoveDir, ElementType.Gas)) {
                Settled = true;
                return;
            }

            else if (RNG.Chance(50 + (i * 2)) && matrix.IsTypeOrEmpty(Position + Direction.Down, ElementType.Gas))
                return;
        }
    }
}
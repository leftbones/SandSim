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

    public override void Step(Matrix matrix) {
        // Unsettle if space below is empty
        if (Settled) {
            if (matrix.IsEmpty(Position + Direction.Down))
                Settled = false;
            else
                return;
        }

        // Move down if space is empty or contains liquid, chance to become settled if it fails
        if (matrix.SwapIfTypeOrEmpty(Position, Position + Direction.Down, ElementType.Liquid)) {
            return;
        } else if (RNG.Roll(Friction)) {
            Settled = true;
            return;
        }

        // Move down left/right (in last direction or random if last direction is not down left/right) if space is empty or contains liquid or gas
        Vector2 MoveDir = LastDirection;
        if (!Direction.DiagonalDown.Contains(LastDirection))
            MoveDir = Direction.RandomDiagonalDown;

        if (matrix.SwapIfTypeOrEmpty(Position, Position + MoveDir, ElementType.Liquid) ||
            matrix.SwapIfTypeOrEmpty(Position, Position + MoveDir, ElementType.Gas))
            return;

        if (matrix.SwapIfTypeOrEmpty(Position, Position + Direction.MirrorHorizontal(MoveDir), ElementType.Liquid) ||
            matrix.SwapIfTypeOrEmpty(Position, Position + MoveDir, ElementType.Gas))
            return;

        // Chance to become settled based on friction value
        if (RNG.Roll(Friction)) {
            Settled = true;
            return;
        }

        // Move left/right (in last direction) if space is empty, become settled if it fails
        MoveDir = LastDirection;
        if (!Direction.Horizontal.Contains(LastDirection))
            MoveDir = Direction.RandomHorizontal;

        if (matrix.SwapIfEmpty(Position, Position + MoveDir)) {
            Settled = true;
            return;
        }
    }
}
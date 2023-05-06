using System.Numerics;
using Raylib_cs;

namespace SharpSand;

////
// Base Powder
abstract class Powder : Element {
    public Powder(Vector2i position) : base(position) {
        Health = 10.0f;
        Type = ElementType.Powder;
    }

    public override void Step(Matrix matrix) {
        // Unsettle if space below is empty
        if (Settled) {
            if (matrix.IsEmpty(Position + Direction.Down))
                Settled = false;
            else if (matrix.GetNeighbors(Position).Any(n => n.GetType() == typeof(Water) || n.GetType() == typeof(Saltwater)))
                Settled = false;
            else
                return;
        }

        // Move down if space is empty or contains liquid or gas
        if (matrix.SwapIfTypeOrEmpty(Position, Position + Direction.Down, ElementType.Gas)) {
            return;
        } else if (matrix.IsLiquid(Position + Direction.Down) && RNG.Roll(0.05f)) {
            matrix.Swap(Position, Position + Direction.Down);
            return;
        }

        // Move down left/right (in last direction or random if last direction is not down left/right) if space is empty or contains liquid or gas
        Vector2i MoveDir = LastDirection;
        if (!Direction.DiagonalDown.Contains(LastDirection))
            MoveDir = Direction.RandomDiagonalDown;

        if (matrix.SwapIfTypeOrEmpty(Position, Position + MoveDir, ElementType.Gas)) return;
        if (matrix.SwapIfTypeOrEmpty(Position, Position + Direction.MirrorHorizontal(MoveDir), ElementType.Gas)) return;

        if (matrix.IsLiquid(Position + MoveDir) && RNG.Roll(0.05f)) {
            matrix.Swap(Position, Position + MoveDir);
            return;
        }

        if (matrix.IsLiquid(Position + Direction.MirrorHorizontal(MoveDir)) && RNG.Roll(0.05f)) {
            matrix.Swap(Position, Position + Direction.MirrorHorizontal(MoveDir));
            return;
        }

        // Move left/right (prefer last direction) if space is empty, become settled if it fails, chance to become settled on success
        MoveDir = LastDirection;
        if (!Direction.Horizontal.Contains(LastDirection))
            MoveDir = Direction.RandomHorizontal;

        if (!matrix.SwapIfEmpty(Position, Position + MoveDir))
            Settled = true;

        // Chance to become settled based on friction value
        if (RNG.Roll(Friction)) {
            Settled = true;
            return;
        }
    }
}
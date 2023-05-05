using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Snow : Powder {
    public Snow(Vector2i position) : base(position) {
        Friction = 0.6f;
        Drift = 0.3f;
        Dissolvable = 100;
        BaseColor = new Color(255, 255, 255, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        // Chance to drift horizontally when in the air
        if (RNG.Roll(Drift) && matrix.IsEmpty(Position + Direction.Down)) {
            foreach (Vector2i MoveDir in Direction.ShuffledHorizontal) {
                if (matrix.SwapIfEmpty(Position, Position + MoveDir))
                    return;
            }
        }

        base.Step(matrix);
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}
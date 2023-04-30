using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Soot : Powder {
    public Soot(Vector2i position) : base(position) {
        Health = 1;
        Friction = 0.01f;
        Drift = 0.1f;
        Flammability = 0.1f;
        BaseColor = new Color(93, 92, 94, 255);
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
        OnFire = true;
    }
}
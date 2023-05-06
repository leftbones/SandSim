using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Soot : Powder {
    public Soot(Vector2i position) : base(position) {
        Health = 1;
        Friction = 0.01f;
        Drift = 0.1f;
        Flammability = 0.1f;
        Dissolvable = 100;
        BaseColor = new Color(93, 92, 94, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        base.Step(matrix);

        // Chance to drift horizontally when in the air
        if (!Settled) {
            if (RNG.Roll(Drift) && matrix.IsEmpty(Position + Direction.Down)) {
                foreach (Vector2i MoveDir in Direction.ShuffledHorizontal) {
                    if (matrix.SwapIfEmpty(Position, Position + MoveDir))
                        return;
                }
            }
        }
    }

    public override void HeatReaction(Matrix matrix) {
        OnFire = true;
    }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Snow : Powder {
    public Snow(Vector2 position) : base(position) {
        Friction = 0.6f;
        Drift = 0.3f;
        BaseColor = new Color(255, 255, 255, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        // Chance to drift horizontally
        foreach (Vector2 MoveDir in Direction.ShuffledHorizontal) {
            if (RNG.Roll(Drift) && matrix.SwapIfEmpty(Position, Position + MoveDir))
                return;
        }

        base.Step(matrix);
    }

    public override void ReceiveHeating(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ember : Powder {
    public Ember(Vector2 position) : base(position) {
        Lifetime = 300;
        Friction = 0.2f;
        Drift = 0.1f;
        IsHeating = true;
        ColorOffset = 35;
        BaseColor = new Color(163, 49, 76, 255);
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

    public override void ActOnOther(Matrix matrix, Element other) {
        if (!other.IsHeating && RNG.Roll(other.HeatPotential))
            other.ReceiveHeating(matrix);
    }

    public override void ReceiveCooling(Matrix matrix) {
        // Expire(matrix);
    }

    public override void Expire(Matrix matrix) {
        // Create smoke if space above is empty
        if (matrix.IsEmpty(Position + Direction.Up))
            matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));

        if (RNG.Chance(10))
            matrix.Set(Position, new Soot(Position));
        else
            matrix.Set(Position, new Air(Position));
    }
}
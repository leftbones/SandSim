using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ember : Powder {
    public Ember(Vector2 position) : base(position) {
        Lifetime = 300;
        Friction = 0.2f;
        IsHeating = true;
        ColorOffset = 35;
        BaseColor = new Color(163, 49, 76, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        // Heat neighbors (cardinal)
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (!matrix.IsEmpty(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }

        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        other.ReceiveHeating(matrix);
        base.ActOnOther(matrix, other);
    }

    public override void ReceiveCooling(Matrix matrix) {
        if (RNG.Roll(CoolPotential))
            Expire(matrix);
    }

    public override void Expire(Matrix matrix) {
        // Create smoke if space above is empty
        if (matrix.IsEmpty(Position + Direction.Up))
            matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));

        matrix.Set(Position, new Soot(Position));
    }
}
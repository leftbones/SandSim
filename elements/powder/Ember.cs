using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ember : Powder {
    public Ember(Vector2 position) : base(position) {
        Lifetime = 300;
        Friction = 50;
        HeatingFactor = 1.0f;
        CanBeHeated = false;
        ColorOffset = 35;
        SetColor(new Color(239, 71, 111, 255));
    }

    public override void Update(Matrix matrix) {
        // Heat neighbors
        foreach (Vector2 Dir in Direction.Full) {
            if (RNG.Chance(20) && matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }

        base.Update(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other.CanBeHeated) {
            other.ApplyHeating(matrix, HeatingFactor);
            other.Settled = false;
            Settled = false;
        }
    }

    public override void LifetimeExpire(Matrix matrix) {
        matrix.Set(Position, new Smoke(Position));
    }
}
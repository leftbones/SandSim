using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ember : Powder {
    public Ember(Vector2 position) : base(position) {
        Lifetime = 300;
        Friction = 50;
        ColorOffset = 35;
        BaseColor = new Color(163, 49, 76, 255);
        ModifyColor();
    }

    public override void Update(Matrix matrix) {
        // Heat neighbors
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (RNG.Chance(20) && matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }

        base.Update(matrix);
    }

    public override bool ActOnOther(Matrix matrix, Element other) {
        if (RNG.Chance(other.FireResistance)) {
            other.ApplyHeating(matrix);
            other.Settled = false;
            Settled = false;
            return true;
        }
        return false;
    }

    public override void Expire(Matrix matrix) {
        if (matrix.IsEmpty(Position + Direction.Up))
            matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));

        matrix.Set(Position, new Soot(Position));
    }
}
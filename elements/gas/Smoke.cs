using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Smoke : Gas {
    public Smoke(Vector2i position) : base(position) {
        Lifetime = RNG.Range(500, 750);
        Density = -0.3f;
        Drift = 0.4f;
        BaseColor = new Color(132, 136, 132, 150);
        ModifyColor();
    }

    public override void Expire(Matrix matrix) {
        if (RNG.Roll(0.1f))
            matrix.Set(Position, new Soot(Position));
        else
            matrix.Set(Position, new Air(Position));
    }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Smoke : Gas {
    public Smoke(Vector2 position) : base(position) {
        Lifetime = 1000;
        CanBeHeated = false;
        CanBeCooled = false;
        SetColor(new Color(132, 136, 132, 150));
    }

    public override void LifetimeExpire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Steam : Gas {
    public Steam(Vector2 position) : base(position) {
        Lifetime = 1000;
        CanBeHeated = false;
        CanBeCooled = false;
        SetColor(new Color(232, 239, 239, 150));
    }

    public override void LifetimeExpire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}
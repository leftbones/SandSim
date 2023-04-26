using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Steam : Gas {
    public Steam(Vector2i position) : base(position) {
        Lifetime = 750;
        Density = -0.1f;
        Drift = 0.7f;
        HeatFactor = 0.25f;
        BaseColor = new Color(232, 239, 239, 150);
        ModifyColor();
    }

    public override void Expire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}
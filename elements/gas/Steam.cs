using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Steam : Gas {
    public Steam(Vector2 position) : base(position) {
        Density = -0.1f;
        Lifetime = 750;
        BaseColor = new Color(232, 239, 239, 150);
        ModifyColor();
    }

    public override void Expire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}
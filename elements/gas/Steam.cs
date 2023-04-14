using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Steam : Gas {
    public Steam(Vector2 position) : base(position) {
        Lifetime = 750;
        DispersionRate = 10;
        BaseColor = new Color(232, 239, 239, 150);
        ModifyColor();
    }

    public override void Expire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}
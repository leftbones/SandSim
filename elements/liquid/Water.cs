using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    public Water(Vector2 position) : base(position) {
        Spread = 10.0f;
        CoolFactor = 0.5f;
        ColorOffset = 0;
        BaseColor = new Color(1, 151, 244, 255);
        ModifyColor();
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other.OnFire)
            other.ExtinguishFire();
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Steam(Position));
    }

    public override void CoolReaction(Matrix matrix) {
        matrix.Set(Position, new Ice(Position));
    }
}
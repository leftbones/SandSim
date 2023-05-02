using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    public Water(Vector2i position) : base(position) {
        Spread = 10.0f;
        CoolFactor = 1.0f;
        ColorOffset = 0;
        BaseColor = new Color(1, 151, 244, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (matrix.IsEmpty(Position + Direction.Up)) {
            if (RNG.Chance(75))
                Shimmer = true;
            else
                Shimmer = false;
        } else if (RNG.Chance(25)) {
            Shimmer = false;
        }

        ModifyColor();

        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other.OnFire)
            other.ExtinguishFire();
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Steam(Position));
    }

    public override void CoolReaction(Matrix matrix) {
        if (RNG.Chance(1))
            matrix.Set(Position, new Ice(Position));
    }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Saltwater : Liquid {
    private int RustChance = 10;

    public Saltwater(Vector2i position) : base(position) {
        Density = 101.0f;
        Spread = 10.0f;
        CoolFactor = 0.5f;
        ConductElectricity = 750;
        ColorOffset = 0;
        BaseColor = new Color(0, 120, 194, 255);
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

        if (other.Dissolvable > 0 && RNG.Roll(other.Dissolvable + (other.Dissolvable / 2))) {
            other.Dissolve(matrix);
            return;
        }

        if (other.GetType() == typeof(Steel) && RNG.Roll(RustChance)) {
            matrix.Set(other.Position, new Rust(other.Position));
        }
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Steam(Position));
    }

    public override void CoolReaction(Matrix matrix) {
        if (RNG.Chance(1))
            matrix.Set(Position, new Ice(Position));
    }
}

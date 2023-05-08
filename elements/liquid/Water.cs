using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    private int RustChance = 5;

    public Water(Vector2i position) : base(position) {
        Spread = 10.0f;
        Temperature = 20.0;
        CoolFactor = 0.5f;
        ConductHeat = 0.11;
        ConductElectricity = 100;
        ColorOffset = 0;
        BaseColor = new Color(1, 151, 244, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (Temperature < 0.0f) {
            Element NE = new Ice(Position);
            NE.Temperature = Temperature;
            matrix.Set(Position, NE);
        }

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

        if (other.Dissolvable > 0 && RNG.Roll(other.Dissolvable)) {
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
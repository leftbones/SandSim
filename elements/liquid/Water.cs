using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    public Water(Vector2 position) : base(position) {
        Spread = 10.0f;
        CoolPotential = 0.05f;
        ColorOffset = 0;
        BaseColor = new Color(1, 151, 244, 255);
        ModifyColor();
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other.IsHeating && RNG.Roll(other.CoolPotential)) {
            other.ReceiveCooling(matrix);
            ReceiveHeating(matrix);
        }

        if (other.IsCooling && RNG.Roll(other.HeatPotential))
            ReceiveCooling(matrix);

        if (other.OnFire)
            other.ExtinguishFire();
    }

    public override void ReceiveHeating(Matrix matrix) {
        matrix.Set(Position, new Steam(Position));
    }

    public override void ReceiveCooling(Matrix matrix) {
        matrix.Set(Position, new Ice(Position));
    }
}
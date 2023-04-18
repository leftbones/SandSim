using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Snow : Powder {
    public Snow(Vector2 position) : base(position) {
        Friction = 0.6f;
        BaseColor = new Color(255, 255, 255, 255);
        ModifyColor();
    }

    public override void ReceiveHeating(Matrix matrix) {
        if (RNG.Roll(HeatPotential))
            matrix.Set(Position, new Water(Position));
    }
}
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ice : Solid {
    public Ice(Vector2 position) : base(position) {
        HeatPotential = 0.1f;
        IsCooling = true;
        ActDirections = Direction.Diagonal;
        ForceAct = true;
        ColorOffset = 15;
        BaseColor = new Color(165, 242, 243, 255);
        ModifyColor();
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        // Apply cooling to non-cooling neighbors
        if (!other.IsCooling && RNG.Roll(other.CoolPotential))
            other.ReceiveCooling(matrix);
    }

    public override void ReceiveHeating(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}
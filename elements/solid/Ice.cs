using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ice : Solid {
    public Ice(Vector2i position) : base(position) {
        Health = 20.0f;
        Temperature = -28.0;
        CoolFactor = 2.0f;
        ConductHeat = 0.18;
        ActDirections = Direction.ShuffledDiagonal;
        ColorOffset = 15;
        BaseColor = new Color(165, 242, 243, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (Temperature > 0.0f) {
            Element NE = new Water(Position);
            NE.Temperature = Temperature;
            matrix.Set(Position, NE);
        }

        base.Step(matrix);
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}
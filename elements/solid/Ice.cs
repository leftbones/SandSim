using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ice : Solid {
    public Ice(Vector2 position) : base(position) {
        CoolFactor = 0.5f;
        ActDirections = Direction.Diagonal;
        ForceAct = true;
        ColorOffset = 15;
        BaseColor = new Color(165, 242, 243, 255);
        ModifyColor();
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}
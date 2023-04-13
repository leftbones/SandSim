using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ice : Solid {
    public Ice(Vector2 position) : base(position) {
        CanBeCooled = false;
        CoolingFactor = 0.5f;
        ColorOffset = 15;
        SetColor(new Color(165, 242, 243, 255));
    }

    public override void Update(Matrix matrix) {
        // Cool neighbors
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other is not Air && other.CanBeCooled)
            other.ApplyCooling(matrix, CoolingFactor);
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}